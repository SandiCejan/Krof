using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace KrofEngine
{
    internal class GameManager
    {
        private static List<IUpdate> updateables;
        private static bool sortRequested;
        public static List<IUpdate> updateablesToRemove;
        public static List<IScene> scenes;
        internal static List<GameObject> objectToDestroy = new();
        public static IScene LoadedScene { get; private set; }
        public static IScene RequestedScene { get; private set; }
        public static KeyboardState KeyboardState { get; private set; }
        public static KeyboardState PreviousKeyboardState { get; private set; }
        public static string PersistantGamePath { get; private set; }

        private int updateID;
        public static Settings Settings { get; private set; }
        internal static Action OnApplicationQuit;
        internal static Action OnResize;
        public static Vector2 MousePosition { get; private set; }
        public static MouseState MouseState { get; private set; }
        public static Vector2 PreviousMousePosition { get; private set; }
        public static MouseState PreviousMouseState { get; private set; }
        private static List<ISaveFile> saveFiles;
        private static Assembly assembly;
        public static bool isMouseVisible { get { return Game1.Instance.IsMouseVisible; } set { Game1.Instance.IsMouseVisible = value; } }
        public static T GetSaveFile<T>()
        {
            foreach (var item in saveFiles)
            {
                if (item.GetType() == typeof(T))
                {
                    return (T)item;
                }
            }
            return default;
        }
        public GameManager()
        {
            scenes = new();

            assembly = Assembly.GetExecutingAssembly();
            List<Type> SceneClasses = assembly.GetTypes()
                .Where(t => t.IsClass && typeof(IScene).IsAssignableFrom(t))
                .ToList();

            foreach (Type type in SceneClasses)
            {
                scenes.Add((IScene)Activator.CreateInstance(type));
            }
            updateables = new();
            updateablesToRemove = new(5);
            RequestedScene = scenes[0];
            saveFiles = new();
        }

        public static void AddUpdate(IUpdate updatable)
        {
            updateables.Add(updatable);
            sortRequested = true;
        }
        public static void AddOnApplicationQuit(Action action)
        {
            OnApplicationQuit += action;
        }

        public static void RemoveOnApplicationQuit(Action action)
        {
            OnApplicationQuit -= action;
        }
        internal void Initialize()
        {
            OnApplicationQuit += SaveSettings;
            PersistantGamePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Banana", "Krof") + "/";
            LoadSettings();
            UpdateResoultion();
            LoadSceneNow();
        }
        async Task UpdateResoultion()
        {
            await Task.Delay(20);
            SetResolution(Settings.ScreenWidth, Settings.ScreenHeight);
        }
        public static void SetResolution(int width, int height, bool? fullscreen = null)
        {
            if (fullscreen != null)
            {
                Game1.Graphics.IsFullScreen = (bool)fullscreen;
            }
            Game1.Graphics.PreferredBackBufferWidth = width;
            Game1.Graphics.PreferredBackBufferHeight = height;
            Game1.Graphics.ApplyChanges();
            Game1.ScreenWidth = width;
            Game1.ScreenHeight = height;
            Camera.Resize();
        }

        public static void SetFullscreen(bool fullscreen)
        {
            Game1.Graphics.IsFullScreen = fullscreen;
            Game1.Graphics.ApplyChanges();
        }
        private void LoadSettings()
        {
            List<Type> SaveClasses = assembly.GetTypes()
                .Where(t => t.IsClass && typeof(ISaveFile).IsAssignableFrom(t))
                .ToList();
            string fileName;
            if (!File.Exists(PersistantGamePath + "Settings.json"))
            {
                Debug.WriteLine("FIRST TIME BOOT");
                try
                {
                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Banana"));
                    Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Banana", "Krof"));
                    Settings = new();
                    string json = JsonConvert.SerializeObject(Settings);
                    File.WriteAllText(PersistantGamePath + "Settings.json", json);
                    foreach (Type item in SaveClasses)
                    {
                        fileName = $"{PersistantGamePath}{item.Name}.json";
                        saveFiles.Add((ISaveFile)Activator.CreateInstance(item));
                        json = JsonConvert.SerializeObject(saveFiles.Last());
                        File.WriteAllText(fileName, json);
                    }
                    Vector2 v = Game1.ScreenSize;
                    SetResolution((int)v.X, (int)v.Y, true);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
            else
            {
                Debug.WriteLine("Loading save file");
                try
                {
                    Settings = new();
                    JsonConvert.PopulateObject(File.ReadAllText(PersistantGamePath + "Settings.json"), Settings);
                    foreach (Type item in SaveClasses)
                    {
                        fileName = $"{PersistantGamePath}{item.Name}.json";
                        saveFiles.Add((ISaveFile)Activator.CreateInstance(item));
                        JsonConvert.PopulateObject(File.ReadAllText(fileName), saveFiles.Last());
                    }
                    SetResolution(Settings.ScreenWidth, Settings.ScreenHeight, Settings.Fullscreen);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
        }
        public static void SaveSettings()
        {
            string fileName;
            try
            {
                Settings.Fullscreen = Game1.Graphics.IsFullScreen;
                Settings.ScreenWidth = Game1.Graphics.PreferredBackBufferWidth;
                Settings.ScreenHeight = Game1.Graphics.PreferredBackBufferHeight;
                string json = JsonConvert.SerializeObject(Settings);
                File.WriteAllText(PersistantGamePath + "Settings.json", json);
                foreach (ISaveFile item in saveFiles)
                {
                    fileName = $"{PersistantGamePath}{item.GetType().Name}.json";
                    json = JsonConvert.SerializeObject(item);
                    File.WriteAllText(fileName, json);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
        public static void LoadScene(int index)
        {
            RequestedScene = scenes[index];
        }
        public static void ReloadScene()
        {
            LoadScene(LoadedScene.index);
        }
        private static void LoadSceneNow()
        {
            if (LoadedScene != null)
            {
                LoadedScene.OnDestroy();
                SoundEngine.Reset();
                GameObject.DestroyAllObjects();
                Physics.Setup();
                Renderer.drawableText.Clear();
            }
            PlayGame();
            IScene s = RequestedScene;
            RequestedScene = null;
            LoadedScene = s.GenerateScene();
        }
        public static Action OnPause;
        public static Action OnPlay;
        public static void PauseGame()
        {
            AIEngine.Active = false;
            Physics.Active = false;
            OnPause?.Invoke();
        }
        public static void PlayGame()
        {
            AIEngine.Active = true;
            Physics.Active = true;
            OnPlay?.Invoke();
        }


        public void Update(GameTime gameTime)
        {
            PreviousKeyboardState = KeyboardState;
            PreviousMousePosition = MousePosition;
            PreviousMouseState = MouseState;
            if (Game1.ScreenWidth != 0)
            {
                KeyboardState = Keyboard.GetState();
                MouseState = Mouse.GetState();
                MousePosition = new Vector2(MouseState.X * Game1.GameWidth / Game1.ScreenWidth, MouseState.Y * Game1.GameHeight / Game1.ScreenHeight);
            }
            foreach (var item in updateablesToRemove)
            {
                updateables.Remove(item);
            }
            updateablesToRemove.Clear();
            updateID = 0;
            if (sortRequested)
            {
                updateables.Sort();
                sortRequested = false;
            }
            do
            {
                try
                {
                    UpdateSequence(gameTime);
                }
                catch (Exception e)
                {
                    Debug.WriteLine($"ERROR: Update error ({e.Message})");
                    updateID++;
                }
            }
            while (updateID < updateables.Count);
            while (0 < objectToDestroy.Count)
            {
                objectToDestroy[0].Dest();
            }
            if (RequestedScene != null)
            {
                LoadSceneNow();
            }
        }
        private void UpdateSequence(GameTime gameTime)
        {
            while (updateID < updateables.Count)
            {
                updateables.ElementAt(updateID).Update(gameTime);
                updateID++;
            }
        }
    }
}

using KrofEngine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static Krof.Player;
using Microsoft.Xna.Framework.Input;

namespace Krof
{
    public enum GameMode
    {
        Normal, KillBoss
    }
    internal class PlaySceneManager : UpdatableGameObject
    {
        public static Action<bool> OnComplete;
        Game game1;
        bool[,] cells;
        public static GameMode gameMode;
        public static int Seed = 0;
        public static bool HasKey;
        public static PlaySceneManager Instance;
        List<WallGroup> wallGroups;
        List<WallGroup> updatableWallGroups;
        public static List<WallGroup> updatableWallGroupsToRemove;
        Wall[,] walls;
        static Random random;
        int waitTime = int.MaxValue;
        public static int SizeX;
        public static int SizeY;
        int x, y;
        int[] temp;
        public static DrawableItem hall;
        private static TimeOnly startTime;
        static Panel pausePanel;
        public static bool UpdateScene;
        Vector2 prevPausePos;
        //Text numOfObjectsRendered;
        public static void PauseOnly()
        {
            UpdateScene = false;
            GameManager.PauseGame();
            //player.Active = false;
        }
        public static void Pause()
        {
            PauseOnly();
            pausePanel.Active = true;
            GameManager.isMouseVisible = true;
            //Vector2 newPos = player.Transform.Position - new Vector2(160, 160);
            //pausePanel.Move(newPos - prevPausePos);
            //prevPausePos = newPos;
        }
        public static void PlayOnly()
        {
            UpdateScene = true;
            GameManager.PlayGame();
            //player.Active = true;
        }
        public static void Play()
        {
            PlayOnly();
            pausePanel.Active = false;
            GameManager.isMouseVisible = false;
        }
        public PlaySceneManager(Vector2 position = default, Vector2 scale = default, float angle = 0) : base(position, scale, angle)
        {
            GameManager.isMouseVisible = false;
            game1 = Game1.Instance;
            OnComplete = Complete;
            //GameManager.OnPause += delegate{ player.Active = false; };
            //GameManager.OnPlay += delegate { player.Active = true; };
            Instance = this;
            //ShowMaze(100, 100, seed == 0 ? (int)new Random().NextInt64() : seed);
            ShowMaze(Seed == 0 ? (int)new Random().NextInt64() : Seed);
            pausePanel = new Panel();
            prevPausePos = new Vector2(Game1.GameWidth / 2 - 230, Game1.GameHeight / 2 - 200);
            Color pausePanelColor = new Color(186, 186, 186, 100);
            //pausePanel.AddChild(new Image(Renderer.Sprites[1], prevPausePos, new Vector2(460, 300), pausePanelColor));
            pausePanel.AddChild(new Image(Renderer.Sprites[15], prevPausePos, Vector2.One));
            pausePanel.AddChild(new Text(new Vector2(Game1.GameWidth/2, Game1.GameHeight/2 - 160), "PAUSE MENU", Color.Black, TextAlignment.Middle));
            pausePanel.AddChild(new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, Game1.GameHeight / 2 - 70), Color.White, Color.Gray, "CONTINUE", Color.Black, TextAlignment.Middle, font: Renderer.Fonts[1], mouseUp: delegate
            {
                Play();
            }));
            pausePanel.AddChild(new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, Game1.GameHeight / 2 - 10), Color.White, Color.Gray, "RESTART", Color.Black, textAlignment: TextAlignment.Middle, font: Renderer.Fonts[1], mouseUp: delegate
            {
                Restart();
            }));
            pausePanel.AddChild(new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, Game1.GameHeight / 2 + 50), Color.White, Color.Gray, "EXIT", Color.Black, textAlignment: TextAlignment.Middle, font: Renderer.Fonts[1], mouseUp: delegate
            {
                GameManager.LoadScene(0);
            }));
            pausePanel.Active = false;
            //numOfObjectsRendered = new Text(Vector2.Zero, "", Color.White, font: Renderer.Fonts[1]);
            SoundEngine.PlaySound(2);
        }
        public static void Restart()
        {
            //Instance.ShowMaze(SizeX, SizeY, seed == 0 ? (int)new Random().NextInt64() : seed);
            GameManager.ReloadScene();
        }
        public int[] GetClosestFreePoint(int x, int y)
        {
            if (!cells[x, y]) return new int[] { x, y };
            x = Math.Max(x, 3);
            int[] ints = getFreeNeighbour(x, y);
            while (ints == null)
            {
                x++;
                x = (x % (SizeX - 2)) + 1;
                y++;
                y %= (SizeY - 1);
                if (!cells[x, y]) return new int[] { x, y };
                ints = getFreeNeighbour(x, y);
            }
            return ints;
        }
        public int[] GetRandomFreePoint()
        {
            x = 1 + random.Next(SizeX - 2);
            y = 1 + random.Next(SizeY - 2);
            temp = GetClosestFreePoint(x, y);
            cells[temp[0], temp[1]] = true;
            return temp;
        }
        public void ShowMaze(int seed)
        {
            updatableWallGroups = new();
            updatableWallGroupsToRemove = new();
            UpdateScene = true;
            Renderer.backgroundColor = Color.White;
            Stopwatch s = new Stopwatch();
            s.Start();
            Physics.Setup(SizeX * 100, SizeY * 100);
            //Prepare maze
            wallGroups = new List<WallGroup>((int)(SizeY * SizeX * 0.8f));
            Seed = seed;
            random = new Random(Seed);
            MazeGenerator mazeGenerator = new MazeGenerator(SizeX, SizeY, random);
            mazeGenerator.generate();
            cells = mazeGenerator.GetMaze();
            mazeGenerator = null;
            //Build walls
            walls = new Wall[SizeX, SizeY];
            walls[0, 0] = new Wall(game1, new Vector2(-1900, -1900), new Vector2(4000 + SizeX * 100, 2000));
            walls[1, 0] = new Wall(game1, new Vector2(-1900, SizeY * 100 - 100), new Vector2(4000 + SizeX * 100, 2000));
            walls[2, 0] = new Wall(game1, new Vector2(-1900, 100), new Vector2(2000, SizeY * 100));
            walls[3, 0] = new Wall(game1, new Vector2(SizeX * 100 - 100, 100), new Vector2(2000, SizeY * 100));
            for (int i = 1; i < SizeX - 1; i++)
            {
                for (int j = 1; j < SizeY - 1; j++)
                {
                    walls[i, j] = new Wall(game1, new Vector2(i * 100, j * 100), cells[i, j]);
                }
            }
            int fullSize = SizeX * SizeY;
            int counter = fullSize / 4;
            //Delete walls
            while (counter > 0)
            {
                x = 1 + random.Next(SizeX - 2);
                y = 1 + random.Next(SizeY - 2);
                walls[x, y].SetFull(false);
                cells[x, y] = false;
                counter--;
            }
            //Spawn movable objects
            if (gameMode == GameMode.Normal)
            {
                HasKey = false;
                x = random.Next(4);
                ImageObject hall1 = new ImageObject(Renderer.Sprites[1], Vector2.Zero, Vector2.Zero);
                hall = hall1.GetComponent<DrawableItem>();
                hall.Color = new Color(41, 41, 41);
                hall.Enabled = false;
                hall.GlobalDraw = true;
                if (x < 2)
                {
                    y = random.Next(SizeX - 2) + 1;
                    hall1.Transform.Scale = new Vector2(100, 2000);
                    cells[y, x == 0 ? 1 : SizeY - 2] = false;
                    walls[y, x == 0 ? 1 : SizeY - 2].SetFull(false);
                    cells[y, x == 0 ? 2 : SizeY - 3] = false;
                    walls[y, x == 0 ? 2 : SizeY - 3].SetFull(false);
                    hall1.Transform.Position = new Vector2(y * 100, x == 0 ? -1900 : SizeY * 100 - 100);
                    new Doors(game1, new Vector2(y * 100 + 50, x == 0 ? 50 : SizeY * 100 - 50), x == 0 ? 0 : (float)Math.PI);
                }
                else
                {
                    y = random.Next(SizeY - 2) + 1;
                    hall1.Transform.Scale = new Vector2(2000, 100);
                    cells[x == 2 ? 1 : SizeX - 2, y] = false;
                    walls[x == 2 ? 1 : SizeX - 2, y].SetFull(false);
                    cells[x == 2 ? 2 : SizeX - 3, y] = false;
                    walls[x == 2 ? 2 : SizeX - 3, y].SetFull(false);
                    hall1.Transform.Position = new Vector2(x == 2 ? -1900 : SizeX * 100 - 100, y * 100);
                    new Doors(game1, new Vector2(x == 2 ? 50 : SizeX * 100 - 50, y * 100 + 50), x == 2 ? -(float)Math.PI / 2 : (float)Math.PI / 2);
                }
                GetRandomFreePoint();
                new Key(game1, new Vector2(temp[0] * 100 + 75, temp[1] * 100 + 75));
                GetRandomFreePoint();
                new Player(game1, new Vector2(50 + temp[0] * 100, 50 + temp[1] * 100));
                counter = SizeX * SizeY / 500 * (random.Next(3) + 1) + 1;
                //counter = 1;
                while (counter > 0)
                {
                    GetRandomFreePoint();
                    new MonsterShoot(game1, new Vector2(50 + temp[0] * 100, 50 + temp[1] * 100));
                    GetRandomFreePoint();
                    new MonsterNormal(game1, new Vector2(50 + temp[0] * 100, 50 + temp[1] * 100));
                    counter--;
                }
                //counter = SizeX * SizeY / 1000 * (random.Next(3) + 1);
                //while (counter > 0)
                //{
                //    GetRandomFreePoint();
                //    movables.Add(new MonsterShoot(game1, new Vector2(50 + temp[0] * 100, 50 + temp[1] * 100)));
                //    counter--;
                //}
            }
            else
            {
                GetRandomFreePoint();
                new Player(game1, new Vector2(50 + temp[0] * 100, 50 + temp[1] * 100));
                GetRandomFreePoint();
                new MonsterBoss(game1, new Vector2(50 + temp[0] * 100, 50 + temp[1] * 100));
            }
            //Create wall groups
            WallGroup wallGroup;
            for (int i = 1; i < SizeX - 1; i++)
            {
                if (!cells[i, 1])
                {
                    wallGroup = new();
                    cells[i, 1] = true;
                    wallGroup.walls.Add(walls[i, 1]);
                    temp = getFreeNeighbour1st(i, 1);
                    if (temp != null)
                    {
                        wallGroup.walls.Add(walls[temp[0], temp[1]]);
                        cells[temp[0], temp[1]] = true;
                        temp = getFreeNeighbour1st(temp[0], temp[1]);
                        if (temp != null)
                        {
                            cells[temp[0], temp[1]] = true;
                            wallGroup.walls.Add(walls[temp[0], temp[1]]);
                        }
                    }
                    wallGroups.Add(wallGroup);
                }
                for (int j = 2; j < SizeY - 1; j++)
                {
                    if (!cells[i, j])
                    {
                        wallGroup = new();
                        cells[i, j] = true;
                        wallGroup.walls.Add(walls[i, j]);
                        temp = getFreeNeighbour(i, j);
                        if (temp != null)
                        {
                            wallGroup.walls.Add(walls[temp[0], temp[1]]);
                            cells[temp[0], temp[1]] = true;
                            temp = getFreeNeighbour(temp[0], temp[1]);
                            if (temp != null)
                            {
                                cells[temp[0], temp[1]] = true;
                                wallGroup.walls.Add(walls[temp[0], temp[1]]);
                            }
                        }
                        wallGroups.Add(wallGroup);
                    }
                }
            }
            //Finish and clean
            //waitTime = 50000/fullSize;
            waitTime = 500000 / fullSize;
            cells = null;
            //Task.Run(ShowWalls);
            s.Stop();
            //Debug.WriteLine(s.ElapsedMilliseconds.ToString());
            startTime = TimeOnly.FromDateTime(DateTime.Now);
        }
        int[] getFreeNeighbour1st(int X, int Y)
        {
            if (!cells[X + 1, Y]) return new int[] { X + 1, Y };
            if (!cells[X, Y + 1]) return new int[] { X, Y + 1 };
            return null;
        }
        int[] getFreeNeighbour(int X, int Y)
        {
            if (!cells[X - 1, Y]) return new int[] { X - 1, Y };
            if (!cells[X + 1, Y]) return new int[] { X + 1, Y };
            if (!cells[X, Y + 1]) return new int[] { X, Y + 1 };
            return null;
        }
        //async Task ShowWalls()
        //{
        //    while (true)
        //    {
        //        await Task.Delay(waitTime);
        //        wallGroups[random.Next(wallGroups.Count)].SetEnabled();
        //    }
        //}
        int currentTime = 0;

        public override void Update(GameTime gameTime)
        {
            //numOfObjectsRendered.text = "Objects rendered: " + Renderer.numOfObjectsRendered + "/" + GameObject.AllObjects.Count;
            if (GameManager.KeyboardState.IsKeyUp(Keys.Escape) && GameManager.PreviousKeyboardState.IsKeyDown(Keys.Escape))
            {
                if (UpdateScene)
                {
                    Pause();
                }
                else
                {
                    if (player == null)
                    {
                        GameManager.LoadScene(0);
                    }
                    else
                    {
                        Play();
                    }
                }
            }else if(GameManager.KeyboardState.IsKeyUp(Keys.Enter) && GameManager.PreviousKeyboardState.IsKeyDown(Keys.Enter))
            {
                if (player == null)
                {
                    Restart();
                }
            }
            if (UpdateScene)
            {
                currentTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                int idx;
                float change = (float)gameTime.ElapsedGameTime.TotalSeconds;
                foreach (var item in updatableWallGroups)
                {
                    item.Update(change);
                }
                foreach (var item in updatableWallGroupsToRemove)
                {
                    updatableWallGroups.Remove(item);
                }
                updatableWallGroupsToRemove.Clear();
                while (currentTime > waitTime)
                {
                    currentTime -= waitTime;
                    idx = random.Next(wallGroups.Count);
                    if (wallGroups[idx].state == 0)
                    {
                        updatableWallGroups.Add(wallGroups[idx].StartClosing());
                    }
                }
            }
        }
        public static void Complete(bool success)
        {
            GameManager.PauseGame();
            UpdateScene = false;
            GameManager.isMouseVisible = true;
            if (success)
            {
                SoundEngine.PlaySound(6);
                Leaderboard l = GameManager.GetSaveFile<Leaderboard>();
                TimeSpan t = TimeOnly.FromDateTime(DateTime.Now) - startTime;
                TimeOnly tt = new System.TimeOnly(t.Ticks);
                new Text(new Vector2(Game1.GameWidth / 2, Game1.GameHeight / 2), "Congratulations!", Color.White, TextAlignment.Middle);
                new Text(new Vector2(Game1.GameWidth / 2, Game1.GameHeight / 2 + 50), gameMode == GameMode.Normal? "You successfully escaped the maze in " + tt.ToLongTimeString() + ".":"You successfully killed the boss in " + tt.ToLongTimeString() + ".", Color.White, TextAlignment.Middle, Renderer.Fonts[1]);
                new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, Game1.GameHeight / 2 + 150), Color.White, Color.Gray, "Restart", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate { Restart(); });
                new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, Game1.GameHeight / 2 + 210), Color.White, Color.Gray, "Play new maze", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate { Seed = (int)new Random().NextInt64(); Restart(); });
                new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, Game1.GameHeight / 2 + 270), Color.White, Color.Gray, "Exit to main menu", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate { GameManager.LoadScene(0); });
                bool found = false;
                int i = 0;
                while (i < l.width.Count && SizeX > l.width[i]) {
                    i++;
                }
                while (i < l.height.Count && SizeY > l.height[i])
                {
                    i++;
                }
                while(i < l.height.Count && SizeX == l.width[i] && SizeY == l.height[i] && tt > l.time[i])
                {
                    if (l.username[i] == MenuManager.loggedUserName && l.gameMode[i] == gameMode)
                    {
                        found = true;
                        break;
                    }
                    i++;
                }
                if (!found)
                {
                    if (i < l.height.Count)
                    {
                        //insert in i
                        l.width.Insert(i, SizeX);
                        l.height.Insert(i, SizeY);
                        l.seed.Insert(i, Seed);
                        l.username.Insert(i, MenuManager.loggedUserName);
                        l.time.Insert(i, tt);
                        l.gameMode.Insert(i, gameMode);
                        i++;
                        while (i < l.height.Count && SizeX == l.width[i] && SizeY == l.height[i])
                        {
                            if (l.username[i] == MenuManager.loggedUserName && l.gameMode[i] == gameMode)
                            {
                                l.width.RemoveAt(i);
                                l.height.RemoveAt(i);
                                l.seed.RemoveAt(i);
                                l.username.RemoveAt(i);
                                l.time.RemoveAt(i);
                                l.gameMode.RemoveAt(i);
                                break;
                            }
                            i++;
                        }
                        APIManager.InsertData(MenuManager.loggedUserName, tt.ToTimeSpan(), gameMode == GameMode.Normal ? 0 : 1, SizeX, SizeY, Seed);
                    }
                    else
                    {
                        //add in i
                        l.width.Add(SizeX);
                        l.height.Add(SizeY);
                        l.seed.Add(Seed);
                        l.username.Add(MenuManager.loggedUserName);
                        l.time.Add(tt);
                        l.gameMode.Add(gameMode);
                        APIManager.InsertData(MenuManager.loggedUserName, tt.ToTimeSpan(), gameMode == GameMode.Normal ? 0 : 1, SizeX, SizeY, Seed);
                    }
                }
                //while (i < l.time.Count && tt > l.time[i])
                //{
                //    i++;
                //}
                //for (int i = 0; i < l.width.Count; i++)
                //{
                //    if (l.width[i] == SizeX && l.height[i] == SizeY && l.username[i] == MenuManager.loggedUserName && l.gameMode[i] == gameMode)
                //    {
                //        if (new System.TimeOnly(t.Ticks) < l.time[i])
                //        {
                //            l.time[i] = new System.TimeOnly(t.Ticks);
                //        }
                //        found = true;
                //        break;
                //    }
                //}
                //if (!found)
                //{
                //    l.width.Add(SizeX);
                //    l.height.Add(SizeY);
                //    l.seed.Add(runningSeed);
                //    l.username.Add(MenuManager.loggedUserName);
                //    l.time.Add(new System.TimeOnly(t.Ticks));
                //    l.gameMode.Add(gameMode);
                //}
            }
            else
            {
                new Text(new Vector2(Game1.GameWidth / 2, Game1.GameHeight / 2), "You died", Color.White, TextAlignment.Middle);
                new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, Game1.GameHeight / 2 + 150), Color.White, Color.Gray, "Restart", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate { Restart(); });
                new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, Game1.GameHeight / 2 + 210), Color.White, Color.Gray, "Play new maze", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate { Seed = (int)new Random().NextInt64(); Restart(); });
                new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, Game1.GameHeight / 2 + 270), Color.White, Color.Gray, "Exit to main menu", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate { GameManager.LoadScene(0); });
                SoundEngine.PlaySound(7);
            }
            //GameManager.LoadScene(0);
        }
    }
}

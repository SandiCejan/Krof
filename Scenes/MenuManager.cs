using Krof.Scenes;
using Krof.Scripts;
using KrofEngine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Krof
{
    internal class MenuManager : GameObject
    {
        InputField SeedInput, WidthInput, HeightInput, NameInput;
        Text NameInputErrorText, LoggedUserText;
        private static Panel openPanel;
        private GameMode gameMode = GameMode.Normal;
        public static string loggedUserName = "";
        bool loadedLeaderborad, loadedResolutions;
        ScrollablePanel leaderboardPanel1;
        ScrollablePanel leaderboardPanel2;
        ScrollablePanel leaderboardPanel3;
        ScrollablePanel leaderboardPanel4;
        ScrollablePanel resolutionPanel;
        Button resolutionB, selectedResolution;
        Button[] resolutionButtons;
        internal MenuManager() : base()
        {
            SoundEngine.PlaySound(1);
            Button.OnClickAction = delegate (Button b) { SoundEngine.PlaySound(0); };
            GameManager.isMouseVisible = true;
            Renderer.backgroundColor = new Color(64, 64, 64);
            Panel mainPanel = new();
            Panel playPanel = new();
            Panel playStoryPanel = new();
            Panel playCustomPanel = new();
            playPanel.AddChild(playStoryPanel);
            playPanel.AddChild(playCustomPanel);
            Panel leaderboardPanel = new();
            leaderboardPanel1 = new(Game1.GameWidth / 2 - 200, 420, 1000, 350, 50);
            leaderboardPanel2 = new(Game1.GameWidth / 2 - 200, 420, 1000, 350, 50);
            leaderboardPanel3 = new(Game1.GameWidth / 2 - 200, 420, 1000, 350, 50);
            leaderboardPanel4 = new(Game1.GameWidth / 2 - 200, 420, 1000, 350, 50);
            resolutionPanel = new(Game1.GameWidth / 2 - 200, Game1.GameHeight / 2 - 300, 400, 550, 60);
            Panel leaderboardLocalPanel = new();
            Panel leaderboardGlobalPanel = new();
            leaderboardLocalPanel.AddChild(leaderboardPanel1);
            leaderboardLocalPanel.AddChild(leaderboardPanel2);
            leaderboardPanel.AddChild(leaderboardLocalPanel);
            leaderboardGlobalPanel.AddChild(leaderboardPanel3);
            leaderboardGlobalPanel.AddChild(leaderboardPanel4);
            leaderboardPanel.AddChild(leaderboardGlobalPanel);
            Panel creditsPanel = new();
            Panel settingsPanel = new();
            Panel namePanel = new();
            //new Networking();
            if (loggedUserName == "")
            {
                SetMusicVolume(GameManager.Settings.MusicVolume);
                SetEffectsVolume(GameManager.Settings.EffectsVolume);
                loggedUserName = GameManager.Settings.LastUserName;
                SoundEngine.SoundEffectsInstances[1].IsLooped = true;
                SoundEngine.SoundEffectsInstances[2].IsLooped = true;
                SoundEngine.SoundEffectsInstances[3].IsLooped = true;
            }
            //new Image(Renderer.Sprites[12], Vector2.Zero, new Vector2(0.89285714285714285714285714285714f, 0.87890625f));
            new Image(Renderer.Sprites[12], Vector2.Zero, new Vector2(1.0714285714285714285714285714286f, 1.1f));

            #region mainPanel
            mainPanel.AddChild(new Text(new Vector2(Game1.GameWidth / 2, 200), "MENU", Color.Black, TextAlignment.Middle));
            mainPanel.AddChild(new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, Game1.GameHeight / 2 - 120), Color.White, Color.Gray, "Start game", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate { ShowMenu(playPanel); }));
            mainPanel.AddChild(new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, Game1.GameHeight / 2 - 60), Color.White, Color.Gray, "Leaderboard", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate
            {
                ShowMenu(leaderboardPanel);
                if (!loadedLeaderborad)
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    PopulateLeaderboard();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
            }));
            mainPanel.AddChild(new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, Game1.GameHeight / 2), Color.White, Color.Gray, "Settings", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate { ShowMenu(settingsPanel); }));
            mainPanel.AddChild(new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, Game1.GameHeight / 2 + 60), Color.White, Color.Gray, "Credits", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate { ShowMenu(creditsPanel); }));
            mainPanel.AddChild(new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, Game1.GameHeight / 2 + 120), Color.White, Color.Gray, "Exit", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate { Game1.Instance.Exit(); }));
            mainPanel.AddChild(new Button(Renderer.Sprites[8], new Vector2(Game1.GameWidth - 25, 25), new Vector2(.1f, .1f), Color.White, Color.Gray, textColor: Color.Black, textAlignment: TextAlignment.Middle, font: Renderer.Fonts[1], mouseUp: delegate { ShowMenu(namePanel); }));
            LoggedUserText = new Text(new Vector2(Game1.GameWidth - 50, 50), loggedUserName, Color.White, textAlignment: TextAlignment.Right, font: Renderer.Fonts[0]);
            mainPanel.AddChild(LoggedUserText);
            #endregion


            #region playPanel
            playPanel.AddChild(new Text(new Vector2(Game1.GameWidth / 2, 200), "PLAY", Color.Black, TextAlignment.Middle));
            Button EscapeB = new();
            Button KillB = new();
            Button StoryB = new();
            StoryB = new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2 - 250, 300), new Vector2(.6f, 1), new Color(145, 145, 145), new Color(133, 133, 133), "Story", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate
            {
                StoryB.backgroundColor = new Color(145, 145, 145);
                StoryB.hoverColor = new Color(133, 133, 133);
                KillB.backgroundColor = Color.White;
                KillB.hoverColor = Color.Gray;
                EscapeB.backgroundColor = Color.White;
                EscapeB.hoverColor = Color.Gray;
                playStoryPanel.Active = true;
                playCustomPanel.Active = false;
            });
            playPanel.AddChild(StoryB);
            EscapeB = new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, 300), new Vector2(.6f, 1), Color.White, Color.Gray, "Escape the maze", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate
            {
                EscapeB.backgroundColor = new Color(145, 145, 145);
                EscapeB.hoverColor = new Color(133, 133, 133);
                KillB.backgroundColor = Color.White;
                KillB.hoverColor = Color.Gray;
                StoryB.backgroundColor = Color.White;
                StoryB.hoverColor = Color.Gray;
                gameMode = GameMode.Normal;
                playStoryPanel.Active = false;
                playCustomPanel.Active = true;
            });
            playPanel.AddChild(EscapeB);
            KillB = new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2 + 250, 300), new Vector2(.6f, 1), Color.White, Color.Gray, "Kill the boss", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate
            {
                KillB.backgroundColor = new Color(145, 145, 145);
                KillB.hoverColor = new Color(133, 133, 133);
                EscapeB.backgroundColor = Color.White;
                EscapeB.hoverColor = Color.Gray;
                StoryB.backgroundColor = Color.White;
                StoryB.hoverColor = Color.Gray;
                gameMode = GameMode.KillBoss;
                playStoryPanel.Active = false;
                playCustomPanel.Active = true;
            });
            playPanel.AddChild(KillB);
            playStoryPanel.AddChild(new Text(new Vector2(Game1.GameWidth / 2, 400), GameManager.Settings.StoryCompleted ? "Story completed" : "Story not yet completed", Color.White, TextAlignment.Middle));
            Text storyExists = null;
            if (GameManager.Settings.StoryState != 0)
            {
                playStoryPanel.AddChild(new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, 520), Color.White, Color.Gray, "Continue", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate
                {
                    StoryManager.State = GameManager.Settings.StoryState;
                    GameManager.LoadScene(2);
                }));
                storyExists = new Text(new Vector2(Game1.GameWidth / 2, 580), $"Current level: {GameManager.Settings.StoryState}/7", Color.White, TextAlignment.Middle, Renderer.Fonts[1]);
                playStoryPanel.AddChild(storyExists);
            }
            Button storyNewB = new();
            storyNewB = new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, 460), Color.White, Color.Gray, "Start new", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate
            {
                if (GameManager.Settings.StoryState != 0)
                {
                    if (storyExists && storyExists.color == Color.Red)
                    {
                        StoryManager.State = 0;
                        GameManager.Settings.StoryState = 0;
                        GameManager.LoadScene(2);
                    }
                    else
                    {
                        if (storyExists)
                        {
                            storyExists.color = Color.Red;
                            storyExists.text = "Story exists. Are you sure you wish to begin from the start? This will overwrite the current save data.";
                        }
                        else
                        {
                            storyExists = new Text(new Vector2(Game1.GameWidth / 2, 580), "Story exists. Are you sure you wish to begin from the start? This will overwrite the current save data.", Color.Red, TextAlignment.Middle, Renderer.Fonts[1]);
                        }
                        storyNewB.GetComponent<DrawableText>().Text = "Confirm";
                        storyNewB.backgroundColor = new Color(255, 94, 94);
                        storyNewB.hoverColor = Color.Red;
                    }
                }
                else
                {
                    StoryManager.State = 0;
                    GameManager.LoadScene(2);
                }
            });
            playStoryPanel.AddChild(storyNewB);

            SeedInput = new InputField(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, 400), Color.White, new InputFieldRules(true, false, false, false), 9, "Seed", "", Color.Black, TextAlignment.Middle, Renderer.Fonts[2], new Color(209, 209, 209), Color.Gray);
            playCustomPanel.AddChild(SeedInput);
            playCustomPanel.AddChild(new Text(new Vector2(Game1.GameWidth / 2 - 210, 412), "Seed", Color.White, TextAlignment.Right, Renderer.Fonts[1]));
            WidthInput = new InputField(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, 460), Color.White, new InputFieldRules(true, false, false, false), 3, "Width", "40", Color.Black, TextAlignment.Middle, Renderer.Fonts[2], new Color(209, 209, 209), Color.Gray);
            playCustomPanel.AddChild(WidthInput);
            playCustomPanel.AddChild(new Text(new Vector2(Game1.GameWidth / 2 - 210, 472), "Width", Color.White, TextAlignment.Right, Renderer.Fonts[1]));
            HeightInput = new InputField(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, 520), Color.White, new InputFieldRules(true, false, false, false), 3, "Height", "40", Color.Black, TextAlignment.Middle, Renderer.Fonts[2], new Color(209, 209, 209), Color.Gray);
            playCustomPanel.AddChild(HeightInput);
            playCustomPanel.AddChild(new Text(new Vector2(Game1.GameWidth / 2 - 210, 532), "Height", Color.White, TextAlignment.Right, Renderer.Fonts[1]));
            playCustomPanel.AddChild(new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, 580), Color.White, Color.Gray, "Start", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate
            {
                if (SeedInput.Text != "")
                {
                    PlaySceneManager.Seed = int.Parse(SeedInput.Text);
                }
                else
                {
                    PlaySceneManager.Seed = Math.Abs((int)new Random().NextInt64());
                }
                if (int.TryParse(WidthInput.Text, out int num))
                {
                    PlaySceneManager.SizeX = num > 25 ? num : 25;
                }
                if (int.TryParse(HeightInput.Text, out num))
                {
                    PlaySceneManager.SizeY = num > 25 ? num : 25;
                }
                PlaySceneManager.gameMode = gameMode;
                GameManager.LoadScene(1);
            }));
            playPanel.AddChild(new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, Game1.GameHeight - 100), Color.White, Color.Gray, "Back", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate { ShowMenu(mainPanel); }));
            #endregion


            #region settingsPanel
            settingsPanel.AddChild(new Text(new Vector2(Game1.GameWidth / 2, 200), "SETTINGS", Color.Black, TextAlignment.Middle));
            settingsPanel.AddChild(new Image(Renderer.Sprites[14], new Vector2(Game1.GameWidth / 2 - 350, 280), Vector2.One));
            //settingsPanel.AddChild(new Image(Renderer.Sprites[1], new Vector2(Game1.GameWidth / 2 - 350, 280), new Vector2(700, 210)));
            settingsPanel.AddChild(new Text(new Vector2(Game1.GameWidth / 2 - 250, 300), "Music Audio", Color.Black, font: Renderer.Fonts[1]));
            Text t = new Text(new Vector2(Game1.GameWidth / 2 + 200, 300), GameManager.Settings.MusicVolume.ToString(), Color.Black, font: Renderer.Fonts[1]);
            settingsPanel.AddChild(t);
            settingsPanel.AddChild(new Slider(new Vector2(Game1.GameWidth / 2 - 50, 310), 200, 20, GameManager.Settings.MusicVolume, delegate (float f)
            {
                t.text = f.ToString();
            }, onEndDragging: delegate (float f)
            {
                GameManager.Settings.MusicVolume = f;
                SetMusicVolume(f);
            }));
            settingsPanel.AddChild(new Text(new Vector2(Game1.GameWidth / 2 - 250, 340), "Effects Audio", Color.Black, font: Renderer.Fonts[1]));
            Text t2 = new Text(new Vector2(Game1.GameWidth / 2 + 200, 340), GameManager.Settings.EffectsVolume.ToString(), Color.Black, font: Renderer.Fonts[1]);
            settingsPanel.AddChild(t2);
            settingsPanel.AddChild(new Slider(new Vector2(Game1.GameWidth / 2 - 50, 350), 200, 20, GameManager.Settings.EffectsVolume, delegate (float f)
            {
                t2.text = f.ToString();
            }, onEndDragging: delegate (float f)
            {
                GameManager.Settings.EffectsVolume = f;
                SetEffectsVolume(f);
            }));
            Button fullscreenB = null;
            fullscreenB = new Button(Renderer.Sprites[11], new Vector2(Game1.GameWidth / 2 - 230, 393), Color.White, Color.DarkGray, mouseUp: delegate
            {
                GameManager.SetFullscreen(!Game1.Graphics.IsFullScreen);
                if (Game1.Graphics.IsFullScreen)
                    fullscreenB.backgroundColor = Color.Gray;
                else
                    fullscreenB.backgroundColor = Color.White;
            });
            settingsPanel.AddChild(fullscreenB);
            settingsPanel.AddChild(new Text(new Vector2(Game1.GameWidth / 2 - 200, 380), "Fullscreen", Color.Black, font: Renderer.Fonts[1]));
            Button resolutionCloseB = new();
            resolutionCloseB = new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, Game1.GameHeight - 100), Color.White, Color.Gray, "Back", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate
            {
                resolutionPanel.Active = false;
                settingsPanel.Active = true;
                resolutionCloseB.Active = false;
            });
            resolutionCloseB.Active = false;
            resolutionB = new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, 440), Color.White, Color.Gray, "", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate
            {
                resolutionPanel.Active = true;
                settingsPanel.Active = false;
                resolutionCloseB.Active = true;
                if (!loadedResolutions)
                {
                    int posY = 0;
                    resolutionButtons = new Button[Game1.supportedDisplayModes.Count];
                    foreach (var item in Game1.supportedDisplayModes)
                    {
                        Button b = null;
                        b = new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, posY * 60), Color.White, Color.Gray, $"{item.Width}X{item.Height}", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], delegate
                        {
                            GameManager.SetResolution(item.Width, item.Height);
                            SetResolutionText();
                            if (selectedResolution)
                            {
                                selectedResolution.backgroundColor = Color.White;
                                selectedResolution.hoverColor = Color.Gray;
                            }
                            selectedResolution = b;
                            b.backgroundColor = new Color(145, 145, 145);
                            b.hoverColor = new Color(133, 133, 133);
                        });
                        resolutionPanel.AddChild(b);
                        if (item.Width == Game1.ScreenWidth && item.Height == Game1.ScreenHeight)
                        {
                            selectedResolution = b;
                            b.backgroundColor = new Color(145, 145, 145);
                            b.hoverColor = new Color(133, 133, 133);
                        }
                        resolutionButtons[posY] = b;
                        posY++;
                    }
                    loadedResolutions = true;
                }
            });
            SetResolutionText();
            settingsPanel.AddChild(resolutionB);
            GameManager.OnResize += ResolutionChanged;
            settingsPanel.AddChild(new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, Game1.GameHeight - 100), Color.White, Color.Gray, "Back", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate { ShowMenu(mainPanel); }));
            #endregion


            #region namePanel
            namePanel.AddChild(new Text(new Vector2(Game1.GameWidth / 2, Game1.GameHeight / 2 - 100), "Who is playing?", Color.Black, textAlignment: TextAlignment.Middle, font: Renderer.Fonts[0]));
            NameInput = new InputField(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, Game1.GameHeight / 2), Color.White, new(), 15, "Enter your name", loggedUserName, Color.Black, TextAlignment.Middle, Renderer.Fonts[2], new Color(209, 209, 209), Color.Gray);
            namePanel.AddChild(NameInput);
            NameInputErrorText = new Text(new Vector2(Game1.GameWidth / 2, Game1.GameHeight / 2 + 50), "", Color.Red, TextAlignment.Middle, Renderer.Fonts[1]);
            namePanel.AddChild(NameInputErrorText);
            namePanel.AddChild(new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, Game1.GameHeight / 2 + 100), Color.White, Color.Gray, "Confirm", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate
            {
                if (NameInput.Text.Length < 3)
                {
                    NameInputErrorText.text = "Please, enter the name that is at least 3 characters long";
                }
                else
                {
                    ShowMenu(mainPanel);
                    loggedUserName = NameInput.Text;
                    NameInputErrorText.text = "";
                    LoggedUserText.text = loggedUserName;
                    GameManager.Settings.LastUserName = loggedUserName;
                }
            }));
            #endregion


            #region leaderboardPanel
            leaderboardPanel.AddChild(new Text(new Vector2(Game1.GameWidth / 2, 200), "LEADERBOARD", Color.Black, TextAlignment.Middle));
            leaderboardPanel.AddChild(new Image(Renderer.Sprites[13], new Vector2(Game1.GameWidth / 2 - 400, 400), Vector2.One));
            Button LeaderLocalB = new();
            Button LeaderGlobalB = new();

            LeaderLocalB = new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2 - 200, 300), new Vector2(.7f, 1), new Color(145, 145, 145), new Color(133, 133, 133), "Local", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate
            {
                LeaderLocalB.backgroundColor = new Color(145, 145, 145);
                LeaderLocalB.hoverColor = new Color(133, 133, 133);
                LeaderGlobalB.backgroundColor = Color.White;
                LeaderGlobalB.hoverColor = Color.Gray;
                leaderboardLocalPanel.Active = true;
                leaderboardGlobalPanel.Active = false;
            });
            LeaderGlobalB = new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2 + 200, 300), new Vector2(.7f, 1), Color.White, Color.Gray, "Global", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate
            {
                LeaderGlobalB.backgroundColor = new Color(145, 145, 145);
                LeaderGlobalB.hoverColor = new Color(133, 133, 133);
                LeaderLocalB.backgroundColor = Color.White;
                LeaderLocalB.hoverColor = Color.Gray;
                leaderboardGlobalPanel.Active = true;
                leaderboardLocalPanel.Active = false;
            });
            leaderboardPanel.AddChild(LeaderLocalB);
            leaderboardPanel.AddChild(LeaderGlobalB);

            Button EscapeB2 = new();
            Button KillB2 = new();
            EscapeB2 = new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2 - 200, 360), new Vector2(.7f, 1), new Color(145, 145, 145), new Color(133, 133, 133), "Escape the maze", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate
            {
                EscapeB2.backgroundColor = new Color(145, 145, 145);
                EscapeB2.hoverColor = new Color(133, 133, 133);
                KillB2.backgroundColor = Color.White;
                KillB2.hoverColor = Color.Gray;
                leaderboardPanel1.Active = true;
                leaderboardPanel2.Active = false;
            });
            leaderboardLocalPanel.AddChild(EscapeB2);
            KillB2 = new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2 + 200, 360), new Vector2(.7f, 1), Color.White, Color.Gray, "Kill the boss", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate
            {
                KillB2.backgroundColor = new Color(145, 145, 145);
                KillB2.hoverColor = new Color(133, 133, 133);
                EscapeB2.backgroundColor = Color.White;
                EscapeB2.hoverColor = Color.Gray;
                leaderboardPanel1.Active = false;
                leaderboardPanel2.Active = true;
            });
            leaderboardLocalPanel.AddChild(KillB2);

            Button EscapeB3 = new();
            Button KillB3 = new();
            EscapeB3 = new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2 - 200, 360), new Vector2(.7f, 1), new Color(145, 145, 145), new Color(133, 133, 133), "Escape the maze", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate
            {
                EscapeB3.backgroundColor = new Color(145, 145, 145);
                EscapeB3.hoverColor = new Color(133, 133, 133);
                KillB3.backgroundColor = Color.White;
                KillB3.hoverColor = Color.Gray;
                leaderboardPanel3.Active = true;
                leaderboardPanel4.Active = false;
            });
            leaderboardGlobalPanel.AddChild(EscapeB3);
            KillB3 = new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2 + 200, 360), new Vector2(.7f, 1), Color.White, Color.Gray, "Kill the boss", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate
            {
                KillB3.backgroundColor = new Color(145, 145, 145);
                KillB3.hoverColor = new Color(133, 133, 133);
                EscapeB3.backgroundColor = Color.White;
                EscapeB3.hoverColor = Color.Gray;
                leaderboardPanel3.Active = false;
                leaderboardPanel4.Active = true;
            });
            leaderboardGlobalPanel.AddChild(KillB3);
            leaderboardPanel.AddChild(new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, Game1.GameHeight - 100), Color.White, Color.Gray, "Back", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate { ShowMenu(mainPanel); }));
            #endregion


            #region creditsPanel
            creditsPanel.AddChild(new Text(new Vector2(Game1.GameWidth / 2, 200), "CREDITS", Color.Black, TextAlignment.Middle));
            creditsPanel.AddChild(new Image(Renderer.Sprites[16], new Vector2(Game1.GameWidth / 2 - 500, Game1.GameHeight / 2 - 300), Vector2.One));
            creditsPanel.AddChild(new Text(new Vector2(Game1.GameWidth / 2, Game1.GameHeight / 2 - 250), "Author: Sandi Cejan", Color.Black, TextAlignment.Middle));
            creditsPanel.AddChild(new Text(new Vector2(Game1.GameWidth / 2, Game1.GameHeight / 2 - 200), "SOUNDS", Color.Black, TextAlignment.Middle));
            creditsPanel.AddChild(new Text(new Vector2(Game1.GameWidth / 2, Game1.GameHeight / 2), "Bomb - Small by Zangrutz -- https://freesound.org/s/155235/\r\nGas Leak by templeofhades -- https://freesound.org/s/233405/\r\nGunfire_02 by Artmasterrich -- https://freesound.org/s/345410/\r\nHeavy Mechancial Door Open by lowpolygon -- https://freesound.org/s/421722/\r\nnegative_beeps.wav by themusicalnomad -- https://freesound.org/s/253886/\r\nTada Fanfare A by plasterbrain -- https://freesound.org/s/397355/\r\nGame Pickup by IENBA -- https://freesound.org/s/698768/\r\nRunning on ground by Disagree -- https://freesound.org/s/433725/\r\nSoaring Clouds by bruce965 -- https://freesound.org/s/464108/\r\nMedium Wind by kangaroovindaloo -- https://freesound.org/s/205966/\r\nSession Beat by Lit1onion -- https://freesound.org/s/784081/\r\nNormal click by Breviceps -- https://freesound.org/s/448086/", Color.Black, TextAlignment.Middle, Renderer.Fonts[1]));
            creditsPanel.AddChild(new Button(Renderer.Sprites[7], new Vector2(Game1.GameWidth / 2, Game1.GameHeight - 100), Color.White, Color.Gray, "Back", Color.Black, TextAlignment.Middle, Renderer.Fonts[1], mouseUp: delegate { ShowMenu(mainPanel); }));
            #endregion

            //OTHER
            resolutionPanel.Active = false;
            playCustomPanel.Active = false;
            playPanel.Active = false;
            settingsPanel.Active = false;
            leaderboardPanel4.Active = false;
            leaderboardPanel2.Active = false;
            leaderboardGlobalPanel.Active = false;
            leaderboardPanel.Active = false;
            creditsPanel.Active = false;
            if (loggedUserName == "")
            {
                mainPanel.Active = false;
                openPanel = namePanel;
            }
            else
            {
                namePanel.Active = false;
                openPanel = mainPanel;
            }
        }
        void ResolutionChanged()
        {
            SetResolutionText();
            if (resolutionButtons != null)
            {
                string resText = $"{Game1.ScreenWidth}X{Game1.ScreenHeight}";
                if (selectedResolution)
                {
                    selectedResolution.backgroundColor = Color.White;
                    selectedResolution.hoverColor = Color.Gray;
                }
                selectedResolution = null;
                foreach (var item in resolutionButtons)
                {
                    if (item.GetComponent<DrawableText>().Text == resText)
                    {
                        selectedResolution = item;
                        item.backgroundColor = new Color(145, 145, 145);
                        item.hoverColor = new Color(133, 133, 133);
                    }
                }
            }
        }
        public void SetResolutionText()
        {
            resolutionB.GetComponent<DrawableText>().Text = "Resolution: " + Game1.ScreenWidth + "X" + Game1.ScreenHeight;
        }
        public static void SetMusicVolume(float volume)
        {
            SoundEngine.SoundEffectsInstances[1].Volume = GameManager.Settings.MusicVolume;
            SoundEngine.SoundEffectsInstances[2].Volume = GameManager.Settings.MusicVolume;
        }
        public static void SetEffectsVolume(float volume)
        {
            SoundEngine.SoundEffectsInstances[0].Volume = GameManager.Settings.EffectsVolume;
        }
        public static void ShowMenu(Panel menu)
        {
            openPanel.Active = false;
            menu.Active = true;
            openPanel = menu;
        }
        async Task PopulateLeaderboard()
        {
            Leaderboard l = GameManager.GetSaveFile<Leaderboard>();
            int pos1 = 0, pos2 = 0;
            for (int i = 0; i < l.width.Count; i++)
            {
                if (l.gameMode[i] == GameMode.Normal)
                {
                    leaderboardPanel1.AddChild(new Text(new Vector2(Game1.GameWidth / 2, pos1 * 50), $"Size: {l.width[i]}X{l.height[i]}  Seed: {l.seed[i]}  Time: {l.time[i].ToLongTimeString()}  Player: {l.username[i]}", Color.Black, TextAlignment.Middle, Renderer.Fonts[1]));
                    pos1++;
                }
                else
                {
                    leaderboardPanel2.AddChild(new Text(new Vector2(Game1.GameWidth / 2, pos2 * 50), $"Size: {l.width[i]}X{l.height[i]}  Seed: {l.seed[i]}  Time: {l.time[i].ToLongTimeString()}  Player: {l.username[i]}", Color.Black, TextAlignment.Middle, Renderer.Fonts[1]));
                    pos2++;
                }
            }
            loadedLeaderborad = true;
            List<SaveData> list = await APIManager.GetLeaderboard();
            pos1 = 0;
            pos2 = 0;
            try
            {
                foreach (var item in list)
                {
                    if (item.gameMode == GameMode.Normal)
                    {
                        leaderboardPanel3.AddChild(new Text(new Vector2(Game1.GameWidth / 2, pos1 * 50), $"Size: {item.width}X{item.height}  Seed: {item.seed}  Time: {item.time.ToLongTimeString()}  Player: {item.username}", Color.Black, TextAlignment.Middle, Renderer.Fonts[1]));
                        pos1++;
                    }
                    else
                    {
                        leaderboardPanel4.AddChild(new Text(new Vector2(Game1.GameWidth / 2, pos2 * 50), $"Size: {item.width}X{item.height}  Seed: {item.seed}  Time: {item.time.ToLongTimeString()}  Player: {item.username}", Color.Black, TextAlignment.Middle, Renderer.Fonts[1]));
                        pos2++;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
        internal override void OnDestroy()
        {
            GameManager.OnResize -= ResolutionChanged;
            base.OnDestroy();
        }
    }
}

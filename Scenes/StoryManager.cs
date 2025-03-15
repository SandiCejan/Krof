using KrofEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Krof.Scenes
{
    internal class StoryManager : UpdatableGameObject
    {
        public static int State = 0;
        private Panel PlayerPanel;
        public static Text PlayerText;
        private int TalkID;
        private static StoryManager instance;
        private static string[][] Conversations =
            {
                new string[]
                {
                    "Press ENTER to continue", "Aaaah, my head...", "Wait, where am I?", "What is this place?", "I guess I should look around.", "Oh, a keycard.", "And a doors. Lets try to unlock them."
                },new string[]
                {
                    "Hmmm, I guess I need to do this again.", "Wait, I heard something. Something scary...", "Ok, lets quickly find those doors and run away"
                },new string[]
                {
                    "I wonder, why am I here...", "How I even got here...", "And why, for all that's wholy, can't I remember anything..."
                },new string[]
                {
                    "These levels are getting harder and bigger...", "When does it end?", "Am I even going to be able to get away from here?", "What is the purpose behind all of this?"
                },new string[]
                {
                    "Uuuu, a note.", "'You are doing great, but the hardest part is yet to come'", "What does that mean?", "I don't like this...", "I don't like this at all..."
                },new string[]
                {
                    "This place is getting creepier...", "I feel like I'm being watched.", "I need to stay focused and find a way out."
                },
                new string[]
                {
                    "I can't believe I've made it this far.", "I must be getting close to the end.", "Just a little more and I'll be free."
                },new string[]
                {
                    "Another note", "'Get it by taking it'", "What does that even mean?", "Wait, is he dead?", "Yes, he is dead!", "Wuuuuh, let's go!", "Soooo, ammm, what now?", "PART 1: COMPLETED"
                }
            };
        public void SaveState()
        {
            if (State == 8)
            {
                GameManager.Settings.StoryCompleted = true;
                GameManager.Settings.StoryState = 0;
            }
            else
            {
                GameManager.Settings.StoryState = State;
            }
        }
        public StoryManager()
        {
            instance = this;
            PlaySceneManager.gameMode = GameMode.Normal;
            switch (State)
            {
                case 0:
                    PlaySceneManager.Seed = 69;
                    PlaySceneManager.SizeX = 15;
                    PlaySceneManager.SizeY = 15;
                    break;
                case 1:
                    PlaySceneManager.Seed = 911;
                    PlaySceneManager.SizeX = 25;
                    PlaySceneManager.SizeY = 25;
                    break;
                case 2:
                    PlaySceneManager.Seed = 422;
                    PlaySceneManager.SizeX = 30;
                    PlaySceneManager.SizeY = 30;
                    break;
                case 3:
                    PlaySceneManager.Seed = 1234;
                    PlaySceneManager.SizeX = 40;
                    PlaySceneManager.SizeY = 40;
                    break;
                case 4:
                    PlaySceneManager.Seed = 8379265;
                    PlaySceneManager.SizeX = 50;
                    PlaySceneManager.SizeY = 50;
                    break;
                case 5:
                    PlaySceneManager.Seed = 6969;
                    PlaySceneManager.SizeX = 60;
                    PlaySceneManager.SizeY = 60;
                    break;
                case 6:
                    PlaySceneManager.Seed = 212121;
                    PlaySceneManager.SizeX = 80;
                    PlaySceneManager.SizeY = 80;
                    break;
                case 7:
                    PlaySceneManager.gameMode = GameMode.KillBoss;
                    PlaySceneManager.Seed = 10101;
                    PlaySceneManager.SizeX = 30;
                    PlaySceneManager.SizeY = 30;
                    break;
                default:
                    break;
            }
        }
        public void OnGenerated()
        {
            TalkID = 0;
            PlayerText = new Text(new Vector2(Game1.GameWidth / 2, Game1.GameHeight / 2), "", Color.Black, TextAlignment.Middle);
            PlayerPanel = new Panel(new List<GameObject> { PlayerText, new Image(Renderer.Sprites[1], new Vector2(Game1.GameWidth / 2 - 500, Game1.GameHeight / 2 - 35), new Vector2(1000, 70), Color.LightGray) });
            PlayerPanel.Active = false;
            PlaySceneManager.OnComplete = OnFinish;
            progress();
            switch (State)
            {
                case 0:
                    foreach (var item in AIEngine.Actors)
                    {
                        item.Destroy();
                    }
                    new EventCaller(Key.Instance.Transform, new Vector2(20, 20), new Vector2(-35, -35));
                    new EventCaller(Doors.instance.Transform, new Vector2(20, 20), new Vector2(-10, 90));
                    new TutorialObject();
                    break;
                case 1:
                    new EventCaller(Key.Instance.Transform, new Vector2(20, 20), new Vector2(-35, -35));
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                case 7:
                    break;
                default:
                    break;
            }
        }
        public void OnFinish(bool success)
        {
            if (success)
            {
                if (State == 7)
                {
                    progress();
                }
                else
                {
                    State++;
                    GameManager.LoadScene(2);
                }
            }
            else
            {
                GameManager.LoadScene(2);
            }
        }
        public static bool Progress()
        {
            return instance.progress();
        }
        private bool progress()
        {
            if (State == 0 && TalkID == 4 && !PlaySceneManager.HasKey)
            {
                return false;
            }
            PlaySceneManager.PauseOnly();
            Enabled = true;
            PlaySceneManager.Instance.Enabled = false;
            PlayerText.text = Conversations[State][TalkID];
            PlayerPanel.Active = true;
            TalkID++;
            return true;
        }
        private void DisableTalk()
        {
            Enabled = false;
            PlayerPanel.Active = false;
            PlaySceneManager.PlayOnly();
            PlaySceneManager.Instance.Enabled = true;
        }

        public override void Update(GameTime gameTime)
        {

            //if (GameManager.KeyboardState.IsKeyUp(Keys.Up) && GameManager.PreviousKeyboardState.IsKeyDown(Keys.Up))
            //{
            //    OnFinish(true);
            //}

            //if (GameManager.KeyboardState.IsKeyUp(Keys.Down) && GameManager.PreviousKeyboardState.IsKeyDown(Keys.Down))
            //{
            //    progress();
            //}
            if (GameManager.KeyboardState.IsKeyUp(Keys.Enter) && GameManager.PreviousKeyboardState.IsKeyDown(Keys.Enter))
            {
                switch (State)
                {
                    case 0:
                        switch (TalkID)
                        {
                            case 5:
                            case 6:
                            case 7:
                                DisableTalk();
                                break;
                            default:
                                PlayerText.text = Conversations[State][TalkID];
                                TalkID++;
                                break;
                        }
                        break;
                    case 1:
                        switch (TalkID)
                        {
                            case 2:
                            case 3:
                                DisableTalk();
                                break;
                            default:
                                PlayerText.text = Conversations[State][TalkID];
                                TalkID++;
                                break;
                        }
                        break;
                    case 2:
                        if (TalkID == 3)
                            DisableTalk();
                        else
                        {
                            PlayerText.text = Conversations[State][TalkID];
                            TalkID++;
                        }
                        break;
                    case 3:
                        if (TalkID == 3)
                            DisableTalk();
                        else
                        {
                            PlayerText.text = Conversations[State][TalkID];
                            TalkID++;
                        }
                        break;
                    case 4:
                        if (TalkID == 5)
                            DisableTalk();
                        else
                        {
                            PlayerText.text = Conversations[State][TalkID];
                            TalkID++;
                        }
                        break;
                    case 5:
                        if (TalkID == 3)
                            DisableTalk();
                        else
                        {
                            PlayerText.text = Conversations[State][TalkID];
                            TalkID++;
                        }
                        break;
                    case 6:
                        if (TalkID == 3)
                            DisableTalk();
                        else
                        {
                            PlayerText.text = Conversations[State][TalkID];
                            TalkID++;
                        }
                        break;
                    case 7:
                        switch (TalkID)
                        {
                            case 3:
                                DisableTalk();
                                break;
                            case 7:
                                new Hider();
                                Enabled = false;
                                PlayerPanel.Active = false;
                                break;
                            case 8:
                                State++;
                                GameManager.LoadScene(0);
                                break;
                            default:
                                PlayerText.text = Conversations[State][TalkID];
                                TalkID++;
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}

// Author: Mark Voong
// File Name: CStrikeModel.cs
// Project Name: Global Offensive
// Creation Date: Dec 21st, 2015
// Modified Date: Jan 20th, 2016
// Description: Handles all logic processing of the game
using System;
using System.Diagnostics;
using CStrike2DServer;
using LightEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace CStrike2D
{
    public class CStrikeModel
    {
        /// <summary>
        /// 
        /// </summary>
        public UIManager InterfaceManager { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public AudioManager AudioManager { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public InputManager Input { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public NetworkManager NetworkManager { get; private set; }

        public LevelEditor Editor { get; private set; }

        public GameEngine GameEngine { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Camera2D Camera { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Map NewMap { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Random NumGen { get; private set; }

        /// <summary>
        /// Bool for choosing whether the terrorist or counter-terrorist background will show.
        /// True for counter-terrorist, false for terrorist
        /// </summary>
        public bool MenuBackgroundType { get; private set; }

        public float GlowTimer { get; private set; }
        public bool FadeIn { get; private set; }

        // Lobby Variables
        public string Address { get; private set; }
        int port = 27015;

        public int[] Collidables { get; private set; }
        public int[] RaycastCollidables { get; private set; }

        private Stopwatch stopWatch = new Stopwatch();
        public double PathfindingTime { get; private set; }

        private int counter;       // Used to count how many times the screen is drawn
        private decimal timer;     // Used to track 1 second intervals in walltime for counting FPS

        public CStrike2D DriverInstance { get; private set; }

        public Vector2 Center { get; private set; }

        public Vector2 Dimensions { get; private set; }

        public ShaderRenderer Shader { get; private set; }

        private const float SPRAY_TIMER = 0.1f;
        private float shotTimer = 0.0f;

        public enum State
        {
            Menu,
            Options,
            Lobby,
            LevelEditor,
            InGame
        }

        public State CurState { get; private set; }

        public CStrikeModel(CStrike2D driver, Vector2 center, Vector2 dimensions)
        {
            DriverInstance = driver;
            Center = center;
            Dimensions = dimensions;

            // Initialize AudioManager
            AudioManager = new AudioManager();

            // Initialize UIManager
            InterfaceManager = new UIManager();

            // Initialize Control Class
            Input = new InputManager();

            // Initialize Random number generator
            NumGen = new Random();

            GameEngine = new GameEngine(DriverInstance);

            NetworkManager = new NetworkManager(GameEngine);

            Shader = new ShaderRenderer(driver);
        }

        /// <summary>
        /// Called once at the startup of the game
        /// </summary>
        public void Initialize()
        {
            DriverInstance.Assets.LoadCoreContent(DriverInstance);

            CurState = State.Menu;

            Shader.Load();

            // Intialize all UI elements in the game
            #region UI Interface Initialization

            #region Menu Buttons

            InterfaceManager.AddComponent(new Bar("menuTopBar", new Rectangle(0, 20, (int)Dimensions.X, 80), 1.0f, 0.65f,
                Color.Black, EasingFunctions.AnimationType.QuinticInOut, GUIComponent.AnimationDirection.Down,
                DriverInstance.Assets));

            InterfaceManager.AddComponent(new Button("playButton", new Rectangle(120, 20, 200, 80), Color.White,
                "Play", 1.5f, EasingFunctions.AnimationType.QuinticOut, GUIComponent.AnimationDirection.Up,
                DriverInstance.Assets));

            InterfaceManager.AddComponent(new Button("mapEditorButton", new Rectangle(400, 20, 200, 80), Color.White,
                "Editor", 1.5f, EasingFunctions.AnimationType.QuinticOut, GUIComponent.AnimationDirection.Up,
                DriverInstance.Assets));

            InterfaceManager.AddComponent(new Button("optionsButton", new Rectangle(750, 20, 200, 80), Color.White,
                "Options", 1.5f, EasingFunctions.AnimationType.QuinticOut, GUIComponent.AnimationDirection.Up,
                DriverInstance.Assets));

            InterfaceManager.AddComponent(new Button("quitButton", new Rectangle(1100, 20, 200, 80), Color.White, "Quit",
                1.5f, EasingFunctions.AnimationType.QuinticOut, GUIComponent.AnimationDirection.Up,
                DriverInstance.Assets));

            InterfaceManager.FormPage("defaultMenu");

            #endregion

            #region Play Menu

            InterfaceManager.AddComponent(new Bar("playBar", new Rectangle(0, 100, (int)Dimensions.X, 600), 1.0f, 0.8f,
                Color.Black, EasingFunctions.AnimationType.QuinticInOut, GUIComponent.AnimationDirection.Down,
                DriverInstance.Assets));

            InterfaceManager.AddComponent(new Button("connectButton", new Rectangle(150, 200, 200, 80), Color.White,
                "Connect", 1.0f, EasingFunctions.AnimationType.QuinticInOut, GUIComponent.AnimationDirection.None,
                DriverInstance.Assets));

            InterfaceManager.FormPage("playMenu");

            #endregion

            #region Options Menu

            InterfaceManager.AddComponent(new Bar("optionsBar", new Rectangle(0, 100, (int)Dimensions.X, 600), 1.0f, 0.8f,
                Color.Black, EasingFunctions.AnimationType.QuinticInOut, GUIComponent.AnimationDirection.Down,
                DriverInstance.Assets));

            InterfaceManager.AddComponent(new TextBox("masterVolumeText", new Vector2(200, 200), "Master Volume",
                Color.White, 1.0f, EasingFunctions.AnimationType.QuinticInOut, GUIComponent.AnimationDirection.Down,
                DriverInstance.Assets));

            InterfaceManager.AddComponent(new TextBox("uiVolumeText", new Vector2(200, 250), "UI Volume",
                Color.White, 1.1f, EasingFunctions.AnimationType.QuinticInOut, GUIComponent.AnimationDirection.Down,
                DriverInstance.Assets));

            InterfaceManager.AddComponent(new TextBox("musicVolumeText", new Vector2(200, 300), "Music Volume",
                Color.White, 1.2f, EasingFunctions.AnimationType.QuinticInOut, GUIComponent.AnimationDirection.Down,
                DriverInstance.Assets));

            InterfaceManager.AddComponent(new TextBox("sfxVolumeText", new Vector2(200, 350), "Sound Effects Volume",
                Color.White, 1.3f, EasingFunctions.AnimationType.QuinticInOut, GUIComponent.AnimationDirection.Down,
                DriverInstance.Assets));

            InterfaceManager.AddComponent(new TextBox("voiceVolumeText", new Vector2(200, 400), "Voice Volume Volume",
                Color.White, 1.4f, EasingFunctions.AnimationType.QuinticInOut, GUIComponent.AnimationDirection.Down,
                DriverInstance.Assets));

            InterfaceManager.FormPage("optionsMenu");

            #endregion

            #region Buy Menu

            InterfaceManager.AddComponent(new Bar("buyMenuBar", new Rectangle(90, 20, 1100, 650), 0.4f,
                0.8f, Color.Black, EasingFunctions.AnimationType.QuinticInOut, GUIComponent.AnimationDirection.Right,
                DriverInstance.Assets));

            InterfaceManager.AddComponent(new TextBox("buyMenuTitle", new Vector2(94, 20), "Buy Menu", Color.White, 0.4f,
                EasingFunctions.AnimationType.QuinticInOut, GUIComponent.AnimationDirection.Down, DriverInstance.Assets));

            InterfaceManager.FormPage("buyMenu");

            #endregion

            #region Team Select Menu

            InterfaceManager.AddComponent(new TextBox("teamSelectTitle", new Vector2(94, 20), "Buy Menu", Color.White, 0.4f,
                EasingFunctions.AnimationType.QuinticInOut, GUIComponent.AnimationDirection.Down, DriverInstance.Assets));

            InterfaceManager.AddComponent(new Button("ctButton", new Rectangle(90, 50, 549, 630), new Color(0, 81, 200),
                Color.DarkBlue, Color.Gray,
                "Counter-Terrorists", 0.8f, EasingFunctions.AnimationType.QuinticInOut,
                GUIComponent.AnimationDirection.Up, DriverInstance.Assets));

            InterfaceManager.AddComponent(new Button("tButton", new Rectangle(639, 50, 549, 630), new Color(255, 0, 25),
                Color.DarkRed, Color.Gray,
                "Terrorists", 0.8f, EasingFunctions.AnimationType.QuinticInOut,
                GUIComponent.AnimationDirection.Up, DriverInstance.Assets));

            InterfaceManager.FormPage("teamSelectMenu");

            #endregion

            #region Buy Menu Buttons

            InterfaceManager.AddComponent(new Button("pistolMenuButton", new Rectangle(100, 80, 200, 40), Color.Gray,
                Color.White, Color.DarkGray, "1. PISTOLS", 0.4f, EasingFunctions.AnimationType.QuinticInOut,
                GUIComponent.AnimationDirection.Right, DriverInstance.Assets));

            InterfaceManager.AddComponent(new Button("heavyMenuButton", new Rectangle(100, 160, 200, 40), Color.Gray,
                Color.White, Color.DarkGray, "2.  HEAVY", 0.4f, EasingFunctions.AnimationType.QuinticInOut,
                GUIComponent.AnimationDirection.Right, DriverInstance.Assets));

            InterfaceManager.AddComponent(new Button("smgMenuButton", new Rectangle(100, 240, 200, 40), Color.Gray,
                Color.White, Color.DarkGray, "3.  SMGs", 0.4f, EasingFunctions.AnimationType.QuinticInOut,
                GUIComponent.AnimationDirection.Right, DriverInstance.Assets));

            InterfaceManager.AddComponent(new Button("rifleMenuButton", new Rectangle(100, 320, 200, 40), Color.Gray,
                Color.White, Color.DarkGray, "4. RIFLES", 0.4f, EasingFunctions.AnimationType.QuinticInOut,
                GUIComponent.AnimationDirection.Right, DriverInstance.Assets));

            InterfaceManager.AddComponent(new Button("gearMenuButton", new Rectangle(100, 400, 200, 40), Color.Gray,
                Color.White, Color.DarkGray, "5.  GEAR", 0.4f, EasingFunctions.AnimationType.QuinticInOut,
                GUIComponent.AnimationDirection.Right, DriverInstance.Assets));

            InterfaceManager.AddComponent(new Button("grenadeMenuButton", new Rectangle(100, 480, 200, 40), Color.Gray,
                Color.White, Color.DarkGray, "6. GRENADES", 0.4f, EasingFunctions.AnimationType.QuinticInOut,
                GUIComponent.AnimationDirection.Right, DriverInstance.Assets));

            InterfaceManager.FormPage("buyButtonMenu");

            #endregion

            #region Pistol Menu Buttons

            InterfaceManager.AddComponent(new Button("uspMenuButton", new Rectangle(100, 80, 200, 40), Color.Gray,
                Color.White, Color.DarkGray, "USP-S", 0.4f, EasingFunctions.AnimationType.QuinticInOut,
                GUIComponent.AnimationDirection.Right, DriverInstance.Assets));

            InterfaceManager.AddComponent(new Button("dualMenuButton", new Rectangle(100, 160, 200, 40), Color.Gray,
                Color.White, Color.DarkGray, "Dual Berettas", 0.4f, EasingFunctions.AnimationType.QuinticInOut,
                GUIComponent.AnimationDirection.Right, DriverInstance.Assets));

            InterfaceManager.AddComponent(new Button("p250menuButton", new Rectangle(100, 240, 200, 40), Color.Gray,
                Color.White, Color.DarkGray, "P250", 0.4f, EasingFunctions.AnimationType.QuinticInOut,
                GUIComponent.AnimationDirection.Right, DriverInstance.Assets));

            InterfaceManager.AddComponent(new Button("57menuButton", new Rectangle(100, 320, 200, 40), Color.Gray,
                Color.White, Color.DarkGray, "Five-SeveN", 0.4f, EasingFunctions.AnimationType.QuinticInOut,
                GUIComponent.AnimationDirection.Right, DriverInstance.Assets));

            InterfaceManager.AddComponent(new Button("deagleMenuButton", new Rectangle(100, 400, 200, 40), Color.Gray,
                Color.White, Color.DarkGray, "Desert Eagle", 0.4f, EasingFunctions.AnimationType.QuinticInOut,
                GUIComponent.AnimationDirection.Right, DriverInstance.Assets));

            InterfaceManager.FormPage("pistolButtonMenu");

            #endregion

            #region CT Rifle Menu Buttons

            InterfaceManager.AddComponent(new Button("famasMenuButton", new Rectangle(100, 80, 200, 40), Color.Gray,
                Color.White, Color.DarkGray, "Famas", 0.4f, EasingFunctions.AnimationType.QuinticInOut,
                GUIComponent.AnimationDirection.Right, DriverInstance.Assets));

            InterfaceManager.AddComponent(new Button("m4a1MenuButton", new Rectangle(100, 160, 200, 40), Color.Gray,
                Color.White, Color.DarkGray, "M4A1", 0.4f, EasingFunctions.AnimationType.QuinticInOut,
                GUIComponent.AnimationDirection.Right, DriverInstance.Assets));

            InterfaceManager.AddComponent(new Button("ssgMenuButton", new Rectangle(100, 240, 200, 40), Color.Gray,
                Color.White, Color.DarkGray, "SSG 08", 0.4f, EasingFunctions.AnimationType.QuinticInOut,
                GUIComponent.AnimationDirection.Right, DriverInstance.Assets));

            InterfaceManager.AddComponent(new Button("augMenuButton", new Rectangle(100, 320, 200, 40), Color.Gray,
                Color.White, Color.DarkGray, "AUG", 0.4f, EasingFunctions.AnimationType.QuinticInOut,
                GUIComponent.AnimationDirection.Right, DriverInstance.Assets));

            InterfaceManager.AddComponent(new Button("awpMenuButton", new Rectangle(100, 400, 200, 40), Color.Gray,
                Color.White, Color.DarkGray, "AWP", 0.4f, EasingFunctions.AnimationType.QuinticInOut,
                GUIComponent.AnimationDirection.Right, DriverInstance.Assets));

            InterfaceManager.FormPage("ctRifleButtonMenu");

            #endregion

            #region T Rifle Menu Buttons

            InterfaceManager.AddComponent(new Button("galilMenuButton", new Rectangle(100, 80, 200, 40), Color.Gray,
                Color.White, Color.DarkGray, "Galil", 0.4f, EasingFunctions.AnimationType.QuinticInOut,
                GUIComponent.AnimationDirection.Right, DriverInstance.Assets));

            InterfaceManager.AddComponent(new Button("ak47MenuButton", new Rectangle(100, 160, 200, 40), Color.Gray,
                Color.White, Color.DarkGray, "AK47", 0.4f, EasingFunctions.AnimationType.QuinticInOut,
                GUIComponent.AnimationDirection.Right, DriverInstance.Assets));

            InterfaceManager.AddComponent(new Button("ssgMenuButton", new Rectangle(100, 240, 200, 40), Color.Gray,
                Color.White, Color.DarkGray, "SSG 08", 0.4f, EasingFunctions.AnimationType.QuinticInOut,
                GUIComponent.AnimationDirection.Right, DriverInstance.Assets));

            InterfaceManager.AddComponent(new Button("sg558MenuButton", new Rectangle(100, 320, 200, 40), Color.Gray,
                Color.White, Color.DarkGray, "SG558", 0.4f, EasingFunctions.AnimationType.QuinticInOut,
                GUIComponent.AnimationDirection.Right, DriverInstance.Assets));

            InterfaceManager.AddComponent(new Button("awpMenuButton", new Rectangle(100, 400, 200, 40), Color.Gray,
                Color.White, Color.DarkGray, "AWP", 0.4f, EasingFunctions.AnimationType.QuinticInOut,
                GUIComponent.AnimationDirection.Right, DriverInstance.Assets));

            InterfaceManager.FormPage("tRifleButtonMenu");

            #endregion

            #region Scoreboard Menu

            InterfaceManager.AddComponent(new Bar("scoreboardBar", new Rectangle(90, 20, 1100, 650), 0.4f,
                0.8f, Color.Black, EasingFunctions.AnimationType.QuinticInOut, GUIComponent.AnimationDirection.Right,
                DriverInstance.Assets));

            InterfaceManager.AddComponent(new TextBox("scoreboardTitle", new Vector2(94, 20), "Scoreboard", Color.White, 0.4f,
                EasingFunctions.AnimationType.QuinticInOut, GUIComponent.AnimationDirection.Down, DriverInstance.Assets));

            InterfaceManager.AddComponent(new TextBox("ctTitle", new Vector2(94, 50), "Counter-Terrorists", ServerClientInterface.CT_Color,
                0.4f,
                EasingFunctions.AnimationType.QuinticInOut, GUIComponent.AnimationDirection.Right, DriverInstance.Assets));

            InterfaceManager.AddComponent(new TextBox("tTitle", new Vector2(645, 50), "Terrorists", ServerClientInterface.T_Color, 0.4f,
                EasingFunctions.AnimationType.QuinticInOut, GUIComponent.AnimationDirection.Right, DriverInstance.Assets));

            InterfaceManager.FormPage("scoreboard");

            #endregion

            #endregion

            // Show the menu
            InterfaceManager.ShowPage("defaultMenu");

            // Set the default address to empty
            Address = string.Empty;

            // Determine menu background, true if the number is not 0
            MenuBackgroundType = NumGen.Next(0, 2) != 0;

            FadeIn = true;

            Camera = new Camera2D
            {
                Position = Vector2.Zero
            };
        }

        public void Update(float gameTime)
        {
            if (DriverInstance.IsActive)
            {
                Input.Tick();
            }

            InterfaceManager.Update(gameTime);
            NetworkManager.Update(gameTime);

            switch (CurState)
            {
                case State.Menu:
                    if (Shader.BlurAmount >= 0f)
                    {
                        Shader.ChangeBlurAmount(Shader.BlurAmount - 0.2f);
                    }
                    AudioManager.PlaySound("menuMusic", AudioManager.MusicVolume);

                    //AudioManager.PlaySound("menuMusic", AudioManager.MusicVolume, Center, new Vector2(Center.X, Center.Y + 100));
                    if (InterfaceManager.Clicked(Input, "defaultMenu", "playButton"))
                    {
                        InterfaceManager.HidePage("optionsMenu");
                        InterfaceManager.ShowPage("playMenu");
                        AudioManager.PlaySound("buttonclick", AudioManager.UiVolume);
                    }
                    else if (InterfaceManager.Clicked(Input, "defaultMenu", "optionsButton"))
                    {
                        InterfaceManager.HidePage("playMenu");
                        InterfaceManager.ShowPage("optionsMenu");
                        AudioManager.PlaySound("buttonclick", AudioManager.UiVolume);
                    }
                    else if (InterfaceManager.Clicked(Input, "defaultMenu", "quitButton"))
                    {
                        DriverInstance.Exit();
                    }
                    else if (InterfaceManager.Clicked(Input, "defaultMenu", "mapEditorButton"))
                    {
                        InterfaceManager.HideAll();
                        AudioManager.PlaySound("buttonclick", AudioManager.UiVolume);
                        CurState = State.LevelEditor;
                        Editor = new LevelEditor(this);
                    }

                    if (InterfaceManager.Clicked(Input, "playMenu", "connectButton"))
                    {
                        InterfaceManager.HideAll();
                        AudioManager.PlaySound("buttonclick", AudioManager.UiVolume);
                        CurState = State.Lobby;
                    }
                    
                    /*
                    if (FadeIn)
                    {
                        GlowTimer += gameTime;

                        if (GlowTimer >= 1.0f)
                        {
                            FadeIn = !FadeIn;
                        }
                    }
                    else
                    {
                        GlowTimer -= gameTime;

                        if (GlowTimer <= 1.0f)
                        {
                            FadeIn = !FadeIn;
                        }
                    }

                    if (Input.Held(Keys.A))
                    {
                        Camera.Position.X -= 5f;
                    }

                    if (Input.Held(Keys.D))
                    {
                        Camera.Position.X += 5f;
                    }

                    if (Input.Held(Keys.W))
                    {
                        Camera.Position.Y -= 5f;
                    }

                    if (Input.Held(Keys.S))
                    {
                        Camera.Position.Y += 5f;
                    }

                    stopWatch.Start();
                    Collidables = GetCollidableTiles(NewMap.ToTile(Camera.Col(), Camera.Row()));
                    stopWatch.Stop();

                    RaycastCollidables = GetWalls(NewMap.ToTile(Camera.Col(), Camera.Row()), (24 * (GlowTimer / 1000f)) * (float)Math.PI / 12f);

                    PathfindingTime = Math.Round((double)stopWatch.ElapsedMilliseconds, 10);

                    stopWatch.Reset();
                    */
                    break;
                case State.Options:
                    break;
                case State.Lobby:
                    switch (NetworkManager.CurState)
                    {
                        case NetworkManager.NetState.Disconnected:
                            if (Input.Tapped(Keys.Escape))
                            {
                                CurState = State.Menu;
                            }
                            else if (Input.Tapped(Keys.Enter))
                            {
                                NetworkManager.Connect(Address);
                                GameEngine.Initialize(NetworkManager, AudioManager, Input, DriverInstance.Assets);
                                GameEngine.CurState = GameEngine.GameEngineState.Loaded;
                            }
                            else
                            {
                                Address = Input.GetText(Address);
                            }
                            
                            break;
                        case NetworkManager.NetState.Connected:
                            if (DriverInstance.Assets.GameContentLoaded)
                            {
                                CurState = State.InGame;
                                InterfaceManager.ShowPage("teamSelectMenu");
                            }
                            else
                            {
                                DriverInstance.Assets.LoadGameContent();
                                DriverInstance.Assets.LoadMapContent();
                                Camera.ResetZoom();
                                Camera.Position = new Vector2(DriverInstance.Assets.MapData.MapArea.Width / 2f,
                                    DriverInstance.Assets.MapData.MapArea.Height / 2f);
                            }
                            break;
                    }
                    break;
                case State.LevelEditor:
                    if (Editor.CurrentState != LevelEditor.EditorStates.Exit)
                    {
                        Editor.Update(gameTime);
                    }
                    else
                    {
                        CurState = State.Menu;
                        InterfaceManager.ShowPage("defaultMenu");
                    }
                    break;
                case State.InGame:
                    GameEngine.Update(gameTime);
                    
                    break;
            }

            if (DriverInstance.IsActive)
            {
                Input.Tock();
            }
        }

        /*
        public void LoadMap(string mapFolder)
        {
            if (File.Exists("map/" + mapFolder + "/tileData.txt"))
            {
                string[] data = File.ReadAllLines("map/" + mapFolder + "/tileData.txt");
                int width = Convert.ToInt32(data[0]);
                int height = Convert.ToInt32(data[1]);
                int[,] tiledata = new int[width, height];

                for (int y = 0; y < height; y++)
                {
                    string rowData = data[y + 2];
                    for (int x = 0; x < width; x++)
                    {
                        tiledata[x, y] = Convert.ToInt32(rowData[x]) - 48;
                    }
                }
                NewMap = new Map(tiledata);
            }
        }

        public void WriteMap(string mapFolder)
        {
            if (!Directory.Exists("map/" + mapFolder))
            {
                Directory.CreateDirectory("map/" + mapFolder);
            }
            StreamWriter stream = File.CreateText("map/" + mapFolder + "/tiledata.txt");

            int[,] defMap =
            {
                {1, 1, 1, 1, 1},
                {1, 0, 0, 0, 1},
                {1, 0, 0, 0, 1},
                {1, 0, 0, 0, 1},
                {1, 1, 0, 1, 1},
                {1, 0, 0, 0, 1},
                {1, 1, 1, 1, 1}
            };


            stream.WriteLine(defMap.GetLength(1));
            stream.WriteLine(defMap.GetLength(0));

            for (int y = 0; y < defMap.GetLength(0); y++)
            {
                for (int x = 0; x < defMap.GetLength(1); x++)
                {
                    stream.Write(defMap[y, x]);
                }
                stream.WriteLine();
            }

            stream.Close();

            LoadMap("bigmap");
        }

        
        public int[] GetCollidableTiles(int origin)
        {
            List<int> tiles = new List<int>();

            for (int i = 0; i < 8; i++)
            {
                int tile = SearchTile(origin, i);

                if (tile != -1)
                {
                    tiles.Add(tile);
                }
            }
            return tiles.ToArray();
        }
        

        public int SearchTile(int origin, int direction)
        {
            // Left Down Right Up
            // Top-Left Top-Right Bottom-Left Bottom-Right
            switch (direction)
            {
                // Left
                case 0:
                    if ((origin - 1) > 0)
                    {
                        if (NewMap.TypeFromTileNumber(origin - 1) == 0)
                        {
                            return SearchTile(origin - 1, direction);
                        }
                        return origin - 1;
                    }
                    break;
                // Down
                case 1:
                    if ((origin + NewMap.MaxCol) < NewMap.MaxTiles)
                    {
                        if (NewMap.TypeFromTileNumber(origin + NewMap.MaxCol) == 0)
                        {
                            return SearchTile(origin + NewMap.MaxCol, direction);
                        }
                        return (origin + NewMap.MaxCol);
                    }
                    break;
                // Right
                case 2:
                    if ((origin + 1) < NewMap.MaxTiles)
                    {
                        if (NewMap.TypeFromTileNumber(origin + 1) == 0)
                        {
                            return SearchTile(origin + 1, direction);
                        }
                        return origin + 1;
                    }
                    break;
                // Up
                case 3:
                    if ((origin - (NewMap.MaxCol)) > 0)
                    {
                        if (NewMap.TypeFromTileNumber(origin - NewMap.MaxCol) == 0)
                        {
                            return SearchTile(origin - NewMap.MaxCol, direction);
                        }
                        return (origin - (NewMap.MaxCol));
                    }
                    break;
                // Top-Left
                case 4:
                    if ((origin - (NewMap.MaxCol + 1)) >= 0)
                    {
                        if (NewMap.TypeFromTileNumber(origin - (NewMap.MaxCol + 1)) == 0)
                        {
                            return SearchTile(origin - (NewMap.MaxCol + 1), direction);
                        }
                        return (origin - (NewMap.MaxCol + 1));
                    }
                    break;
                // Top-Right
                case 5:
                    if (origin - (NewMap.MaxCol - 1) > 0)
                    {
                        if (NewMap.TypeFromTileNumber(origin - (NewMap.MaxCol - 1)) == 0)
                        {
                            return SearchTile(origin - (NewMap.MaxCol - 1), direction);
                        }
                        return (origin - (NewMap.MaxCol - 1));
                    }
                    break;
                // Bottom-Left
                case 6:
                    if (origin + (NewMap.MaxCol - 1) < NewMap.MaxTiles)
                    {
                        if (NewMap.TypeFromTileNumber(origin + (NewMap.MaxCol - 1)) == 0)
                        {
                            return SearchTile(origin + (NewMap.MaxCol - 1), direction);
                        }
                        return origin + (NewMap.MaxCol - 1);
                    }
                    break;
                // Bottom-Right
                case 7:
                    if (origin + (NewMap.MaxCol + 1) < NewMap.MaxTiles)
                    {
                        if (NewMap.TypeFromTileNumber(origin + (NewMap.MaxCol + 1)) == 0)
                        {
                            return SearchTile(origin + (NewMap.MaxCol + 1), direction);
                        }
                        return origin + (NewMap.MaxCol + 1);
                    }
                    break;
                default:
                    return -1;
            }
            return -1;
        }
        
        /*
        public int[] GetWalls(int origin, float angle)
        {
            angle = MathHelper.Clamp(angle, 0, 6);
            int[] rowCol = NewMap.FromTile(origin);
            List<int> tiles = new List<int>();

            // 1 = Bottom-Right (PI / 2)
            // 2 = Bottom-Left (PI)
            // 3 = Top-Left (3 * PI / 2)
            // 4 = Top-Right (2 PI)
            switch (MathOps.ReturnQuadrant(angle))
            {
                case 1:
                    for (int row = rowCol[1]; row < NewMap.MaxRow; row++)
                    {
                        for (int col = rowCol[0]; col < NewMap.MaxCol; col++)
                        {
                            if (NewMap.TileMap[col, row].TileType == 1)
                            {
                                tiles.Add(NewMap.ToTile(col, row));
                            }
                        }
                    }
                    break;
                case 2:
                    for (int row = rowCol[1]; row < NewMap.MaxRow; row++)
                    {
                        for (int col = 0; col <= rowCol[0]; col++)
                        {
                            if (NewMap.TileMap[col, row].TileType == 1)
                            {
                                tiles.Add(NewMap.ToTile(col, row));
                            }
                        }
                    }
                    break;
                case 3:
                    for (int row = 0; row <= rowCol[1]; row++)
                    {
                        for (int col = 0; col <= rowCol[0]; col++)
                        {
                            if (NewMap.TileMap[col, row].TileType == 1)
                            {
                                tiles.Add(NewMap.ToTile(col, row));
                            }
                        }
                    }
                    break;
                case 4:
                    for (int row = 0; row <= rowCol[1]; row++)
                    {
                        for (int col = rowCol[0]; col < NewMap.MaxCol; col++)
                        {
                            if (NewMap.TileMap[col, row].TileType == 1)
                            {
                                tiles.Add(NewMap.ToTile(col, row));
                            }
                        }
                    }
                    break;
            }

            if (tiles.Count == 0)
            {
                throw new Exception("Rip skins");
            }

            return tiles.ToArray();
        }
         */
    }
}
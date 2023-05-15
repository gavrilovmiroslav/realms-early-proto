using Microsoft.VisualBasic.FileIO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.Extended;
using MonoGame.Extended.Tweening;

using NLua;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace realms.Code
{
    public class Realms : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Lua _scripting = new Lua();

        private static SpriteFont _largeFont;
        private static SpriteFont _smallFont;
        private static Texture2D _eventCard;
        private static Texture2D _resourceCard;
        private static Texture2D _resourceCardFrame;

        private readonly Tweener _tweener = new Tweener();

        public static SpriteFont BigFont => _largeFont;
        public static SpriteFont SmallFont => _smallFont;
        public static Texture2D EventCard => _eventCard;
        public static Texture2D ResourceCard => _resourceCard;
        public static Texture2D ResourceCardFrame => _resourceCardFrame;

        public GameState GameState = new();
        public Progression Progression = new();
        
        public EventCardView EventView;

        public static Realms Instance;

        private void LoadJourney(string name)
        {
            var result = _scripting.LoadScript(name);
            
            if (result.LoadState == ScriptLoadState.LoadFileNotFound)
                throw new Exception($"Failed to load script {name}.lua: File not found.");
            else if (result.LoadState == ScriptLoadState.LoadInterpretError)
                throw new Exception($"Failed to load script {name}.lua: Failed to interpret.");
        }

        private void LoadEventPack(string name)
        {
            var result = _scripting.LoadScript(name);
            if (result.LoadState == ScriptLoadState.LoadOk)
            {
                GameState.EventPacks.Add(name.Capitalize(), result.Values);
            }
            else
            {
                if (result.LoadState == ScriptLoadState.LoadFileNotFound)
                    throw new Exception($"Failed to load script {name}.lua: File not found.");
                else 
                    throw new Exception($"Failed to load script {name}.lua: Failed to interpret.");
            }
        }

        public Realms()
        {
            Instance = this;

            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 800;
            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _scripting["all"] = 0;
            _scripting["game"] = GameState;
            _scripting["card"] = new Card();
            _scripting["give"] = new Give();
            _scripting["add"] = new Get();

            LoadEventPack("castle");
            LoadEventPack("griffins");
            LoadEventPack("healing");
            LoadEventPack("leadership");
            LoadEventPack("treasure");
            LoadEventPack("common");

            LoadJourney("main");

            Progression.OnEnter_Starting += Progression_OnEnter_Starting;
            Progression.OnEnter_NewDay += Progression_OnEnter_NewDay;
            Progression.OnEnter_NextEvent += Progression_OnEnter_NextEvent;
            Progression.OnEnter_ResolveExchange += Progression_OnEnter_ResolveExchange;
            Progression.OnEnter_UpdateHand += Progression_OnEnter_UpdateHand;
            Progression.OnEnter_End += Progression_OnEnter_End;

            EventView = new EventCardView(GameState);

            StartGame();
        }

        private void Progression_OnEnter_End(object sender, string e)
        {
            Debug.WriteLine("END!");
            Realms.Instance.GameOver = true;
        }

        private EventOption OfferedOption = null;

        private void Progression_OnEnter_UpdateHand(object sender, string e)
        {
            Debug.WriteLine("UPDATE HAND!");

            foreach (var card in GameState.Hand.Where(card => card.Offered))
            {
                GameState.Exchange.Add(card);
            }
            GameState.Hand.RemoveAll(card => card.Offered);
            foreach (var card in GameState.Exchange)
            {
                card.Exchange(ref GameState);
            }
            GameState.Exchange.Clear();

            foreach (var resp in OfferedOption.Responses)
            {
                if (resp == null) continue;
                if (GameState.Hand.Count <= HandLimit)
                    resp.Apply();
            }
            OfferedOption = null;

            Progression.Trigger("done");
        }

        private void Progression_OnEnter_ResolveExchange(object sender, string e)
        {
            Debug.WriteLine("RESOLVE EXCHANGE!");
            var index = Realms.Instance.EventView.SelectedOption.Value;
            var currentEvent = Realms.Instance.GameState.CurrentEvent;
            var option = currentEvent.Options[index];

            OfferedOption = option;

            Realms.Instance.EventView.SelectedOption = null;
            Progression.Trigger("all resolved");
        }

        Timer timer = null;
        private bool GameOver;
        private int HandLimit = 15;

        private void Progression_OnEnter_NextEvent(object sender, string e)
        {
            Debug.WriteLine("NEXT EVENT!");

            if (GameState.Deck.Count == 0)
            {
                Debug.WriteLine("STARTING END OF DAY TIMER!");
                GameState.CurrentEvent = null;
                timer = new Timer();
                timer.Callback += (object sender, EventArgs e) => 
                {
                    Progression.Trigger("new day");
                };
                timer.Start(1.0f);
            }
            else
            {
                var eventName = GameState.Deck.First();
                GameState.Deck.RemoveAt(0);
                var func = _scripting[eventName] as LuaFunction;
                GameState.CurrentEvent = func.Call(GameState).First() as EventCard;
            }
        }

        private void Progression_OnEnter_NewDay(object sender, string e)
        {
            Debug.WriteLine("NEW DAY!");
            if (GameState.StartNewDay())
            {
                Progression.Trigger("start event");
            }
            else
            {
                Progression.Trigger("end");
            }
        }

        private void Progression_OnEnter_Starting(object sender, string e)
        {
            Debug.WriteLine("STARTING!");
        }

        public void StartGame()
        {
            Progression.Trigger("start");
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            _largeFont = Content.Load<SpriteFont>("Hardpixel");
            _smallFont = Content.Load<SpriteFont>("Softpixel");
            _eventCard = Content.Load<Texture2D>("event-card");
            _resourceCard = Content.Load<Texture2D>("resource-card");
            _resourceCardFrame = Content.Load<Texture2D>("resource-card-frame");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
             || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            MouseCursor.Update(gameTime);
            EventView.Update(gameTime);

            if (timer != null)
            {
                timer.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (GameOver || Realms.Instance.GameState.Journey.Count == 0)
            {
                GraphicsDevice.Clear(Color.DarkSlateGray);

                _spriteBatch.Begin();
                _spriteBatch.DrawString(Realms.BigFont, "GAME OVER", new Vector2(600, 200), Color.White);
                _spriteBatch.End();
                return;
            }

            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(Realms.BigFont, "NEW DAY STARTING...", new Vector2(520, 200), Color.White);
            EventView.Draw(_spriteBatch);

            var mouse = Mouse.GetState();
            for (int i = 0; i < GameState.Hand.Count; i++)
            {
                var card = GameState.Hand[i];
                var rect = new Rectangle(100 + i * 50, 600, i == GameState.Hand.Count - 1 ? 101 : 50, 228);
                var selected = rect.Contains(mouse.Position);
                card.Draw(_spriteBatch, rect.X, rect.Y - (selected ? 40 : 0));                

                if (selected && MouseCursor.Click)
                {
                    card.ToggleOffer();
                }
            }
            
            // debug
            _spriteBatch.DrawString(Realms.BigFont, "Day:     " + GameState.Day, new Vector2(30, 30), Color.White);
            _spriteBatch.DrawString(Realms.BigFont, "Deck:    " + (GameState.Deck.Count + 1), new Vector2(30, 50), Color.White);
            _spriteBatch.DrawString(Realms.BigFont, "State:   " + Realms.Instance.Progression.Stage, new Vector2(30, 70), Color.White);
            _spriteBatch.DrawString(Realms.BigFont, "Offered: " + Realms.Instance.GameState.Hand.Where(card => card.Offered).Count(), new Vector2(30, 90), Color.White);
            if (Realms.Instance.GameState.Journey.Count > 0)
            {
                _spriteBatch.DrawString(Realms.BigFont, "Place:   " + Realms.Instance.GameState.Journey.First(), new Vector2(30, 110), Color.White);
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}

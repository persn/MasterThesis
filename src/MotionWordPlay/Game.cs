﻿namespace NTNU.MotionWordPlay
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using UserInterface;
    using Inputs;
    using Inputs.Keyboard;
    using Inputs.Motion;
    using MotionControlWrapper;
    using WordPlay;
    using Color = System.Drawing.Color;

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        private const string SwapObjectGestureName = "swapObjectGesturePlaceholder";
        private const string CheckAnswerGestureName = "checkAnswerGesturePlaceholder";

        private static readonly Vector2 BaseScreenSize = new Vector2(640, 360);
        private readonly GraphicsDeviceManager _graphicsDevice;
        private SpriteBatch _spriteBatch;
        private Matrix _globalTransformation;

        private readonly InputHandler _inputHandler;
        private readonly IUserInterface _userInterface;

        private DemoGameLogic _demoGame;

        public Game()
        {
            _graphicsDevice = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = (int)BaseScreenSize.X,
                PreferredBackBufferHeight = (int)BaseScreenSize.Y,
                GraphicsProfile = GraphicsProfile.Reach
            };
            _graphicsDevice.DeviceCreated += GraphicsDeviceCreated;

            Content.RootDirectory = "Content";

            _inputHandler = new InputHandler();
            _inputHandler.KeyboardInput.KeyPressed += KeyboardInputKeyPressed;
            _inputHandler.MotionController.GesturesReceived += MotionControllerGesturesReceived;
            _userInterface = new EmptyKeysWrapper();
            _demoGame = new DemoGameLogic(6, _userInterface);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _globalTransformation = _inputHandler.MotionController.CalculateDrawScale(BaseScreenSize);

            _inputHandler.Initialize();
            _userInterface.Initialize();
            _demoGame.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _inputHandler.Load(Content);
            _userInterface.Load(Content);
            _demoGame.Load(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();

            base.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            _inputHandler.Update(gameTime);
            _demoGame.Update(gameTime);
            _userInterface.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.Transparent);

            _spriteBatch.Begin(SpriteSortMode.Deferred, transformMatrix: _globalTransformation);

            _inputHandler.Draw(gameTime, _spriteBatch);

            _spriteBatch.End();

            // Independent draw
            _userInterface.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            if (_inputHandler != null)
            {
                _inputHandler.Dispose();
            }

            base.OnExiting(sender, args);
        }

        private void GraphicsDeviceCreated(object sender, EventArgs e)
        {
            _inputHandler.GraphicsDeviceCreated(GraphicsDevice, BaseScreenSize);
            _userInterface.GraphicsDeviceCreated(GraphicsDevice, BaseScreenSize);
        }

        private void KeyboardInputKeyPressed(object sender, KeyPressedEventArgs e)
        {
            switch (e.PressedKey)
            {
                case Keys.D1:
                    _inputHandler.MotionController.CurrentFrameState = FrameState.Color;
                    break;
                case Keys.D2:
                    _inputHandler.MotionController.CurrentFrameState = FrameState.Depth;
                    break;
                case Keys.D3:
                    _inputHandler.MotionController.CurrentFrameState = FrameState.Infrared;
                    break;
                case Keys.D4:
                    _inputHandler.MotionController.CurrentFrameState = FrameState.Silhouette;
                    break;
                case Keys.F4:
                    _graphicsDevice.IsFullScreen = !_graphicsDevice.IsFullScreen;
                    _graphicsDevice.ApplyChanges();
                    break;
                case Keys.A:
                    _demoGame.SwapObjects(0, 1);
                    break;
                case Keys.S:
                    _demoGame.SwapObjects(1, 2);
                    break;
                case Keys.D:
                    _demoGame.SwapObjects(2, 3);
                    break;
                case Keys.F:
                    _demoGame.SwapObjects(3, 4);
                    break;
                case Keys.G:
                    _demoGame.SwapObjects(4, 5);
                    break;
                case Keys.Q:
                    _demoGame.LoadTask();
                    break;
                case Keys.W:
                    _demoGame.CheckAnswer();
                    break;
                default:
                    throw new NotSupportedException("Key is not supported");
            }

            _globalTransformation = _inputHandler.MotionController.CalculateDrawScale(BaseScreenSize);
        }

        private void MotionControllerGesturesReceived(object sender, GestureReceivedEventArgs e)
        {
            List<int> playersDoingSwapObjectGesture = new List<int>();
            List<int> playersDoingCheckAnswerGesture = new List<int>();
            for (int i = 0; i < 6; i++)
            {
                IList<GestureResult> gestures = e.Gestures.GetGestures(i);

                foreach (GestureResult gestureResult in gestures)
                {
                    if (gestureResult.Name.Equals(SwapObjectGestureName))
                    {
                        playersDoingSwapObjectGesture.Add(i);
                    }
                    else if (gestureResult.Name.Equals(CheckAnswerGestureName))
                    {
                        playersDoingCheckAnswerGesture.Add(i);
                    }
                }
            }
            if (playersDoingCheckAnswerGesture.Count == _demoGame.NumPlayers)
            {
                _demoGame.CheckAnswer();
            }
            if (playersDoingSwapObjectGesture.Count >= 2)
            {
                _demoGame.SwapObjects(playersDoingSwapObjectGesture[0], playersDoingSwapObjectGesture[1]);
            }
        }
    }
}

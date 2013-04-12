using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace sgd_project
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Lander : Game
    {
        private const float Boundary = 100000.0f;
        private const float CameraVerticalAngle = -MathHelper.PiOver4;
        public static readonly Vector3 Metre = new Vector3(39.4f, 39.4f, 39.4f);
        private readonly Vector3 _cameraPosition = new Vector3(0.0f, 0.0f, 250.0f);
        private readonly GraphicsDeviceManager _graphics;
        private readonly Dictionary<string, Body> _gravity = new Dictionary<string, Body>();

        private LandingPad _currentObjective;
        private bool _storeAvailable;

        private readonly VertexPositionColorTexture[] _groundVertices = new VertexPositionColorTexture[4];
        private readonly List<LandingPad> _pads = new List<LandingPad>();
        private Viewport _bottomView;
        private BlendState _bs;
        private float _cameraHorizontalAngle;
        private Body _currentGravity;
        private bool _debug;
        private DepthStencilState _dss;

        private Model _flame;
        private MenuScreen _gameOver;
        private Texture2D _grassTexture;
        private HeadsUpDisplay _hud;
        private bool _invertedControls;
        private Model _landingPad;
        private Model _landingPadGreen;
        private Lem _lem = new Lem();
        private Model _lemModel;
        private Viewport _mainView;
        private Menu _menu;

        private SoundEffect _menuBack;
        private SoundEffect _menuMove;
        private SoundEffect _menuSelect;
        private MenuScreen _pause;
        private Effect _positionColorEffect;
        private EffectParameter _positionColorEffectWvp;
        private RasterizerState _rs;
        private bool _running;
        private SpriteFont _scoreFont;
        private SpriteBatch _spriteBatch;
        private SamplerState _ss;
        private Effect _textureEffect;
        private EffectParameter _textureEffectImage;
        private EffectParameter _textureEffectWvp;
        private Stream _soundfile;
        private SoundEffect _soundEffect;

        private AudioListener _audioListener = new AudioListener();

        public Lander()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            _textureEffect = Content.Load<Effect>("Shaders\\Texture");
            _positionColorEffect = Content.Load<Effect>("Shaders\\PositionColor");
            _positionColorEffectWvp = _positionColorEffect.Parameters["wvpMatrix"];
            BoundingSphereRenderer.Init(_positionColorEffectWvp, _positionColorEffect);
            _textureEffectWvp = _textureEffect.Parameters["wvpMatrix"];
            _textureEffectImage = _textureEffect.Parameters["textureImage"];


            const float border = Boundary;
            var uv = new Vector2(0.0f, 0.0f);
            var pos = new Vector3(0.0f, 0.0f, 0.0f);
            Color color = Color.White;

            // top left
            uv.X = 0.0f;
            uv.Y = 0.0f;
            pos.X = -border;
            pos.Y = 0.0f;
            pos.Z = -border;
            _groundVertices[0] = new VertexPositionColorTexture(pos, color, uv);

            // bottom left
            uv.X = 0.0f;
            uv.Y = 10.0f;
            pos.X = -border;
            pos.Y = 0.0f;
            pos.Z = border;
            _groundVertices[1] = new VertexPositionColorTexture(pos, color, uv);

            // top right
            uv.X = 10.0f;
            uv.Y = 0.0f;
            pos.X = border;
            pos.Y = 0.0f;
            pos.Z = -border;
            _groundVertices[2] = new VertexPositionColorTexture(pos, color, uv);

            // bottom right
            uv.X = 10.0f;
            uv.Y = 10.0f;
            pos.X = border;
            pos.Y = 0.0f;
            pos.Z = border;
            _groundVertices[3] = new VertexPositionColorTexture(pos, color, uv);


            _menu = new Menu();

            //Using the sprite batch messes up these settings
            _rs = GraphicsDevice.RasterizerState;
            _dss = GraphicsDevice.DepthStencilState;
            _bs = GraphicsDevice.BlendState;
            _ss = GraphicsDevice.SamplerStates[0];

            base.Initialize();
        }


        /// <summary>
        /// Inits the menu.
        /// </summary>
        private void InitMenu()
        {
            var start = new MenuScreen("Asteroids", null);
            var about = new MenuScreen("About", null);
            var planet = new MenuScreen("Planet", null);
            var options = new MenuScreen("Options", null);
            var inverted = new MenuScreen("Inverted Controls", options);
            var debug = new MenuScreen("Debug Mode", options);
            _pause = new MenuScreen("Paused", null);
            _gameOver = new MenuScreen("Game Over", null);
            var controls = new MenuScreen("Controls", null);

            var e = new Dictionary<string, Action>
                {
                    {
                        "On", () =>
                            {
                                _invertedControls = true;
                                _menu.SelectedMenuScreen = _menu.Screens.IndexOf(options);
                            }
                    },
                    {
                        "Off", () =>
                            {
                                _invertedControls = false;
                                _menu.SelectedMenuScreen = _menu.Screens.IndexOf(options);
                            }
                    }
                };
            inverted.Elements = e;


            e = new Dictionary<string, Action>
                {
                    {
                        "On", () =>
                            {
                                _debug = true;
                                _menu.SelectedMenuScreen = _menu.Screens.IndexOf(options);
                            }
                    },
                    {
                        "Off", () =>
                            {
                                _debug = false;
                                _menu.SelectedMenuScreen = _menu.Screens.IndexOf(options);
                            }
                    }
                };
            debug.Elements = e;

            e = new Dictionary<string, Action>
                {
                    {
                        "Start Game", () =>
                            {
                                _menu.MainMenuIndex = _menu.Screens.IndexOf(_pause);
                                _menu.SelectedMenuScreen = _menu.MainMenuIndex;
                                NewGame();
                            }
                    },
                    {"Planet", () => { _menu.SelectedMenuScreen = _menu.Screens.IndexOf(planet); }},
                    {"Options", () => { _menu.SelectedMenuScreen = _menu.Screens.IndexOf(options); }},
                    {"About", () => { _menu.SelectedMenuScreen = _menu.Screens.IndexOf(about); }},
                    {"Controls", () => { _menu.SelectedMenuScreen = _menu.Screens.IndexOf(controls); }},
                    {"Quit", Exit}
                };

            start.Elements = e;

            e = new Dictionary<string, Action>
                {
                    {
                        "Inverted Controls", () =>
                            {
                                inverted.SelectedIndex = _invertedControls ? 0 : 1;
                                _menu.SelectedMenuScreen = _menu.Screens.IndexOf(inverted);
                            }
                    },
                    {
                        "Debug Mode", () =>
                            {
                                debug.SelectedIndex = _debug ? 0 : 1;
                                _menu.SelectedMenuScreen = _menu.Screens.IndexOf(debug);
                            }
                    }
                };

            options.Elements = e;

            e = new Dictionary<string, Action>
                {
                    {"Resume", () => { _running = true; }},
                    {"New Game", NewGame},
                    {"Planet", () => { _menu.SelectedMenuScreen = _menu.Screens.IndexOf(planet); }},
                    {"Options", () => { _menu.SelectedMenuScreen = _menu.Screens.IndexOf(options); }},
                    {"About", () => { _menu.SelectedMenuScreen = _menu.Screens.IndexOf(about); }},
                    {"Controls", () => { _menu.SelectedMenuScreen = _menu.Screens.IndexOf(controls); }},
                    {"Quit", Exit}
                };

            _pause.Elements = e;

            e = new Dictionary<string, Action>
                {
                    {"New game", NewGame},
                    {"Quit", Exit}
                };

            _gameOver.Elements = e;

            e = new Dictionary<string, Action>
                {
                    {"Samuel Lewis & Thomas Kempton", null},
                    {"Simulation and Game Development", null},
                    {"Project - Post-Protoplanetary Disk Lander", null}
                };

            about.Elements = e;

            e = new Dictionary<string, Action>
                {
                    {"TODO", null}
                };

            controls.Elements = e;

            e = _gravity.ToList().ToDictionary<KeyValuePair<string, Body>, string, Action>(
                pair =>
                string.Format(@"{0, 10} G: {1:0.##}mpsps W: {2:0.##}mps", pair.Key, -pair.Value.Gravity.Y,
                              pair.Value.Wind.Length()),
                pair => (() =>
                    {
                        _lem.Body = pair.Value;
                        _currentGravity = pair.Value;
                        _menu.SelectedMenuScreen = _menu.MainMenuIndex;
                    }));
            planet.Elements = e;

            _menu.AddMenuScreen(start);
            _menu.AddMenuScreen(_gameOver);
            _menu.AddMenuScreen(inverted);
            _menu.AddMenuScreen(debug);
            _menu.AddMenuScreen(options);
            _menu.AddMenuScreen(about);
            _menu.AddMenuScreen(_pause);
            _menu.AddMenuScreen(controls);
            _menu.AddMenuScreen(planet);

            _menu.SelectedMenuScreen = _menu.Screens.IndexOf(start);
            _menu.MainMenuIndex = _menu.Screens.IndexOf(start);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _mainView = GraphicsDevice.Viewport;

            _bottomView = _mainView;
            _bottomView.Height = _mainView.Height/3;
            _bottomView.Width = _bottomView.Height;
            _bottomView.X = 2;
            _bottomView.Y = 2;

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _grassTexture = Content.Load<Texture2D>("Images\\grass");
            _lemModel = Content.Load<Model>("models\\LEM\\LEM");
            _menuMove = Content.Load<SoundEffect>("Sounds\\menuMove");
            _menuSelect = Content.Load<SoundEffect>("Sounds\\menuSelect");
            _scoreFont = Content.Load<SpriteFont>("Fonts\\Mono");
            _menuBack = Content.Load<SoundEffect>("Sounds\\menuBack");
            _flame = Content.Load<Model>("models\\jet\\jet");
            _landingPad = Content.Load<Model>("models\\landingPad\\landingPad");
            _landingPadGreen = Content.Load<Model>("models\\landingPad\\landingPadGreen");


            var hudTexture = Content.Load<Texture2D>("Images\\hud");
            _hud = new HeadsUpDisplay(_scoreFont, hudTexture);
             
            _menu.Initialize(GraphicsDevice.Viewport, _scoreFont, _menuMove, _menuSelect, _menuBack);

            _soundEffect = Content.Load<SoundEffect>("Sounds\\engine");

            var ground = new Ground();
            ground.Init(_textureEffect, _textureEffectWvp, _textureEffectImage, _grassTexture, _groundVertices);

            //Equitorial Surface Body as listed on Wikipedia
            _gravity.Add("sun", new Body(new Vector3(0, -274.0f, 0), new Vector3(0.0f, 0.0f, 0.0f), ground));
            _gravity.Add("mercury", new Body(new Vector3(0, -3.7f, 0), new Vector3(1.0f, 0.0f, 1.0f), ground));
            _gravity.Add("venus", new Body(new Vector3(0, -8.87f, 0), new Vector3(0.0f, 0.0f, 0.0f), ground));
            _gravity.Add("earth", new Body(new Vector3(0, -9.780327f, 0), new Vector3(1.0f, 0.0f, 0.0f), ground));
            _gravity.Add("moon", new Body(new Vector3(0, -1.622f, 0), new Vector3(0.0f, 0.0f, 0.0f), ground));
            _gravity.Add("mars", new Body(new Vector3(0, -3.711f, 0), new Vector3(0.0f, 0.0f, 0.0f), ground));
            _gravity.Add("jupiter", new Body(new Vector3(0, -24.79f, 0), new Vector3(0.0f, 0.0f, 0.0f), ground));
            _gravity.Add("saturn", new Body(new Vector3(0, -10.44f, 0), new Vector3(0.0f, 0.0f, 0.0f), ground));
            _gravity.Add("uranus", new Body(new Vector3(0, -8.69f, 0), new Vector3(0.0f, 0.0f, 0.0f), ground));
            _gravity.Add("neptune", new Body(new Vector3(0, -11.15f, 0), new Vector3(0.0f, 0.0f, 0.0f), ground));
            _gravity.Add("pluto", new Body(new Vector3(0, -0.658f, 0), new Vector3(0.0f, 0.0f, 0.0f), ground));

            _currentGravity = _gravity["moon"];


            InitMenu();

            _lem.Init(new Vector3(0, 500, 0), _lemModel, _flame, _currentGravity, 100, _soundEffect, _audioListener);

            var pad = new LandingPad();
            pad.Init(new Vector3(0, 3, 0)*Metre, _landingPadGreen);
            _pads.Add(pad);
            _currentObjective = pad;
            pad = new LandingPad();
            pad.Init(new Vector3(15, 3, 30)*Metre, _landingPad);
            _pads.Add(pad);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            _grassTexture.Dispose();
            _menuMove.Dispose();
            _menuSelect.Dispose();
            _menuBack.Dispose();
            _hud.Dispose();
            _soundEffect.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            int delta = gameTime.ElapsedGameTime.Milliseconds;
            GamePadState gpState = GamePad.GetState(PlayerIndex.One);
            KeyboardState kbState = Keyboard.GetState();
            var input = new Input(gpState, kbState, _invertedControls);

            if (gpState.IsButtonDown(Buttons.Start) || kbState.IsKeyDown(Keys.Escape))
            {
                _running = false;
            }

            if (_running)
            {
                _cameraHorizontalAngle = input.CameraRotationY()*MathHelper.Pi;

                _lem.Update(delta, input);
            }
            else
            {
                _menu.Update(GraphicsDevice, input, delta);
            }

            Collision();

            base.Update(gameTime);
        }

        private void DrawLem(Matrix look, Matrix projection)
        {
            _lem.Draw( GraphicsDevice, look, projection);
        }

        private void DrawScene(Matrix look, Matrix projection)
        {
            foreach (LandingPad pad in _pads)
            {
                pad.Draw(GraphicsDevice, look, projection);
            }
            _currentGravity.Ground.Draw(GraphicsDevice, look, projection);
        }

        private void DrawDebug(Matrix look, Matrix projection)
        {
            if (_debug)
            {
                foreach (IBound b in _lem.GetBounds())
                {
                    b.Draw(GraphicsDevice, look, projection);
                }
                foreach (LandingPad pad in _pads)
                {
                    foreach (IBound bound in pad.GetBounds())
                    {
                        //bound.Draw(GraphicsDevice, look, projection);
                    }
                }
            }
        }

        private void DrawHud()
        {
            if (_running)
            {
                _spriteBatch.Begin();
                _hud.Draw(
                        _spriteBatch,
                        GraphicsDevice.Viewport,
                        _lem,
                        _currentGravity.Wind);
                _spriteBatch.End();
            }
            else
            {
                _spriteBatch.Begin();
                _menu.Draw(_spriteBatch);
                _spriteBatch.End();
            }

            //Reset values spritebatch changes
            GraphicsDevice.BlendState = _bs;
            GraphicsDevice.RasterizerState = _rs;
            GraphicsDevice.DepthStencilState = _dss;
            GraphicsDevice.SamplerStates[0] = _ss;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Viewport = _mainView;
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // Copy any parent transforms.
            Matrix m =
                Matrix.CreateFromAxisAngle(Vector3.UnitX, CameraVerticalAngle)*
                Matrix.CreateFromAxisAngle(Vector3.UnitY, _cameraHorizontalAngle);
            Vector3 camera = Vector3.Transform(_cameraPosition, m);
            // Draw the model. A model can have multiple meshes, so loop.
            Matrix look = Matrix.CreateLookAt(_lem.Position + camera, _lem.Position, Vector3.Up);
            _audioListener.Position = _lem.Position + camera;
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(80.0f), GraphicsDevice.Viewport.AspectRatio,
                1.0f, 10000.0f);

            DrawLem(look, projection);
            DrawScene(look, projection);
            DrawDebug(look, projection);
            DrawHud();

            GraphicsDevice.Viewport = _bottomView;

            look = Matrix.CreateLookAt(_lem.Position, new Vector3(_lem.Position.X, .5f, _lem.Position.Z + .1f),
                                       Vector3.Down);
            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(30.0f), GraphicsDevice.Viewport.AspectRatio,
                1.0f, 10000.0f);

            DrawScene(look, projection);

            base.Draw(gameTime);
        }

        public void NewGame()
        {
            _menu.MainMenuIndex = _menu.Screens.IndexOf(_pause);
            _menu.SelectedMenuScreen = _menu.MainMenuIndex;
            _lem.Dispose();
            _lem = new Lem();
            _lem.Init(new Vector3(0, 500, 0), _lemModel, _flame, _currentGravity, 100, _soundEffect, _audioListener);
            _running = true;
        }

        public void GameOver( string reason )
        {
            _running = false;
            _gameOver.Title = string.Format(@"Game Over - {0}", reason);
            _menu.MainMenuIndex = _menu.Screens.IndexOf(_gameOver);
            _menu.SelectedMenuScreen = _menu.MainMenuIndex;
        }

        private void Collision()
        {
            foreach (IBound bound in _lem.GetBounds())
            {
                foreach (IBound b in _currentGravity.Ground.GetBounds())
                {
                    if (bound.Intersects(b))
                    {
                        GameOver("Lander touched the ground");
                        break;
                    }
                }
            }

            foreach (LandingPad pad in _pads)
            {
                foreach (IBound bound in pad.GetBounds())
                {
                    while (true)
                    {
                        int collisions = 0;
                        foreach (IBound lemBound in _lem.GetBounds())
                        {
                            if (bound.Intersects(lemBound))
                            {
                                collisions++;
                            }
                        }
                        if (collisions == 4)
                        {
                            _storeAvailable = true;

                            if(pad == _currentObjective)
                            {
                                pad.Model = _landingPad;
                                _currentObjective = _pads[(_pads.IndexOf(pad) + 1) % _pads.Count];
                                _currentObjective.Model = _landingPadGreen;
                            }

                            if (_lem.Velocity.Y < 0)
                            {
                                _lem.Position = new Vector3(_lem.Position.X, bound.Max().Y + Lem.MinY + 2.15f,
                                                            _lem.Position.Z);
                            }
                            if (_lem.Velocity.Length() > 10)
                            {
                                GameOver("Landed too fast");
                            }
                            _lem.Velocity = Vector3.Zero;
                            break;
                        }

                        if (collisions == 0)
                        {
                            break;
                        }


                        if (Math.Abs(_lem.RotationX) == 0.00 && Math.Abs(_lem.RotationZ) == 0.0)
                        {
                            GameOver(string.Format(@"Only {0} feet on landing pad", collisions));
                            break;
                        }

                        if (Math.Abs(_lem.RotationX) > MathHelper.ToRadians(5) ||
                            Math.Abs(_lem.RotationZ) > MathHelper.ToRadians(5) && _lem.Velocity.Y < 0)
                        {
                            GameOver("Landing angle too steep");
                            break;
                        }

                        _lem.RotationX *= .98f;
                        _lem.RotationZ *= .98f;
                        if (Math.Abs(_lem.RotationX) < 0.01 && Math.Abs(_lem.RotationZ) < 0.01)
                        {
                            _lem.RotationX = 0;
                            _lem.RotationZ = 0;
                        }
                    }
                }
            }
        }
    }
}

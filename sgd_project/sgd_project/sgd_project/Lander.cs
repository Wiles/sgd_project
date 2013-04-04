using System;
using System.Collections.Generic;
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
        readonly GraphicsDeviceManager _graphics;
        public static readonly Vector3 Metre = new Vector3(39.4f, 39.4f, 39.4f);
        private Texture2D _grassTexture;
        private SpriteBatch _spriteBatch;

        private Lem _lem = new Lem();

        private const float Boundary = 160000.0f;
        private Model _lemModel;
        private Model _crate;
        private readonly Vector3 _cameraPosition = new Vector3(0.0f, 00.0f, 500.0f);
        private const float CameraHorizontalAngle = -MathHelper.PiOver4;
        private const float CameraVerticalAngle = 0;
        private Effect _textureEffect;
        private EffectParameter _textureEffectWvp;
        private EffectParameter _textureEffectImage;
        private SoundEffect _menuBack;
        private SoundEffect _menuMove;
        private SoundEffect _menuSelect;
        private MenuScreen _pause;
        private bool _running;
        private MenuScreen _gameOver;
        private Menu _menu;
        private const float CameraRps = MathHelper.TwoPi;
        private Body _currentGravity;
        private readonly Dictionary<string, Body> _gravity = new Dictionary<string, Body>();

        readonly VertexPositionColorTexture[] _groundVertices = new VertexPositionColorTexture[4];
        private SpriteFont _scoreFont;
        private RasterizerState _rs;
        private DepthStencilState _dss;
        private BlendState _bs;
        private SamplerState _ss;

        private Viewport _mainView;
        private Viewport _bottomView;

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
            //Equitorial Surface Gravity as listed on Wikipedia
            _gravity.Add("moon", new Body(new Vector3(0, -1.622f, 0), new Vector3(0.0f,0.0f,0.0f)));
            _gravity.Add("earth", new Body(new Vector3(0, -9.780327f, 0), new Vector3(1.0f,0.0f,0.0f)));
            _gravity.Add("jupiter", new Body(new Vector3(0, -24.79f, 0), new Vector3(0.0f,0.0f,0.0f)));
            _gravity.Add("sun", new Body(new Vector3(0, -274.0f, 0), new Vector3(0.0f,0.0f,0.0f)));
            _currentGravity = _gravity["earth"];

            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            _textureEffect = Content.Load<Effect>("Shaders\\Texture");
            _textureEffectWvp = _textureEffect.Parameters["wvpMatrix"];
            _textureEffectImage = _textureEffect.Parameters["textureImage"];

            const float border = Boundary;
            var uv = new Vector2(0.0f, 0.0f);
            var pos = new Vector3(0.0f, 0.0f, 0.0f);
            var color = Color.White;

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
            InitMenu();

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
            _pause = new MenuScreen("Paused", null);
            _gameOver = new MenuScreen("Game Over", null);
            var controls = new MenuScreen("Controls", null);

            var e = new Dictionary<string, Action>
                {
                    {"Start Game", () => {
                        _menu.MainMenuIndex = _menu.Screens.IndexOf(_pause);
                        _menu.SelectedMenuScreen = _menu.MainMenuIndex;
                        NewGame();}},   
                    {"Planet", () => { _menu.SelectedMenuScreen = _menu.Screens.IndexOf(planet); }},
                    {"About", () => { _menu.SelectedMenuScreen = _menu.Screens.IndexOf(about); }},
                    {"Controls", () => { _menu.SelectedMenuScreen = _menu.Screens.IndexOf(controls); }},
                    {"Quit", Exit}
                };

            start.Elements = e;

            e = new Dictionary<string, Action>
                {
                    {"Resume", () => { _running = true; }},
                    {"New Game", NewGame},
                    {"Planet", () => { _menu.SelectedMenuScreen = _menu.Screens.IndexOf(planet); }},
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
                pair => pair.Key, 
                pair => (() =>
                    {
                        _lem.Gravity = pair.Value;
                        _currentGravity = pair.Value;
                        _menu.SelectedMenuScreen = _menu.MainMenuIndex;
                    }));
            planet.Elements = e;

            _menu.AddMenuScreen(start);
            _menu.AddMenuScreen(_gameOver);
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
            _bottomView.Width = _mainView.Width / 3;
            _bottomView.Height = _mainView.Height / 3;
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _grassTexture = Content.Load<Texture2D>("Images\\grass");
            _lemModel = Content.Load<Model>("models\\LEM\\LEM");
            _menuMove = Content.Load<SoundEffect>("Sounds\\menuMove");
            _menuSelect = Content.Load<SoundEffect>("Sounds\\menuSelect");
            _scoreFont = Content.Load<SpriteFont>("Fonts\\Mono");
            _menuBack = Content.Load<SoundEffect>("Sounds\\menuBack");
            _crate = Content.Load<Model>("models\\crate\\crate");
            _menu.Initialize(GraphicsDevice.Viewport, _scoreFont, _menuMove, _menuSelect, _menuBack);
            _lem.Init(new Vector3(0, Lem.MinY, 0), _lemModel, _gravity["earth"], 100);
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
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            var delta = gameTime.ElapsedGameTime.Milliseconds;
            var gpState = GamePad.GetState(PlayerIndex.One);
            var kbState = Keyboard.GetState();

            if(gpState.IsButtonDown(Buttons.Start) || kbState.IsKeyDown(Keys.Escape))
            {
                _running = false;
            }

            if (_running)
            {
              //  _cameraHorizontalAngle += delta/1000.0f*gpState.ThumbSticks.Right.Y*CameraRps;
               // _cameraHorizontalAngle = MathHelper.Clamp(_cameraHorizontalAngle, -MathHelper.PiOver2*.95f,
               //                                           MathHelper.PiOver2*.95f);
              //  _cameraVerticalAngle -= delta/1000.0f*gpState.ThumbSticks.Right.X*CameraRps;

                _lem.Update(delta, gpState, kbState);
            }
            else
            {
                _menu.Update(GraphicsDevice, gpState, kbState, delta);
            }
            base.Update(gameTime);
        }

        private void DrawLem(Matrix look, Matrix projection)
        {
            _lem.Draw(look, projection);
        }

        private void DrawScene(Matrix look, Matrix projection)
        {
        
            
            var transforms = new Matrix[_crate.Bones.Count];
            _crate.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (var mesh in _crate.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] *
                        Matrix.CreateTranslation(new Vector3(0, .5f, 0) * Metre);
                    effect.View = look;
                    effect.Projection = projection;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
            DrawGround(look, projection);

        }

        private void DrawHUD()
        {
            if (_running)
            {
                _spriteBatch.Begin();
                float height = _scoreFont.MeasureString("_").Y;
                _spriteBatch.DrawString(_scoreFont,
                                       string.Format(@"X Rot.: {0:0.00}", MathHelper.ToDegrees(_lem.RotationX)),
                                       new Vector2((GraphicsDevice.Viewport.Width / 2) + 2, height + 2),
                                       Color.White, 0.0f,
                                       Vector2.Zero,
                                       1.0f, SpriteEffects.None, 0.0f);
                _spriteBatch.DrawString(_scoreFont,
                                       string.Format(@"Y Rot.: {0:0.00}", MathHelper.ToDegrees(_lem.RotationZ)),
                                       new Vector2((GraphicsDevice.Viewport.Width / 2) + 2, height * 2 + 2),
                                       Color.White, 0.0f,
                                       Vector2.Zero,
                                       1.0f, SpriteEffects.None, 0.0f);
                _spriteBatch.DrawString(_scoreFont,
                                       string.Format(@"  Fuel: {0:0.00}", _lem.Fuel),
                                       new Vector2((GraphicsDevice.Viewport.Width / 2) + 2, height * 3 + 2),
                                       Color.White, 0.0f,
                                       Vector2.Zero,
                                       1.0f, SpriteEffects.None, 0.0f);
                _spriteBatch.DrawString(_scoreFont,
                                       string.Format(@"  Height: {0:0.00}", (_lem.Position.Y - Lem.MinY) / Metre.Y),
                                       new Vector2((GraphicsDevice.Viewport.Width / 2) + 2, height * 4 + 2),
                                       Color.White, 0.0f,
                                       Vector2.Zero,
                                       1.0f, SpriteEffects.None, 0.0f);

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
            var camera = Vector3.Transform(_cameraPosition,
                                           Matrix.CreateFromAxisAngle(Vector3.UnitX, CameraHorizontalAngle));
            camera = Vector3.Transform(camera, Matrix.CreateFromAxisAngle(Vector3.UnitY, CameraVerticalAngle));
            // Draw the model. A model can have multiple meshes, so loop.
            var look = Matrix.CreateLookAt(_lem.Position + camera, _lem.Position, Vector3.Up);
            var projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45.0f), GraphicsDevice.Viewport.AspectRatio,
                1.0f, 10000.0f);
            DrawLem(look, projection);
            DrawScene(look, projection);
            DrawHUD();

            GraphicsDevice.Viewport = _bottomView;

            look = Matrix.CreateLookAt(_lem.Position, new Vector3(_lem.Position.X, .5f, _lem.Position.Z + .1f), Vector3.Down);
            projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(45.0f), GraphicsDevice.Viewport.AspectRatio,
                1.0f, 10000.0f);

            DrawScene(look, projection);

            base.Draw(gameTime);
        }

        private void DrawGround(Matrix view, Matrix projection)
        {
            // 1: declare matrices

            // 2: initialize matrices
            Matrix translation = Matrix.CreateTranslation(0.0f, 0.0f, 0.0f);

            // 3: build cumulative world matrix using I.S.R.O.T. sequence
            // identity, scale, rotate, orbit(translate & rotate), translate
            Matrix world = translation;

            // 4: set shader parameters
            _textureEffectWvp.SetValue(world * view * projection);
            _textureEffectImage.SetValue(_grassTexture);

            // 5: draw object - primitive type, vertex data, # primitives
            TextureShader(PrimitiveType.TriangleStrip, _groundVertices, 2);
        }

        private void TextureShader(PrimitiveType primitiveType,
                                   VertexPositionColorTexture[] vertexData,
                                   int numPrimitives)
        {
            _textureEffect.Techniques[0].Passes[0].Apply();

            // set drawing format and vertex data then draw surface
            GraphicsDevice.DrawUserPrimitives(
                                    primitiveType, vertexData, 0, numPrimitives);
        }

        public void NewGame()
        {
            _lem = new Lem();
            _lem.Init(new Vector3(0, Lem.MinY, 0), _lemModel, _currentGravity, 100);
            _running = true;
        }
    }
}

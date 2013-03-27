using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace sgd_project
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        readonly GraphicsDeviceManager _graphics;
        private Texture2D _grassTexture;

        private readonly Lem _lem = new Lem();

        private const float Boundary = 1600.0f;
        private Model _lemModel;
        private readonly Vector3 _cameraPosition = new Vector3(0.0f, 00.0f, 10.0f);
        private float _cameraHorizontalAngle = -MathHelper.PiOver4;
        private float _cameraVerticalAngle = MathHelper.PiOver4;
        private Effect _textureEffect;
        private EffectParameter _textureEffectWvp;
        private EffectParameter _textureEffectImage;
        private const float CameraRps = MathHelper.TwoPi;
        private Dictionary<string, float> Gravity = new Dictionary<string, float>();

        readonly VertexPositionColorTexture[] _groundVertices = new VertexPositionColorTexture[4];
        
        public Game1()
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
            Gravity.Add("moon", -1.622f);
            Gravity.Add("earth", -9.81f);

            _graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;

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

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _grassTexture = Content.Load<Texture2D>("Images\\grass");
            _lemModel = Content.Load<Model>("models\\LEM\\LEM");
            _lem.Init(new Vector3(0,1.78f,0), _lemModel, new Vector3(0, Gravity["earth"], 0));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            var delta = gameTime.ElapsedGameTime.Milliseconds;
            // Allows the game to exit
            var gpState = GamePad.GetState(PlayerIndex.One);

            if (gpState.Buttons.Back == ButtonState.Pressed)
                Exit();

            _cameraHorizontalAngle += delta / 1000.0f * gpState.ThumbSticks.Right.Y * CameraRps;
            _cameraHorizontalAngle = MathHelper.Clamp(_cameraHorizontalAngle, -MathHelper.PiOver2 * .95f, MathHelper.PiOver2 * .95f);
            _cameraVerticalAngle -= delta / 1000.0f * gpState.ThumbSticks.Right.X * CameraRps;

            _lem.Update(delta, gpState);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            _graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // Copy any parent transforms.
            var camera = Vector3.Transform(_cameraPosition, Matrix.CreateFromAxisAngle(Vector3.UnitX, _cameraHorizontalAngle));
            camera = Vector3.Transform(camera, Matrix.CreateFromAxisAngle(Vector3.UnitY, _cameraVerticalAngle));
            // Draw the model. A model can have multiple meshes, so loop.
            var look = Matrix.CreateLookAt(_lem.Position + camera, _lem.Position, Vector3.Up);
            var projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45.0f), _graphics.GraphicsDevice.Viewport.AspectRatio,
                        1.0f, 10000.0f);
            _lem.Draw(look, projection);
            DrawGround(look, projection);
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
            _graphics.GraphicsDevice.DrawUserPrimitives(
                                    primitiveType, vertexData, 0, numPrimitives);
        }
    }
}

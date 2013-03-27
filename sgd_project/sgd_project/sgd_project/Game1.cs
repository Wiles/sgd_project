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
        private SpriteBatch _spriteBatch;
        private Texture2D _grassTexture;

        private LEM lem = new LEM();

        private const float Boundary = 1600.0f;
        private Model _lemModel;
        private readonly Vector3 _cameraPosition = new Vector3(0.0f, 00.0f, 10.0f);
        private float _cameraHorizontalAngle;
        private float _cameraVerticalAngle;
        private Effect _textureEffect;
        private EffectParameter _textureEffectWvp;
        private EffectParameter _textureEffectImage;
        private const float CameraRps = MathHelper.TwoPi;


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
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _grassTexture = Content.Load<Texture2D>("Images\\grass");
            _lemModel = Content.Load<Model>("models\\LEM\\LEM");
            lem.Init(new Vector3(0,1.78f,0), _lemModel);
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

            lem.Update(delta, gpState);

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

            var camera = Vector3.Transform(_cameraPosition - lem.Position, Matrix.CreateFromAxisAngle(Vector3.UnitX, _cameraHorizontalAngle)) + lem.Position;
            camera = Vector3.Transform(camera - lem.Position, Matrix.CreateFromAxisAngle(Vector3.UnitY, _cameraVerticalAngle)) + lem.Position;
            // Draw the model. A model can have multiple meshes, so loop.
            var look = Matrix.CreateLookAt(camera, lem.Position, Vector3.Up);
            var projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45.0f), _graphics.GraphicsDevice.Viewport.AspectRatio,
                        1.0f, 10000.0f);
            lem.Draw(look, projection);
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

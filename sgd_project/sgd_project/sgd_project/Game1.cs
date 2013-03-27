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

        private LEM lem = new LEM();

        private Model _lemModel;
        private readonly Vector3 _cameraPosition = new Vector3(0.0f, 00.0f, 10.0f);
        private float _cameraHorizontalAngle;
        private float _cameraVerticalAngle;
        private const float _cameraRPS = MathHelper.TwoPi;

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

            _lemModel = Content.Load<Model>("models\\LEM\\LEM");
            lem.Init(Vector3.Zero, _lemModel);
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

            _cameraHorizontalAngle += delta / 1000.0f * gpState.ThumbSticks.Right.Y * MathHelper.TwoPi;
            _cameraHorizontalAngle = MathHelper.Clamp(_cameraHorizontalAngle, -MathHelper.PiOver2 * .95f, MathHelper.PiOver2 * .95f);
            _cameraVerticalAngle -= delta / 1000.0f * gpState.ThumbSticks.Right.X * MathHelper.TwoPi;

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
            base.Draw(gameTime);
        }
    }
}

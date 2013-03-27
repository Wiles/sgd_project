using System;
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

        private Model _lemModel;
        private const float ModelRotation = 0.0f;
        readonly Vector3 _modelPosition = Vector3.Zero;
        private Vector3 _cameraPosition = new Vector3(0.0f, 00.0f, 10.0f);
        private float _cameraHorizontalAngle;
        private float _cameraVerticalAngle;

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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();
            
            _cameraHorizontalAngle += GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y / 10.0f;
            _cameraHorizontalAngle = MathHelper.Clamp(_cameraHorizontalAngle, -MathHelper.PiOver2 * .95f, MathHelper.PiOver2 * .95f);
            _cameraVerticalAngle -= GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X / 10.0f;

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
            var transforms = new Matrix[_lemModel.Bones.Count];
            _lemModel.CopyAbsoluteBoneTransformsTo(transforms);

            var camera = Vector3.Transform(_cameraPosition - _modelPosition, Matrix.CreateFromAxisAngle(Vector3.UnitX, _cameraHorizontalAngle)) + _modelPosition;
            camera = Vector3.Transform(camera - _modelPosition, Matrix.CreateFromAxisAngle(Vector3.UnitY, _cameraVerticalAngle)) + _modelPosition;
            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in _lemModel.Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] *
                        Matrix.CreateRotationY(ModelRotation)
                        * Matrix.CreateTranslation(_modelPosition);
                    effect.View = Matrix.CreateLookAt(camera,
                        _modelPosition, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45.0f), _graphics.GraphicsDevice.Viewport.AspectRatio,
                        1.0f, 10000.0f);
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
            base.Draw(gameTime);
        }
    }
}

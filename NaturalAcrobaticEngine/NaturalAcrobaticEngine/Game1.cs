using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace NaturalAcrobaticEngine
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D ball;
        Vector2 ballPosition;
        Vector2 ballVelocity;
        KeyboardState kstate;
        KeyboardState pstate;// Previous state used to fix keyboard bounce problem
        const float grav = 500f;
        const float jumpVelocity = -300f;
        const float movementVelocity = -200f;
        // consider creating an ENUM as a way to track player states (grounded, falling, running, crouching, or whatever other states we want)
        bool grounded = false;
        bool falling = true;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }


        protected override void Initialize()
        {
            ballPosition = new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2);
            ballVelocity = new Vector2();
            kstate = Keyboard.GetState();// intializing keyboard states instead of reinitializing during every update call
            pstate = Keyboard.GetState();
            base.Initialize();
        }

 
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            ball = Content.Load<Texture2D>("ball");
        }


        protected override void UnloadContent()
        {

        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            kstate = Keyboard.GetState();

            // check to see if we're falling
            falling = ballVelocity.Y > 0;

            // TODO: figure out how to do the "keyboard bounce" thing malloy talked about because right now if you hold space you jump as soon as physically possible and i don't like that :P
            // Fixed Babe
            if (grounded && kstate.IsKeyDown(Keys.Space) && pstate.IsKeyUp(Keys.Space))
            {
                ballVelocity.Y = jumpVelocity;
                grounded = false;
            }
            // Moving Left, TODO: trying to make the character have a little slowdown after letting go
            if (kstate.IsKeyDown(Keys.A))
            {
                ballVelocity.X = movementVelocity;
            }
            else
            {
                ballVelocity.X *= .99f;//slows down ball after letting go, used for ice physics and Luigi type walking
            }
            // Moving Right, TODO: trying to make the character have a little slowdown after letting 
            if (kstate.IsKeyDown(Keys.D))
            {
                ballVelocity.X = -movementVelocity;
            }
            else
            {
                ballVelocity.X *= .99f;
            }
            // r = d  / t
            // d = r * t

            // if the ball is above the bottom of the window we apply gravity else we are grounded
            if (ballPosition.Y < graphics.PreferredBackBufferHeight - ball.Height / 2) {
                grounded = false;
                // multiply gravity by 3.5f if falling to have more "weight"
                ballVelocity.Y += falling ? 3.5f * grav * (float)gameTime.ElapsedGameTime.TotalSeconds : grav * (float)gameTime.ElapsedGameTime.TotalSeconds;
            } else {
                grounded = true;
            }

            // actually updates the position of the ball based on the Y velocity.
            ballPosition.Y += ballVelocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Moving the ball on the X axis based on the current X velocity
            ballPosition.X += ballVelocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // bounds checking and keeping inside box
            ballPosition.Y = MathHelper.Min(MathHelper.Max(ball.Height / 2, ballPosition.Y), graphics.PreferredBackBufferHeight - ball.Height / 2); // no falling out of the bottom and no going thru the roof
            ballPosition.X = MathHelper.Min(MathHelper.Max(ball.Width / 2, ballPosition.X), graphics.PreferredBackBufferWidth - ball.Width / 2);

            pstate = Keyboard.GetState(); //saves the keyboards state after the cycle, before next state is made

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(ball,
                ballPosition,
                null,
                Color.White,
                0f,
                new Vector2(ball.Width / 2, ball.Height /2),
                Vector2.One,
                SpriteEffects.None,
                0f);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

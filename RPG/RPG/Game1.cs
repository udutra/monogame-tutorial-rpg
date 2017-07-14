using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace RPG
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D text;
        RenderTarget2D rt;
        Point virtDim, WinDim;
        Vector2 scale, rtPos;
        bool isReleased;
        MagicTexture test;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            virtDim = new Point(1920, 1080);
            WinDim = new Point(960, 540);
            rt = new RenderTarget2D(GraphicsDevice, virtDim.X, virtDim.Y);
            base.Initialize();

            ResizeWindow();
            CalcScale();
        }

        protected override void LoadContent()
        {
            //Create a new Spritebatch, wich can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            text = Content.Load<Texture2D>("Imagens/grad");
            test = new MagicTexture(text, new Rectangle(0, 0, 100, 100), 10, 0.1f, 0);
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            test.Update(gameTime);
            if (Keyboard.GetState().IsKeyDown(Keys.U) && isReleased)
            {
                WinDim.X = GraphicsDevice.DisplayMode.Width;
                WinDim.X = GraphicsDevice.DisplayMode.Height;
                graphics.ToggleFullScreen();
                isReleased = false;
                ResizeWindow();
                CalcScale();
            }
            if (Keyboard.GetState().IsKeyUp(Keys.U))
            {
                isReleased = true;
            }
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //draw on the target
            GraphicsDevice.SetRenderTarget(rt);
            spriteBatch.Begin();
            test.Draw(spriteBatch, Vector2.Zero);
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);

            //draw the target
            spriteBatch.Begin();
            spriteBatch.Draw(rt, rtPos, null, scale:scale);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void CalcScale()
        {
            float scaleX = (float)WinDim.X / virtDim.X;
            float scaleY = (float)WinDim.Y / virtDim.Y;

            if(scaleX > scaleY)
            {
                scale = new Vector2(scaleY);
                rtPos = new Vector2((WinDim.X - virtDim.X * scaleY)/2, 0);
            }
            else
            {
                scale = new Vector2(scaleX);
                rtPos = new Vector2(0,(WinDim.Y - virtDim.Y * scaleX) / 2);
            }
        }

        public void ResizeWindow()
        {
            graphics.PreferredBackBufferHeight = WinDim.Y;
            graphics.PreferredBackBufferWidth = WinDim.X;
            graphics.ApplyChanges();
        }
    }
}

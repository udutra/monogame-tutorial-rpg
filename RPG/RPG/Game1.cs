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
        Vector2 scale, rtPos, translation;
        bool isReleased;
        Tile[,] tiles;
        MagicTexture test;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            translation = new Vector2(-400, -400);
            virtDim = new Point(1920, 1080);
            WinDim = new Point(960, 540);
            rt = new RenderTarget2D(GraphicsDevice, virtDim.X, virtDim.Y);
            base.Initialize();

            ResizeWindow();
            CalcScale();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            text = Content.Load<Texture2D>("Imagens/tile");
            tiles = new Tile[10, 10];
            test = new MagicTexture(text, new Rectangle(0,0,text.Width, text.Height), Facing.N);
            for(int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    tiles[x, y] = new Tile(test, new Vector2(x * 100 - y * 100, x * 50 + y * 50));
                }
            }

        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            test.Update(gameTime);
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            KeyboardState kbs = Keyboard.GetState();
            if (kbs.IsKeyDown(Keys.Up)){
                translation.Y += 1;
            }
            if (kbs.IsKeyDown(Keys.Down))
            {
                translation.Y -= 1;
            }
            if (kbs.IsKeyDown(Keys.Left))
            {
                translation.X += 1;
            }
            if (kbs.IsKeyDown(Keys.Right))
            {
                translation.X -= 1;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //draw on the target
            Matrix translator = Matrix.CreateTranslation(translation.X, translation.Y,0);
            GraphicsDevice.SetRenderTarget(rt);
            spriteBatch.Begin(transformMatrix: translator);
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    tiles[x, y].Draw(spriteBatch);
                }
            }
            spriteBatch.End();
            GraphicsDevice.SetRenderTarget(null);

            //draw the target
            Matrix scaler = Matrix.CreateScale(scale.X, scale.Y, 0);
            spriteBatch.Begin(transformMatrix:scaler);
            spriteBatch.Draw(rt, rtPos, null);
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
/*
 * if (Keyboard.GetState().IsKeyDown(Keys.U) && isReleased)
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
*/
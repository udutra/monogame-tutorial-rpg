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
        Vector2 scale, rtPos, translation, mousePos;
        Tileset ts;
        float translationSpeed;
        KeyboardState kbs;
        MagicTexture cursor;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            translation = new Vector2(-400, -400);
            translationSpeed = 5f;
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
            cursor = new MagicTexture(Content.Load<Texture2D>("Imagens/cursor"), new Rectangle(0,0,100,100),Facing.L);
            Tile[,] tiles = new Tile[10, 10];
            MagicTexture test = new MagicTexture(text, new Rectangle(0,0,text.Width, text.Height), Facing.N);
            for(int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    tiles[x, y] = new Tile(test, new Vector2(x * 100 - y * 100, x * 50 + y * 50));
                }
            }
            ts = new Tileset(tiles, 10,10,200,100);
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            kbs = Keyboard.GetState();
            UpdateMousePos();
            MouseAct();
            UpdateTranslation();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            
            

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //draw on the target
            Matrix translator = Matrix.CreateTranslation(translation.X, translation.Y,0);
            GraphicsDevice.SetRenderTarget(rt);
            spriteBatch.Begin(transformMatrix: translator);

            ts.Draw(spriteBatch);

            cursor.Draw(spriteBatch, mousePos);
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

        public void UpdateMousePos()
        {
            Vector2 originalPos = Mouse.GetState().Position.ToVector2();
            originalPos.X *= 1/scale.X;
            originalPos.Y *= 1/scale.Y;
            mousePos = originalPos;
            mousePos += translation * -1;
            
        }

        public void MouseAct()
        {

        }

        public void UpdateTranslation()
        {
            if (kbs.IsKeyDown(Keys.Up))
            {
                translation.Y += translationSpeed;
            }
            if (kbs.IsKeyDown(Keys.Down))
            {
                translation.Y -= translationSpeed;
            }
            if (kbs.IsKeyDown(Keys.Left))
            {
                translation.X += translationSpeed;
            }
            if (kbs.IsKeyDown(Keys.Right))
            {
                translation.X -= translationSpeed;
            }
        }
    }
}



/*
 * bool isReleased;
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

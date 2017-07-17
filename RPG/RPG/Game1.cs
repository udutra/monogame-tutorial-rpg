using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Xml.Linq;
using System.Collections.Generic;

namespace RPG
{
    enum GameMode {  TilesetEditor, Game, Menus }

    public class Game1 : Game
    {
        GameMode gm;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D tex;
        RenderTarget2D rt;
        Point virtDim, WinDim;
        Vector2 scale, rtPos, translation, mousePos;
        Tileset ts;
        float translationSpeed;
        KeyboardState kbs;
        MagicTexture cursor;
        bool isReleased;
        Button b;

        public Game1()
        {
            gm = GameMode.Menus;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
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
            cursor = new MagicTexture(Content.Load<Texture2D>("Imagens/cursor"), new Rectangle(0, 0, 100, 100), Facing.L);
            tex = Content.Load<Texture2D>("Imagens/grad");
            MagicTexture test2 = new MagicTexture(tex, new Rectangle(0, 0, tex.Width, tex.Height), Facing.N);
            b = new Button(test2, new Vector2(300, 100), "TilesetEditor");
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            kbs = Keyboard.GetState();
            UpdateMouse();

            switch (gm)
            {
                case (GameMode.TilesetEditor):
                    {
                        UpdateEditor(gameTime);
                        break;
                    }
                case (GameMode.Game):
                    {
                        UpdateGame(gameTime);
                        break;
                    }
                case (GameMode.Menus):
                    {
                        UpdateMenus(gameTime);
                        break;
                    }
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
            Matrix translator = Matrix.CreateTranslation(translation.X, translation.Y, 0);
            GraphicsDevice.SetRenderTarget(rt);
            spriteBatch.Begin(transformMatrix: translator);

            switch (gm)
            {
                case (GameMode.TilesetEditor):
                    {
                        DrawEditor();
                        break;
                    }
                case (GameMode.Game):
                    {
                        DrawGame();
                        break;
                    }
                case (GameMode.Menus):
                    {
                        DrawMenus();
                        break;
                    }
            }

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

        public void UpdateMouse()
        {
            Vector2 originalPos = Mouse.GetState().Position.ToVector2();
            originalPos.X *= 1/scale.X;
            originalPos.Y *= 1/scale.Y;
            mousePos = originalPos;
            mousePos += translation * -1;
            if(Mouse.GetState().LeftButton == ButtonState.Released)
            {
                isReleased = true;
            }
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

        public void UpdateEditor(GameTime gt_)
        {
            UpdateTranslation();
        }

        public void UpdateGame(GameTime gt_)
        {

        }

        public void UpdateMenus(GameTime gt_)
        {
            if (b.GetFrame().Contains(mousePos) && IsClicking())
            {
                gm = GameMode.TilesetEditor;
                SetupTSE();
            }
        }

        public void DrawEditor()
        {
            ts.Draw(spriteBatch);
        }

        public void SetupTSE()
        {
            tex = Content.Load<Texture2D>("Imagens/tile");

            ts = GetTileSet();
        }
        
        private void DrawGame()
        {
            throw new NotImplementedException();
        }

        private void DrawMenus()
        {
            b.Draw(spriteBatch);
        }

        public bool IsClicking()
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && isReleased)
            {
                return true;
                isReleased = false;
            }
            return false;
        }

        public Tileset GetTileSet()
        {
            XDocument doc = XDocument.Load("Content/XML/TestTileset.xml");
            int tx = int.Parse(doc.Element("Tileset").Attribute("x").Value);
            int ty = int.Parse(doc.Element("Tileset").Attribute("y").Value);

            Tile[,] tiles2 = new Tile[tx, ty];
            foreach (XElement tile in doc.Element("Tileset").Elements("Tile"))
            {
                MagicTexture ttt = new MagicTexture(Content.Load<Texture2D>(tile.Value), new Rectangle(0,0,200,100), Facing.N);
                int x = int.Parse(tile.Attribute("x").Value);
                int y = int.Parse(tile.Attribute("y").Value);
                tiles2[x, y] = new Tile(ttt, new Vector2(x * 100 - y * 100, x * 50 + y * 50));
            }
            


            
            
            return new Tileset(tiles2, tx, ty,200,100);
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
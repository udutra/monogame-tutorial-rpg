using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Xml.Linq;

namespace RPG
{
    enum GameMode {TilesetEditor, Game, Menus}
    enum DrawPhase { Trans, NonTrans}
    public class Game1 : Game
    {
        private GameMode gm;
        private DrawPhase dPhase;
        private Keys k_SaveTileset;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D tex;
        private RenderTarget2D rt, nonTransRt;
        private Point virtDim, WinDim;
        private Vector2 scale, rtPos, translation, mousePos;
        private Tile currentTile;
        private Tileset ts;
        private Button b;
        private KeyboardState kbs;
        private MagicTexture cursor;
        private float translationSpeed;
        private bool isReleased, p_SaveTileset, pb_SaveTileset;
        private String[] tileNames;
        private int tileIndex;


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
            nonTransRt = new RenderTarget2D(GraphicsDevice, virtDim.X, virtDim.Y);
            base.Initialize();

            SetupKeys();
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
            rt.Dispose();
            nonTransRt.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            kbs = Keyboard.GetState();
            UpdateKeys();
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
            dPhase = DrawPhase.Trans;

            //draw translated stuff on the target
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

            spriteBatch.End();

            dPhase = DrawPhase.NonTrans;
            GraphicsDevice.SetRenderTarget(nonTransRt);
            GraphicsDevice.Clear(Color.TransparentBlack);
            spriteBatch.Begin();

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
            
            //draw overlay
            GraphicsDevice.SetRenderTarget(null);
            
            //draw the non translated target
            Matrix scaler = Matrix.CreateScale(scale.X, scale.Y, 0);
            spriteBatch.Begin(transformMatrix: scaler);
            spriteBatch.Draw(rt, rtPos, null);
            spriteBatch.Draw(nonTransRt, rtPos, null);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void CalcScale()
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

        private void ResizeWindow()
        {
            graphics.PreferredBackBufferHeight = WinDim.Y;
            graphics.PreferredBackBufferWidth = WinDim.X;
            graphics.ApplyChanges();
        }

        private void UpdateMouse()
        {
            Vector2 originalPos = Mouse.GetState().Position.ToVector2();
            originalPos.X *= 1/scale.X;
            originalPos.Y *= 1/scale.Y;
            mousePos = originalPos;
            //mousePos += translation * -1;
            if(Mouse.GetState().LeftButton == ButtonState.Released)
            {
                isReleased = true;
            }
        }

        private void UpdateTranslation()
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

        private void UpdateEditor(GameTime gt_)
        {
            UpdateTranslation();
            if(p_SaveTileset)
            {
                //save
            }
            if (IsClicking())
            {
                bool switched = false;
                if (currentTile.GetFrame().Contains(mousePos)){
                    ToggleSelectedTile();
                    switched = true;
                }

                if (!switched)
                {
                    Point closest = new Point(0, 0);
                    float closestDist = 1000;

                    for (int x = 0; x < ts.GetWidth(); x++)
                    {
                        for (int y = 0; y < ts.GetHeight(); y++)
                        {
                            float dist = Vector2.Distance(GetMousePos(), ts.GetTiles()[x, y].GetMiddle());

                            if (dist < closestDist)
                            {
                                closestDist = dist;
                                closest = new Point(x, y);
                            }
                        }
                    }
                    if (ts.GetTiles()[closest.X, closest.Y].GetFrame().Contains(GetMousePos()))
                    {
                        ts.GetTiles()[closest.X, closest.Y] = GetTile(tileNames[tileIndex]);
                        ts.PlaceTiles();
                    }
                }
            }
        }

        private void UpdateGame(GameTime gt_)
        {

        }

        private void UpdateMenus(GameTime gt_)
        {
            if (b.GetFrame().Contains(GetMousePos()) && IsClicking())
            {
                gm = GameMode.TilesetEditor;
                SetupTSE();
            }
        }

        private void SetupTSE()
        {
            tex = Content.Load<Texture2D>("Imagens/tile");
            tileIndex = 0;
            tileNames = new string[] { "Imagens/tile", "Imagens/tile2" };
            currentTile = GetTile(tileNames[tileIndex]);
            ts = GetTileSet();
        }

        private void DrawEditor()
        {
            if (dPhase == DrawPhase.Trans)
            {
                ts.Draw(spriteBatch);
            }
            else{
                currentTile.Draw(spriteBatch);
            }
        }

        private void DrawGame()
        {
            if (dPhase == DrawPhase.Trans)
            {
                
            }
            else
            {
                
            }
        }

        private void DrawMenus()
        {
            if (dPhase == DrawPhase.Trans)
            {
                
            }
            else
            {
                b.Draw(spriteBatch);
            }
            
        }

        private bool IsClicking()
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed && isReleased)
            {
                isReleased = false;
                return true;
            }
            return false;
        }

        private Tileset GetTileSet()
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
                tiles2[x, y] = new Tile(ttt, Vector2.Zero);
            }

            return new Tileset(tiles2, tx, ty, 200, 100);
        }

        private void SetupKeys()
        {
            k_SaveTileset = Keys.S;
        }

        private void UpdateKeys()
        {
            if (kbs.IsKeyUp(k_SaveTileset))
            {
                pb_SaveTileset = false;
            }

            if (kbs.IsKeyDown(k_SaveTileset) && !pb_SaveTileset)
            {
                pb_SaveTileset = true;
                p_SaveTileset = true;
            }
            
            else if (kbs.IsKeyDown(k_SaveTileset))
            {
                p_SaveTileset = false;
            }
        }

        public Tile GetTile(string tileName_)
        {
            MagicTexture ttt = new MagicTexture(Content.Load<Texture2D>(tileName_), new Rectangle(0, 0, 200, 100), Facing.N);
            return new Tile(ttt, Vector2.Zero);
        }

        private Vector2 GetMousePos()
        {
            return mousePos + translation * -1;
        }

        public void ToggleSelectedTile()
        {
            tileIndex++;
            if(tileIndex >= tileNames.Length)
            {
                tileIndex = 0;
            }
            currentTile = GetTile(tileNames[tileIndex]);
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
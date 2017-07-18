using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RPG
{
    public class Tileset
    {

        private Tile[,] tiles;
        private int width, height, tw, th;
        
        public Tileset(Tile[,] tiles_, int width_, int height_, int tx_, int ty_)
        {
            tiles = tiles_;
            width = width_;
            height = height_;
            tw = tx_;
            th = ty_;
            PlaceTiles();
        }

        public void Draw(SpriteBatch sb_)
        {
            for(int x = 0; x < width; x++){
                for (int y = 0; y < height; y++)
                {
                    tiles[x, y].Draw(sb_);
                }
            }
        }

        public void PlaceTiles()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tiles[x, y].SetPos(new Vector2(x * 100 - y * 100, x * 50 + y * 50));
                }
            }
        }

        public int GetWidth()
        {
            return width;
        }

        public int GetHeight()
        {
            return height;
        }

        public Tile[,] GetTiles()
        {
            return tiles;
        }
    }
}
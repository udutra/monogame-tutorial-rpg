using Microsoft.Xna.Framework.Graphics;

namespace RPG
{
    public class Tileset
    {

        public Tile[,] tiles;
        int width, height, tw, th;
        
        public Tileset(Tile[,] tiles_, int width_, int height_, int tx_, int ty_)
        {
            tiles = tiles_;
            width = width_;
            height = height_;
            tw = tx_;
            th = ty_;
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

    }
}
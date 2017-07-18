using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG
{
    public class Sprite
    {

        private  MagicTexture tex;
        private Vector2 pos;

        public Sprite(MagicTexture tex_, Vector2 pos_)
        {
            tex = tex_;
            pos = pos_;
        }

        public void Draw(SpriteBatch sb_)
        {
            tex.Draw(sb_, pos);
        }

        public void Update(GameTime gt_)
        {
            tex.Update(gt_);
        }

        public Rectangle GetFrame()
        {
            Rectangle rec = tex.GetFrame();
            rec.X += (int) pos.X;
            rec.Y += (int) pos.Y;

            return rec;
        }

        public Vector2 GetMiddle()
        {
            return (tex.GetMiddle() + pos);
        }

        public void SetPos(Vector2 pos_)
        {
            pos = pos_;
        }
    }
}
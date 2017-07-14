using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace RPG
{
    public enum Facing {L, R, N};


    public class MagicTexture
    {
        Facing face;
        Texture2D source;
        Rectangle sourceRect, frame;
        int frameCount, frameCounter;
        float frameTime, frameTimer;
        

        public MagicTexture(Texture2D source_, Rectangle sourceRect_, Facing face_)
        {
            source = source_;
            sourceRect = sourceRect_;
            face = face_;
        }

        public MagicTexture(Texture2D source_, Rectangle sourceRect_, Facing face_, int frameCount_, float frameTime_, float delay_)
        {
            source = source_;
            sourceRect = sourceRect_;
            frameCount = frameCount_;
            frameTime = frameTime_;
            frameTimer += delay_;
            face = face_;
        }
        
        public void Update(GameTime gt_)
        {
            frameTimer -= (float) gt_.ElapsedGameTime.TotalSeconds;
            if(frameTimer < 0)
            {
                frameTimer = frameTime;
                frameCounter++;
            }
            if(frameCounter >= frameCount)
            {
                frameCounter = 0;
            }

        }

        public void Draw(SpriteBatch sb_, Vector2 pos_)
        {
            //calcule the correct rect
            frame = new Rectangle(sourceRect.X + frameCounter * sourceRect.Width, sourceRect.Y, sourceRect.Width, sourceRect.Height);
            
            sb_.Draw(source, sourceRectangle:frame, position:pos_);
        }
    }
}

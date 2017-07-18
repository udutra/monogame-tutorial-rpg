using Microsoft.Xna.Framework;

namespace RPG
{
    class Button : Sprite
    {
        private string action;

        public Button(MagicTexture tex_, Vector2 pos_, string action_) : base(tex_, pos_)
        {
            action = action_;
        }
    }
}

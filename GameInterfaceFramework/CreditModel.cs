using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameInterfaceFramework
{
    public class CreditModel
    {
        public SpriteFont TitleFont
        {
            get;
            private set;
        }

        public SpriteFont NameFont
        {
            get;
            private set;
        }

        public CreditModel(SpriteFont title, SpriteFont name)
        {
            TitleFont = title;
            NameFont = name;
        }
    }
}

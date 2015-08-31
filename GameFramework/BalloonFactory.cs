using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using GameCore;

namespace GameFramework
{
    public class BalloonFactory
    {
        private const int START_HEALTH_RED = 3;
        private const int START_HEALTH_GREEN = 1;
        private const int START_HEALTH_BLUE = 2;

        private BalloonModel _redModel;
        private BalloonModel _greenModel;
        private BalloonModel _blueModel;

        public BalloonFactory() { }

        public void Initialize()
        {
            ResourceManager manager = ResourceManager.Resources;
            
            Animation redMove = manager.GetAnimation("redmove");
            Animation greenMove = manager.GetAnimation("greenmove");
            Animation blueMove = manager.GetAnimation("bluemove");

            Animation popAnimation = manager.GetAnimation("popmove");
            Animation hitAnimation = manager.GetAnimation("hitmove");

            SoundEffect popSound = manager.GetSoundEffect("pop");

            Vector2 redVelocity = new Vector2(0, -8f);
            Vector2 greenVelocity = new Vector2(0, -5f);
            Vector2 blueVelocity = new Vector2(0, -6.5f);

            _redModel = new BalloonModel(redMove, popAnimation, hitAnimation, popSound, ref redVelocity);
            _greenModel = new BalloonModel(greenMove, popAnimation, hitAnimation, popSound, ref greenVelocity);
            _blueModel = new BalloonModel(blueMove, popAnimation, hitAnimation, popSound, ref blueVelocity);
        }

        /// <summary>
        /// This will create a new balloon on the heap! Use if
        /// no other resource is available.
        /// </summary>
        /// <param name="color">Color of the balloon to make.</param>
        /// <param name="position">The position of the balloon on the screen.</param>
        /// <returns>A new balloon initialized to the type of the color parameter.</returns>
        public Balloon MakeBalloon(BalloonColor color, ref Vector2 position)
        {
            Balloon make = new Balloon();

            MakeBalloon(color, ref position, ref make);

            return make;
        }

        /// <summary>
        /// Creates a balloon using an already instantiated object.
        /// </summary>
        /// <param name="color">Color the balloon should be made into.</param>
        /// <param name="position">The position of the balloon on the screen.</param>
        /// <param name="make">An instantiated balloon.</param>
        /// <returns>Balloon made into the color desired.</returns>
        public void MakeBalloon(BalloonColor color, ref Vector2 position, ref Balloon make)
        {
            make.Color = color;
            BalloonModel model = GetModel(color);

            switch (make.Color)
            {
                case BalloonColor.Red:
                    make.Initialize(model, ref position, START_HEALTH_RED);
                    break;
                case BalloonColor.Blue:
                    make.Initialize(model, ref position, START_HEALTH_BLUE);
                    break;
                case BalloonColor.Green:
                default:
                    make.Initialize(model, ref position, START_HEALTH_GREEN);
                    break;
            }
        }

        public void RehydrateBalloon(XElement balloonElement, ref Balloon rehydrate)
        {
            BalloonColor temp = BalloonColor.Green;
            BalloonColor color = (BalloonColor)Enum.Parse(temp.GetType(), balloonElement.Attribute("Color").Value, false);
            BalloonModel model = GetModel(color);

            rehydrate.Color = color;
            rehydrate.Rehydrate(balloonElement, model);
        }

        private BalloonModel GetModel(BalloonColor color)
        {
            BalloonModel model;

            switch (color)
            {
                case BalloonColor.Red:
                    model = _redModel;
                    break;
                case BalloonColor.Blue:
                    model = _blueModel;
                    break;
                case BalloonColor.Green:
                default:
                    model = _greenModel;
                    break;
            }

            return model;
        }
    }
}

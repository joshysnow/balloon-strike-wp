using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using GameCore;

namespace GameFramework
{
    public class BalloonFactory
    {
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

            _redModel = new BalloonModel(redMove, popAnimation, hitAnimation, popSound, ref redVelocity, 3);
            _greenModel = new BalloonModel(greenMove, popAnimation, hitAnimation, popSound, ref greenVelocity, 1);
            _blueModel = new BalloonModel(blueMove, popAnimation, hitAnimation, popSound, ref blueVelocity, 2);
        }

        /// <summary>
        /// This will create a new balloon on the heap! Use if
        /// no other resource is available.
        /// </summary>
        /// <param name="color">Color of the balloon to make.</param>
        /// <returns>A new balloon initialized to the type of the color parameter.</returns>
        public Balloon MakeBalloon(BalloonColor color)
        {
            Balloon make = new Balloon();
            make.Color = color;

            MakeBalloon(color, ref make);

            return make;
        }

        /// <summary>
        /// Creates a balloon using an already instantiated object.
        /// </summary>
        /// <param name="color">Color the balloon should be made into.</param>
        /// <param name="make">An instantiated balloon.</param>
        /// <returns>Balloon made into the color desired.</returns>
        public Balloon MakeBalloon(BalloonColor color, ref Balloon make)
        {
            switch (make.Color)
            {
                case BalloonColor.Red:
                    make.Initialize(_redModel);
                    break;
                case BalloonColor.Blue:
                    make.Initialize(_blueModel);
                    break;
                case BalloonColor.Green:
                default:
                    make.Initialize(_greenModel);
                    break;
            }

            return make;
        }
    }
}

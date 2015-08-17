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
            make.Color = color;

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
        public Balloon MakeBalloon(BalloonColor color, ref Vector2 position, ref Balloon make)
        {
            switch (make.Color)
            {
                case BalloonColor.Red:
                    make.Initialize(_redModel, ref position, 3);
                    break;
                case BalloonColor.Blue:
                    make.Initialize(_blueModel, ref position, 2);
                    break;
                case BalloonColor.Green:
                default:
                    make.Initialize(_greenModel, ref position, 1);
                    break;
            }

            return make;
        }
    }
}

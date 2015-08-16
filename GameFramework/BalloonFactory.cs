using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using GameCore;

namespace GameFramework
{
    public class BalloonFactory
    {
        private Animation _greenMove;
        private Animation _blueMove;
        private Animation _redMove;
        private Animation _popAnimation;
        private Animation _hitAnimation;
        private SoundEffect _popSound;

        private Vector2 _greenVelocity;
        private Vector2 _blueVelocity;
        private Vector2 _redVelocity;

        public BalloonFactory() { }

        public void Initialize()
        {
            ResourceManager manager = ResourceManager.Resources;

            _greenMove = manager.GetAnimation("greenmove");
            _blueMove = manager.GetAnimation("redmove");
            _redMove = manager.GetAnimation("bluemove");

            _popAnimation = manager.GetAnimation("popmove");
            _hitAnimation = manager.GetAnimation("hitmove");

            _popSound = manager.GetSoundEffect("pop");

            _redVelocity = new Vector2(0, -8f);
            _greenVelocity = new Vector2(0, -5f);
            _blueVelocity = new Vector2(0, -6.5f);
        }

        public Balloon MakeBalloon(BalloonColor color)
        {
            Balloon make = new Balloon();
            make.Color = color;

            MakeBalloon(make);

            return make;
        }

        public Balloon MakeBalloon(Balloon make)
        {
            Animation move;
            Vector2 velocity;
            float health;

            switch (make.Color)
            {
                case BalloonColor.Red:
                    health = 3;
                    move = _redMove;
                    velocity = _redVelocity;
                    break;
                case BalloonColor.Blue:
                    health = 2;
                    move = _blueMove;
                    velocity = _blueVelocity;
                    break;
                case BalloonColor.Green:
                default:
                    health = 1;
                    move = _greenMove;
                    velocity = _greenVelocity;
                    break;
            }

            make.Initialize(move, _hitAnimation, _popAnimation, _popSound, Vector2.Zero, velocity, health);

            return make;
        }
    }
}

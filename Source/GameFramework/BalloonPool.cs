using GameCore.Memory;

namespace GameFramework
{
    public class BalloonPool : ObjectPool<Balloon>
    {
        public BalloonPool(int max) : base(max) { }

        public void Fill()
        {
            if (Full())
            {
                return;
            }

            byte index = 0;
            while (index < Max())
            {
                Push(new Balloon());
                index++;
            }
        }
    }
}

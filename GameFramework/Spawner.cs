using System.Xml.Linq;
using GameCore.Timers;

namespace GameFramework
{
    public delegate void SpawnHandler(Character type);

    public class Spawner
    {
        public event SpawnHandler Spawn;

        private Character _prototype;
        private SimpleTimer _timer;

        public Spawner(SimpleTimer timer, Character prototype)
        {
            _timer = timer;
            _prototype = prototype;

            _timer.Elapsed += TimerElapsedHandler;
        }

        /// <summary>
        /// Serializes the spawner to an XML element. The root element
        /// encompasses a timer element with attributes to reconstruct it.
        /// Recommend adding the characters type as an attribute to root
        /// element "Spawner".
        /// </summary>
        /// <returns>A spawner XML element.</returns>
        public XElement Serialize()
        {
            XElement root = new XElement("Spawner");

            XElement xTimer = new XElement("Timer",
                new XAttribute("ElapseTime", _timer.ElapseTime),
                new XAttribute("TimePassed", _timer.TimePassed)
                );

            if (_timer is VariableTimer)
            {
                VariableTimer vTimer = (VariableTimer)_timer;
                xTimer.Add(
                    new XAttribute("Type", "Variable"),
                    new XAttribute("Modifier", vTimer.Modifier),
                    new XAttribute("Bounds", vTimer.Bounds)
                    );
            }
            else
            {
                xTimer.Add(new XAttribute("Type", "Simple"));
            }

            root.Add(xTimer);

            return root;
        }

        private void TimerElapsedHandler(SimpleTimer timer)
        {
            if (Spawn != null)
                Spawn(_prototype);
        }
    }
}

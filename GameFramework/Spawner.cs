using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using GameCore.Timers;

namespace GameFramework
{
    public delegate void SpawnHandler(Spawner sender, object prototype);

    public class Spawner
    {
        public event SpawnHandler Spawn;

        private SimpleTimer _timer;
        private TimeCounter _counter;
        private object _prototype;
        private bool _spawning = false;

        public Spawner(SimpleTimer timer, object prototype, float startTime = 0)
        {
            _timer = timer;
            _prototype = prototype;

            // Always create the object anyways, incase it is tested later.
            _counter = new TimeCounter(TimeSpan.FromMilliseconds(startTime));

            // Subscriber to timers elapse event, will use this to trigger a spawn.
            _timer.Elapsed += TimerElapsedHandler;
        }

        public void Update(GameTime gameTime)
        {
            // Checked for efficency, always means if timer elapse flag is set
            // then we will fire on this update call.
            if (_counter.Elapsed == false)
                UpdateCounter(gameTime);

            if (_spawning)
                _timer.Update(gameTime);
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

            // TODO: Move into timer class! Would eliminate the horrible if statement below
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

        private void UpdateCounter(GameTime gameTime)
        {
            if (_counter != null && !_counter.Elapsed)
            {
                _counter.Update(gameTime);                

                // Update flag with latest value.
                _spawning = _counter.Elapsed;
            }
        }

        private void TimerElapsedHandler(SimpleTimer timer)
        {
            RaiseSpawn();
        }

        private void RaiseSpawn()
        {
            if (Spawn != null)
                Spawn(this, _prototype);
        }
    }
}

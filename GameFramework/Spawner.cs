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

        private VariableTimer _timer;
        private TimeCounter _counter;
        private object _prototype;
        private bool _spawning = false;

        public Spawner(VariableTimer timer, object prototype, float startTime = 0)
        {
            _timer = timer;
            _prototype = prototype;

            // Always create the object anyways, incase it is tested later.
            _counter = new TimeCounter(TimeSpan.FromMilliseconds(startTime));
        }

        public void Update(GameTime gameTime)
        {
            // Checked for efficency, always means if timer elapse flag is set
            // then we will fire on this update call.
            if (_counter.Elapsed == false)
                UpdateCounter(gameTime);

            if (_spawning)
            {
                if (_timer.Update(gameTime))
                {
                    RaiseSpawn();
                }
            }
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
            XElement xSpawner = new XElement("Spawner",
                new XAttribute("Spawning", _spawning)
                );

            // TODO: Serialize prototype (will have to be character to override dehydrate)

            // Serialize counter.
            xSpawner.Add(_counter.Dehydrate());

            // Serialize timer.
            xSpawner.Add(_timer.Dehydrate());

            return xSpawner;
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

        private void RaiseSpawn()
        {
            if (Spawn != null)
            {
                Spawn(this, _prototype);
            }
        }
    }
}

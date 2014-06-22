#region File Description
// Open Licence
// Foxcode Games (TM)
#endregion

#region Directives
using System;
using Microsoft.Xna.Framework;
#endregion

namespace sandbox1
{
    public delegate void SpawnTimerElapsed();

    public delegate void SpawnTimerExpired(FiniteSpawnTimer spawnTimer);

    /// <summary>
    /// This spawn timer will be able to limit
    /// the amount of times the elapsed event
    /// will trigger.
    /// </summary>
    public class FiniteSpawnTimer : SpawnTimer
    {
        #region Fields

        /// <summary>
        /// True if the spawn timer will
        /// not spawn anymore.
        /// </summary>
        public bool HasExpired
        {
            get;
            private set;
        }

        /// <summary>
        /// How many spawns till the timer
        /// will expire.
        /// </summary>
        public int SpawnCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Raised when the spawn timer has expired.
        /// </summary>
        public event SpawnTimerExpired Expired;

        #endregion

        #region Initialisation

        /// <summary>
        /// Construct a finite spawn timer for a fixed spawn time
        /// limited to the amount of spawns.
        /// </summary>
        /// <param name="spawnTime">Time to spawn.</param>
        /// <param name="spawnCount">The amount of spawns this timer is limited to.</param>
        public FiniteSpawnTimer(TimeSpan spawnTime, int spawnCount)
            : base(spawnTime)
        {
            SpawnCount = spawnCount;
        }

        /// <summary>
        /// Construct a finite spawn timer to a variable time that increases or decreases
        /// between each spawn and is limited to the amount of spawns.
        /// </summary>
        /// <param name="spawnTime">Time to spawn.</param>
        /// <param name="minSpawnTime">Minimum time between each spawn.</param>
        /// <param name="spawnTimeModifier">Percentage to increase or decrease the time for each spawn.</param>
        /// <param name="spawnCount">The amount of spawns this timer is limited to.</param>
        public FiniteSpawnTimer(TimeSpan spawnTime, TimeSpan minSpawnTime, float spawnTimeModifier, int spawnCount)
            : base(spawnTime, minSpawnTime, spawnTimeModifier)
        {
            SpawnCount = spawnCount;
        }

        #endregion

        /// <summary>
        /// When the spawn timer has run out of spawns.
        /// </summary>
        private void OnExpired()
        {
            if (Expired != null)
            {
                Expired(this);
            }
        }

        /// <summary>
        /// Override the event to decrease the spawn count.
        /// </summary>
        protected override sealed void OnSpawnTimerElapsed()
        {
            SpawnCount--;

            base.OnSpawnTimerElapsed();
        }

        /// <summary>
        /// Update the spawn timer if the timer hasn't
        /// expired and deduct the spawn count by one.
        /// </summary>
        /// <param name="gameTime">The time for this frame.</param>
        public override void Update(GameTime gameTime)
        {
            if (SpawnCount <= 0)
            {
                if (!HasExpired)
                {
                    OnExpired();
                    HasExpired = true;
                }

                return;
            }

            base.Update(gameTime);
        }
    }

    /// <summary>
    /// A timer to count up towards a spawn time.
    /// </summary>
    public class SpawnTimer
    {
        #region Fields

        /// <summary>
        /// Time elapsed from last spawn. (ms)
        /// </summary>
        private float _millisecondsElapsed;

        /// <summary>
        /// The amount of time to reduce between spawns. (ms)
        /// </summary>
        private float _spawnTimeModifier;

        /// <summary>
        /// The minimum time between spawns, can be no less
        /// than this value. (ms)
        /// </summary>
        private float _minSpawnTime;

        /// <summary>
        /// The amount of (ms) to spawn when the 
        /// elapsed time reaches this amount or more.
        /// </summary>
        private float _spawnTime;

        /// <summary>
        /// Flag to represent whether or not this spawn time
        /// has reached its minimum limit.
        /// </summary>
        private bool _limitReached = false;

        /// <summary>
        /// An event to be raised when this spawn timer elapses.
        /// </summary>
        public event SpawnTimerElapsed Elapsed;

        #endregion

        #region Initialisation

        /// <summary>
        /// Construct a spawn timer to a fixed time between each spawn.
        /// </summary>
        /// <param name="spawnTime">Time to spawn.</param>
        public SpawnTimer(TimeSpan spawnTime)
        {
            _spawnTime = (float)spawnTime.TotalMilliseconds;
        }

        /// <summary>
        /// Construct a spawn timer to a variable time that increases or decreases
        /// between each spawn.
        /// </summary>
        /// <param name="spawnTime">Time to spawn.</param>
        /// <param name="minSpawnTime">Minimum time between each spawn.</param>
        /// <param name="spawnTimeModifier">Percentage to increase or decrease the time for each spawn.</param>
        public SpawnTimer(TimeSpan spawnTime, TimeSpan minSpawnTime, float spawnTimeModifier)
            : this(spawnTime)
        {
            _minSpawnTime = (float)minSpawnTime.TotalMilliseconds;
            _spawnTimeModifier = spawnTimeModifier;
        }

        #endregion

        /// <summary>
        /// Raise the elapsed event.
        /// </summary>
        protected virtual void OnSpawnTimerElapsed()
        {
            if (Elapsed != null)
            {
                Elapsed();
            }
        }

        /// <summary>
        /// Update the spawn timer.
        /// </summary>
        /// <param name="gameTime">The time for this frame.</param>
        /// <returns>True if the spawn time has elpased.</returns>
        public virtual void Update(GameTime gameTime)
        {
            // Make sure that a spawn time was properly set.
            if (_spawnTime <= 0)
            {
                return;
            }

            // Record the amount of seconds passed.
            _millisecondsElapsed += gameTime.ElapsedGameTime.Milliseconds;

            if (_millisecondsElapsed >= _spawnTime)
            {
                // Store the remaining seconds from the spawn.
                _millisecondsElapsed = _millisecondsElapsed % _spawnTime;

                // Alter the spawn time if the modifier has been set.
                if ((_spawnTimeModifier != 0) && !_limitReached)
                {
                    float modifiedTime = _spawnTime * _spawnTimeModifier;

                    if (modifiedTime <= _minSpawnTime)
                    {
                        _spawnTime = _minSpawnTime;
                        _limitReached = true;
                    }
                    else
                    {
                        _spawnTime = modifiedTime;
                    }
                }

                // Raise the spawn timer elapsed event.
                OnSpawnTimerElapsed();
            }
        }
    }
}

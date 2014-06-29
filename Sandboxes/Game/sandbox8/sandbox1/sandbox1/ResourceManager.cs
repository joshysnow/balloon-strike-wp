using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace sandbox8
{
    public class ResourceManager
    {
        public static ResourceManager Manager
        {
            get 
            {
                return _manager; 
            }
        }

        private static ResourceManager _manager;
        private static ContentManager _content;
        private static bool _loaded = false;

        private Dictionary<string, Texture2D> _textures;
        private Dictionary<string, SoundEffect> _sounds;
        private Dictionary<string, SpriteFont> _fonts;
        private Dictionary<string, Animation> _animations;

        private ResourceManager()
        {
            _textures = new Dictionary<string, Texture2D>();
            _sounds = new Dictionary<string, SoundEffect>();
            _fonts = new Dictionary<string, SpriteFont>();
            _animations = new Dictionary<string, Animation>();
        }

        public static void Initialize(ContentManager content)
        {
            if (_loaded)
            {
                return;
            }

            _manager = new ResourceManager();
            _content = content;
            _manager.LoadResources();

            _loaded = true;
        }

        public Texture2D GetTexture(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Name not initialized or empty");
            }

            if (!_loaded)
            {
                throw new Exception("Resource Managed has not been initialized");
            }

            if (!_textures.ContainsKey(key))
            {
                throw new KeyNotFoundException();
            }

            return _textures[key];
        }

        public SoundEffect GetSoundEffect(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Name not initialized or empty");
            }

            if (!_loaded)
            {
                throw new Exception("Resource Managed has not been initialized");
            }

            if (!_sounds.ContainsKey(key))
            {
                throw new KeyNotFoundException();
            }

            return _sounds[key];
        }

        public SpriteFont GetFont(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Name not initialized or empty");
            }

            if (!_loaded)
            {
                throw new Exception("Resource Managed has not been initialized");
            }

            if (!_fonts.ContainsKey(key))
            {
                throw new KeyNotFoundException();
            }

            return _fonts[key];
        }

        public Animation GetAnimation(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException("Name not initialized or empty");
            }

            if (!_loaded)
            {
                throw new Exception("Resource Managed has not been initialized");
            }

            if(!_animations.ContainsKey(key))
            {
                throw new KeyNotFoundException();
            }

            return _animations[key];
        }

        private void LoadResources()
        {
            _fonts.Add("debug", _content.Load<SpriteFont>("DebugText"));
            _fonts.Add("display", _content.Load<SpriteFont>("DisplayText"));
            _fonts.Add("gameover", _content.Load<SpriteFont>("GameOverText"));
            _fonts.Add("score", _content.Load<SpriteFont>("ScoreText"));

            // Load score fonts.
            for (int i = 0; i < 10; i++)
            {
                _fonts.Add("font_score" + i, _content.Load<SpriteFont>("Fonts/score" + i));
            }

            SoundEffect pop = _content.Load<SoundEffect>("Sounds/snowball_car_impact1");
            pop.Play(0, 0, 0);
            _sounds.Add("pop", pop);

            Texture2D redTexture = _content.Load<Texture2D>("Balloons/red200");
            Texture2D blueTexture = _content.Load<Texture2D>("Balloons/blue200");
            Texture2D greenTexture = _content.Load<Texture2D>("Balloons/green200");
            Texture2D popTexture = _content.Load<Texture2D>("Effects/explosion");
            Texture2D freezeTexture = _content.Load<Texture2D>("Powerups/snowflake_med");
            Texture2D shellTexture = _content.Load<Texture2D>("Powerups/shell_200");
            Texture2D missileTexture = _content.Load<Texture2D>("Powerups/missile_200");
            Texture2D reticleFingerTexture = _content.Load<Texture2D>("Reticles/finger");
            Texture2D reticleShotgunTexture = _content.Load<Texture2D>("Reticles/shotgun");
            Texture2D reticleRocketLauncherTexture = _content.Load<Texture2D>("Reticles/rocketlauncher");

            _textures.Add("red", redTexture);
            _textures.Add("blue", blueTexture);
            _textures.Add("green", greenTexture);
            _textures.Add("pop", popTexture);
            _textures.Add("freeze", freezeTexture);
            _textures.Add("shell", shellTexture);
            _textures.Add("missile", missileTexture);
            _textures.Add("reticle_finger", reticleFingerTexture);
            _textures.Add("reticle_shotgun", reticleShotgunTexture);
            _textures.Add("reticle_rocketlauncher", reticleRocketLauncherTexture);

            _animations.Add("popmove", new Animation(popTexture, false, popTexture.Width, popTexture.Height, 125, 0.25f));
            _animations.Add("redmove", new Animation(redTexture, true, redTexture.Width, redTexture.Height, 0, 0.5f));
            _animations.Add("greenmove", new Animation(greenTexture, true, greenTexture.Width, greenTexture.Height, 0, 0.5f));
            _animations.Add("bluemove", new Animation(blueTexture, true, blueTexture.Width, blueTexture.Height, 0, 0.5f));
            _animations.Add("freezemove", new Animation(freezeTexture, true, freezeTexture.Width, freezeTexture.Height, 0, 0.25f));
            _animations.Add("shellmove", new Animation(shellTexture, true, shellTexture.Width, shellTexture.Height, 0, 0.5f));
            _animations.Add("missilemove", new Animation(missileTexture, true, missileTexture.Width, missileTexture.Height, 0, 0.5f));
        }
    }
}

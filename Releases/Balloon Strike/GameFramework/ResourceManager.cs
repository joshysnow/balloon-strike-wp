using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace GameFramework
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
        private Dictionary<string, Song> _music;

        private ResourceManager()
        {
            _textures = new Dictionary<string, Texture2D>();
            _sounds = new Dictionary<string, SoundEffect>();
            _fonts = new Dictionary<string, SpriteFont>();
            _animations = new Dictionary<string, Animation>();
            _music = new Dictionary<string, Song>();
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

        public Song GetSong(string key)
        {
            if (!_loaded)
            {
                throw new Exception("Resource Managed has not been initialized");
            }

            if (!_music.ContainsKey(key))
            {
                throw new KeyNotFoundException();
            }

            return _music[key];
        }

        private void LoadResources()
        {
            _fonts.Add("debug", _content.Load<SpriteFont>("DebugText"));
            _fonts.Add("score", _content.Load<SpriteFont>("Fonts/score"));
            _fonts.Add("your_score", _content.Load<SpriteFont>("Fonts/yourScore"));

            // Load score fonts.
            for (int i = 0; i < 10; i++)
            {
                _fonts.Add("font_score" + i, _content.Load<SpriteFont>("Fonts/score" + i));
            }

            SoundEffect pop = _content.Load<SoundEffect>("Audio/Sounds/snowball_car_impact1");
            pop.Play(0, 0, 0);
            _sounds.Add("pop", pop);

            //Song test = _content.Load<Song>("Audio/Music/test_everythingisawesome");
            //_music.Add("test", test);

            Texture2D redTexture = _content.Load<Texture2D>("Textures/Balloons/red200");
            Texture2D blueTexture = _content.Load<Texture2D>("Textures/Balloons/blue200");
            Texture2D greenTexture = _content.Load<Texture2D>("Textures/Balloons/green200");
            Texture2D blackTexture = _content.Load<Texture2D>("Textures/Balloons/black200");
            Texture2D popTexture = _content.Load<Texture2D>("Textures/Effects/explosion");
            Texture2D freezeTexture = _content.Load<Texture2D>("Textures/Powerups/snowflake_med");
            Texture2D shellTexture = _content.Load<Texture2D>("Textures/Powerups/shell_200");
            Texture2D missileTexture = _content.Load<Texture2D>("Textures/Powerups/missile_200");
            Texture2D xHairFingerTexture = _content.Load<Texture2D>("Textures/Crosshairs/finger");
            Texture2D xHairShotgunTexture = _content.Load<Texture2D>("Textures/Crosshairs/shotgun");
            Texture2D xHairBazookaTexture = _content.Load<Texture2D>("Textures/Crosshairs/bazooka");
            Texture2D splashTexture = _content.Load<Texture2D>("Textures/Backgrounds/fox_480_800");
            Texture2D buttonPlayTexture = _content.Load<Texture2D>("Textures/Buttons/test_play");
            Texture2D buttonAboutTexture = _content.Load<Texture2D>("Textures/Buttons/test_about");

            _textures.Add("red", redTexture);
            _textures.Add("blue", blueTexture);
            _textures.Add("green", greenTexture);
            _textures.Add("pop", popTexture);
            _textures.Add("freeze", freezeTexture);
            _textures.Add("shell", shellTexture);
            _textures.Add("missile", missileTexture);
            _textures.Add("xhair_finger", xHairFingerTexture);
            _textures.Add("xhair_shotgun", xHairShotgunTexture);
            _textures.Add("xhair_bazooka", xHairBazookaTexture);
            _textures.Add("splash", splashTexture);
            _textures.Add("button_play", buttonPlayTexture);
            _textures.Add("button_about", buttonAboutTexture);

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

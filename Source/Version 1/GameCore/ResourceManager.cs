using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace GameCore
{
    public class ResourceManager
    {
        public static ResourceManager Resources
        {
            get
            {
                return _manager;
            }
        }

        private static ResourceManager _manager;
        private static ContentManager _content;
        private static bool _loaded = false;

        private const string EXCEPTION_NOT_INITIALIZED = "Resource Managed has not been initialized";

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
                throw new Exception(EXCEPTION_NOT_INITIALIZED);
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
                throw new Exception(EXCEPTION_NOT_INITIALIZED);
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
                throw new Exception(EXCEPTION_NOT_INITIALIZED);
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
                throw new Exception(EXCEPTION_NOT_INITIALIZED);
            }

            if (!_animations.ContainsKey(key))
            {
                throw new KeyNotFoundException();
            }

            return _animations[key];
        }

        public Song GetSong(string key)
        {
            if (!_loaded)
            {
                throw new Exception(EXCEPTION_NOT_INITIALIZED);
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
            _fonts.Add("credit_title", _content.Load<SpriteFont>("Fonts/credit_title"));
            _fonts.Add("credit_name", _content.Load<SpriteFont>("Fonts/credit_name"));
            _fonts.Add("small", _content.Load<SpriteFont>("Fonts/small"));
            _fonts.Add("popup_text", _content.Load<SpriteFont>("Fonts/popup_text"));

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

            Texture2D blankTexture = _content.Load<Texture2D>("Textures/white");

            Texture2D redBalloon = _content.Load<Texture2D>("Textures/Balloons/red200");
            Texture2D blueBalloon = _content.Load<Texture2D>("Textures/Balloons/blue200");
            Texture2D greenBalloon = _content.Load<Texture2D>("Textures/Balloons/green200");
            Texture2D blackBalloon = _content.Load<Texture2D>("Textures/Balloons/black200");
            Texture2D hitBalloon = _content.Load<Texture2D>("Textures/Balloons/white200");

            Texture2D sunCrying = _content.Load<Texture2D>("Textures/Sun/crying");
            Texture2D sunSad = _content.Load<Texture2D>("Textures/Sun/sad");
            Texture2D sunOk = _content.Load<Texture2D>("Textures/Sun/ok");
            Texture2D sunHappy = _content.Load<Texture2D>("Textures/Sun/happy");
            Texture2D sunSuperHappy = _content.Load<Texture2D>("Textures/Sun/superhappy");

            Texture2D popTexture = _content.Load<Texture2D>("Textures/Effects/explosion");
            Texture2D freezeTexture = _content.Load<Texture2D>("Textures/Powerups/snowflake_med");
            Texture2D shellTexture = _content.Load<Texture2D>("Textures/Powerups/shell_200");
            Texture2D missileTexture = _content.Load<Texture2D>("Textures/Powerups/missile_200");

            Texture2D xHairFingerTexture = _content.Load<Texture2D>("Textures/Crosshairs/finger");
            Texture2D xHairShotgunTexture = _content.Load<Texture2D>("Textures/Crosshairs/shotgun");
            Texture2D xHairBazookaTexture = _content.Load<Texture2D>("Textures/Crosshairs/bazooka");

            Texture2D splashTexture = _content.Load<Texture2D>("Textures/Backgrounds/fox_480_800");
            Texture2D popupforeground = _content.Load<Texture2D>("Textures/Backgrounds/popup440");

            Texture2D cloudSmall = _content.Load<Texture2D>("Textures/Clouds/cloud_small");
            Texture2D cloudMedium = _content.Load<Texture2D>("Textures/Clouds/cloud_medium");

            Texture2D buttonPlayUnselected      = _content.Load<Texture2D>("Textures/Buttons/hex_play");
            Texture2D buttonAchieveUnselected   = _content.Load<Texture2D>("Textures/Buttons/hex_achievements");
            Texture2D buttonInfoUnselected      = _content.Load<Texture2D>("Textures/Buttons/hex_info");
            Texture2D buttonMenuUnselected      = _content.Load<Texture2D>("Textures/Buttons/hex_menu");
            Texture2D buttonTickUnselected      = _content.Load<Texture2D>("Textures/Buttons/hex_tick");
            Texture2D buttonCancelUnselected    = _content.Load<Texture2D>("Textures/Buttons/hex_cancel");

            Texture2D buttonPlaySelected    = _content.Load<Texture2D>("Textures/Buttons/hex_play_pressed");
            Texture2D buttonAchieveSelected = _content.Load<Texture2D>("Textures/Buttons/hex_achievements_pressed");
            Texture2D buttonInfoSelected    = _content.Load<Texture2D>("Textures/Buttons/hex_info_pressed");
            Texture2D buttonMenuSelected    = _content.Load<Texture2D>("Textures/Buttons/hex_menu_pressed");
            Texture2D buttonTickSelected    = _content.Load<Texture2D>("Textures/Buttons/hex_tick_pressed");
            Texture2D buttonCancelSelected  = _content.Load<Texture2D>("Textures/Buttons/hex_cancel_pressed");

            Texture2D buttonPlay        = _content.Load<Texture2D>("Textures/Buttons/play_400px");
            Texture2D buttonHighscores  = _content.Load<Texture2D>("Textures/Buttons/highscores_400px");
            Texture2D buttonAbout       = _content.Load<Texture2D>("Textures/Buttons/about_400px");
            Texture2D buttonExit        = _content.Load<Texture2D>("Textures/Buttons/exit_400px");

            Texture2D title = _content.Load<Texture2D>("Textures/game_title");

            _textures.Add("title", title);

            _textures.Add("button_play", buttonPlay);
            _textures.Add("button_highscores", buttonHighscores);
            _textures.Add("button_about", buttonAbout);
            _textures.Add("button_exit", buttonExit);

            _textures.Add("blank", blankTexture);
            _textures.Add("red", redBalloon);
            _textures.Add("blue", blueBalloon);
            _textures.Add("green", greenBalloon);
            _textures.Add("pop", popTexture);
            _textures.Add("freeze", freezeTexture);
            _textures.Add("shell", shellTexture);
            _textures.Add("missile", missileTexture);
            _textures.Add("xhair_finger", xHairFingerTexture);
            _textures.Add("xhair_shotgun", xHairShotgunTexture);
            _textures.Add("xhair_bazooka", xHairBazookaTexture);
            _textures.Add("splash", splashTexture);
            _textures.Add("popup_foreground", popupforeground);

            _textures.Add("button_unselected_play", buttonPlayUnselected);
            _textures.Add("button_unselected_achievements", buttonAchieveUnselected);
            _textures.Add("button_unselected_info", buttonInfoUnselected);
            _textures.Add("button_unselected_menu", buttonMenuUnselected);
            _textures.Add("button_unselected_tick", buttonTickUnselected);
            _textures.Add("button_unselected_cancel", buttonCancelUnselected);

            _textures.Add("button_selected_play", buttonPlaySelected);
            _textures.Add("button_selected_achievements", buttonAchieveSelected);
            _textures.Add("button_selected_info", buttonInfoSelected);
            _textures.Add("button_selected_menu", buttonMenuSelected);
            _textures.Add("button_selected_tick", buttonTickSelected);
            _textures.Add("button_selected_cancel", buttonCancelSelected);

            _animations.Add("popmove", new Animation(popTexture, false, popTexture.Width, popTexture.Height, 125, 0.25f));
            _animations.Add("redmove", new Animation(redBalloon, true, redBalloon.Width, redBalloon.Height, 0, 0.5f));
            _animations.Add("greenmove", new Animation(greenBalloon, true, greenBalloon.Width, greenBalloon.Height, 0, 0.5f));
            _animations.Add("bluemove", new Animation(blueBalloon, true, blueBalloon.Width, blueBalloon.Height, 0, 0.5f));
            _animations.Add("blackmove", new Animation(blackBalloon, true, blackBalloon.Width, blackBalloon.Height, 0, 0.5f));
            _animations.Add("freezemove", new Animation(freezeTexture, true, freezeTexture.Width, freezeTexture.Height, 0, 0.25f));
            _animations.Add("hitmove", new Animation(hitBalloon, false, hitBalloon.Width, hitBalloon.Height, 100, 0.5f));
            _animations.Add("shellmove", new Animation(shellTexture, true, shellTexture.Width, shellTexture.Height, 0, 0.5f));
            _animations.Add("missilemove", new Animation(missileTexture, true, missileTexture.Width, missileTexture.Height, 0, 0.5f));
            _animations.Add("cloud_small_move", new Animation(cloudSmall, true, cloudSmall.Width, cloudSmall.Height, 0, 1f));
            _animations.Add("cloud_medium_move", new Animation(cloudMedium, true, cloudMedium.Width, cloudMedium.Height, 0, 1f));
            _animations.Add("sun_crying", new Animation(sunCrying, true, sunCrying.Width, sunCrying.Height, 0, 1));
            _animations.Add("sun_sad", new Animation(sunSad, true, sunSad.Width, sunSad.Height, 0, 1));
            _animations.Add("sun_ok", new Animation(sunOk, true, sunOk.Width, sunOk.Height, 0, 1));
            _animations.Add("sun_happy", new Animation(sunHappy, true, sunHappy.Width, sunHappy.Height, 0, 1));
            _animations.Add("sun_superhappy", new Animation(sunSuperHappy, true, sunSuperHappy.Width, sunSuperHappy.Height, 0, 1));
        }
    }
}

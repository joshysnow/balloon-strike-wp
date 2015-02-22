/**
 * FoxCode Studios. Copyright 2014. All rights reserved.
 * 
 * With thanks to Nokia.
 * http://developer.nokia.com/community/wiki/Fullscreen_XNA_games_on_Windows_Phone_8_devices
 * 
 **/

using System;
using System.Windows;
using Microsoft.Xna.Framework;

namespace BalloonStrike
{
    public struct Display
    {
        public int Width;
        public int Height;
    }

    /// <summary>
    /// Used to show supported resolutions.
    /// </summary>
    public enum ScreenResolution : byte
    {
        Unknown = 0x00,
        WVGA    = 0x01,     // 800 x 480
        WXGA    = 0x02,     // 1280 x 768
        HD720p  = 0x04      // 1280 x 720
    }

    public static class PhoneProperties
    {
        public static ScreenResolution SupportedResolutions
        {
            get;
            private set;
        }

        public static int OSVersion
        {
            get
            {
                return Environment.OSVersion.Version.Major;
            }
        }

        private static int? _scaleFactor;

        static PhoneProperties()
        {
            _scaleFactor = GetScaleFactor();
            SupportedResolutions = ResolutionsSupported(_scaleFactor);
        }

        public static void GetDisplaySettings(DisplayOrientation orientation, out Display settings)
        {
            settings = new Display();

            // TODO: Could be improved to base the sizes on the resolution.

            if (orientation == DisplayOrientation.Portrait)
            {
                settings.Width = 480;
                settings.Height = 800;
            }
            else
            {
                settings.Width = 800;
                settings.Height = 480;
            }
        }

        private static int? GetScaleFactor()
        {
            int? scaleFactor = null;
            var content = Application.Current.Host.Content;
            var scaleFactorProperty = content.GetType().GetProperty("ScaleFactor");

            if (scaleFactorProperty != null)
            {
                scaleFactor = scaleFactorProperty.GetValue(content, null) as int?;
            }

            if (scaleFactor == null)
            {
                scaleFactor = 100;
            }

            return scaleFactor;
        }

        private static ScreenResolution ResolutionsSupported(int? scaleFactor)
        {
            ScreenResolution type;

            if (scaleFactor == null)
            {
                type = ScreenResolution.Unknown;
            }
            else if (scaleFactor == 100)
            {
                type = ScreenResolution.WVGA | ScreenResolution.WXGA;
            }
            else if (scaleFactor == 150)
            {
                type = ScreenResolution.HD720p;
            }
            else
            {
                type = ScreenResolution.Unknown;
            }

            return type;
        }
    }
}

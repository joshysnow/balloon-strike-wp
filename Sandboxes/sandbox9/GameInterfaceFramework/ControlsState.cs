using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace GameInterfaceFramework
{
    public class ControlsState
    {
        public GestureSample[] Gestures
        {
            get
            {
                return _gestures.ToArray();
            }
        }

        public TouchCollection TouchState
        {
            get { return _touchState; }
        }

        private TouchCollection _touchState;
        private List<GestureSample> _gestures;

        public ControlsState()
        {
            _gestures = new List<GestureSample>();
        }

        public void Update()
        {
            _touchState = TouchPanel.GetState();
            _gestures.Clear();

            while (TouchPanel.IsGestureAvailable)
            {
                _gestures.Add(TouchPanel.ReadGesture());
            }
        }

        public bool BackButtonPressed()
        {
            return (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed);
        }
    }
}

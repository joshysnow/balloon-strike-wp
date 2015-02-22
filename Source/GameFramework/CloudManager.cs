﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameCore;
using GameCore.Triggers;

namespace GameFramework
{
    public class CloudManager : CharacterManager
    {
        private const int MOVABILITY = 25;

        private CloudModel _smallModel;
        private CloudModel _mediumModel;
        private Random _randomYGen;

        public CloudManager(GraphicsDevice graphics, TriggerManager triggers) : base(graphics, triggers) { }

        protected override void Initialize()
        {
            _randomYGen = new Random(DateTime.Now.Millisecond);

            InitializeCloudModels();

            Vector2[] smallCloudPositions = GetInitialSmallCloudPositions();
            Vector2[] mediumCloudPositions = GetInitialMediumCloudPositions();

            InitializeClouds(smallCloudPositions, _smallModel);
            InitializeClouds(mediumCloudPositions, _mediumModel);
        }

        protected override void UpdateCharacters(GameTime gameTime)
        {
            Cloud cloud;

            for (int i = 0; i < Characters.Count; i++)
            {
                cloud = (Cloud)Characters[i];

                if (cloud.State == CloudState.OffScreen)
                {
                    // Reuse cloud by changing its position
                    // instead of instantiating a new object.

                    CloudModel model;
                    Vector2 position = new Vector2(ScreenWidth, 0);
                    bool up = (_randomYGen.Next(2) > 0);

                    if (up)
                        position.Y = (int)(cloud.OriginalPosition.Y + _randomYGen.Next(MOVABILITY));
                    else
                        position.Y = (int)(cloud.OriginalPosition.Y - _randomYGen.Next(MOVABILITY));

                    if (cloud.Type == CloudType.Small)
                        model = _smallModel;
                    else
                        model = _mediumModel;

                    cloud.Initialize(model, position, ScreenWidth);
                }
                else
                {
                    cloud.Update(gameTime);
                }
            }
        }

        private void InitializeCloudModels()
        {
            ResourceManager resources = ResourceManager.Resources;
            Animation smallMove = resources.GetAnimation("cloud_small_move");
            Animation mediumMove = resources.GetAnimation("cloud_medium_move");

            Vector2 smallVelocity = new Vector2(-1.5f, 0);
            Vector2 mediumVelocity = new Vector2(-0.5f, 0);

            _smallModel = new CloudModel() { Velocity = smallVelocity, MoveAnimation = smallMove, Type = CloudType.Small };
            _mediumModel = new CloudModel() { Velocity = mediumVelocity, MoveAnimation = mediumMove, Type = CloudType.Medium };
        }

        private void InitializeClouds(Vector2[] initialPositions, CloudModel model)
        {
            Cloud cloud;
            Vector2 position;
            byte index = 0;

            while (index < initialPositions.Length)
            {
                position = initialPositions[index++];
                cloud = new Cloud() { OriginalPosition = position };
                cloud.Initialize(model, position, ScreenWidth);
                Characters.Add(cloud);
            }
        }

        private Vector2[] GetInitialSmallCloudPositions()
        {
            return new Vector2[3]
            { 
                new Vector2(50, 500), 
                new Vector2(275, 660), 
                new Vector2(480, 560)
            };
        }

        private Vector2[] GetInitialMediumCloudPositions()
        {
            return new Vector2[3]
            { 
                new Vector2(100, 100),
                new Vector2(400, 300),
                new Vector2(500, 50)
            };
        }
    }
}
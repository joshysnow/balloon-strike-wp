using System;
using System.Xml.Linq;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameCore;

namespace GameFramework
{
    public class CloudManager : CharacterManager
    {
        private const string STORAGE_FILE_NAME = "CLOUD_MANAGER.xml";
        private const int MOVABILITY = 25;

        private CloudModel _smallModel;
        private CloudModel _mediumModel;
        private Random _randomYGen;

        public CloudManager(GraphicsDevice graphics) : base(graphics) { }

        public override void Initialize()
        {
            _randomYGen = new Random(DateTime.Now.Millisecond);

            InitializeCloudModels();

            Vector2[] smallCloudPositions = GetInitialSmallCloudPositions();
            Vector2[] mediumCloudPositions = GetInitialMediumCloudPositions();

            InitializeClouds(smallCloudPositions, _smallModel);
            InitializeClouds(mediumCloudPositions, _mediumModel);
        }

        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                Initialize();
            }
            else
            {
                // TODO: Load from disk.
            }
        }

        public override void Deactivate()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                XDocument doc = new XDocument();
                XElement root = new XElement("CloudManager");
                doc.Add(root);

                XElement cloudElement;

                foreach (Cloud cloud in Characters)
                {
                    cloudElement = new XElement(
                        "View",
                        new XAttribute("ObjectType", cloud.GetType().AssemblyQualifiedName),
                        new XAttribute("CloudType", cloud.Type)
                        //new XAttribute("X", cloud.),
                        //new XAttribute("Y", cloud.)
                        );

                    root.Add(cloudElement);
                }

                using (IsolatedStorageFileStream stream = storage.CreateFile(STORAGE_FILE_NAME))
                {
                    doc.Save(stream);
                }
            }
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
                    RecycleCloud(cloud);
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

        private void RecycleCloud(Cloud cloud)
        {
            CloudModel model = GetCloudModel(cloud.Type);
            Vector2 position = AdjustCloudPosition(cloud);

            cloud.Initialize(model, position, ScreenWidth);
        }

        private CloudModel GetCloudModel(CloudType type)
        {
            CloudModel model;

            if (type == CloudType.Small)
                model = _smallModel;
            else
                model = _mediumModel;

            return model;
        }

        private Vector2 AdjustCloudPosition(Cloud cloud)
        {
            // Use the screen width so the cloud starts
            // off, off the right hand side of the screen.
            Vector2 position = new Vector2(ScreenWidth, 0);

            // Decide whether to adjust the position
            // further up or down the screen.
            bool up = (_randomYGen.Next(2) > 0);

            if (up)
                position.Y = (int)(cloud.OriginalPosition.Y + _randomYGen.Next(MOVABILITY));
            else
                position.Y = (int)(cloud.OriginalPosition.Y - _randomYGen.Next(MOVABILITY));

            return position;
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

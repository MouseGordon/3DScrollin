using System.Collections;
using GameStateSystems;
using NUnit.Framework;
using SavableComponent;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlaymodeTests
{
    public class SaveableComponentWorkflowTests
    {
        private GameObject _gameManagerObject;
        private GameObject _saveableObject;
        private SaveableTransform _saveableTransform;
        private SaveEventChannel _saveEventChannel;
        private GameManager _gameManager;

        [SetUp]
        public void Setup()
        {
            // Create GameManager
            _gameManagerObject = new GameObject("GameManager");
           // _gameManager = _gameManagerObject.AddComponent<GameManager>();

            // Create SaveEventChannel
            _saveEventChannel = ScriptableObject.CreateInstance<SaveEventChannel>();

            // Create saveable object
            _saveableObject = new GameObject("SaveableObject");
            _saveableTransform = _saveableObject.AddComponent<SaveableTransform>();
            _saveableTransform.SetSaveEventChannel(_saveEventChannel);

            // Set initial position
            _saveableObject.transform.position = new Vector3(5, 10, 15);
        }

        [TearDown]
        public void TearDown()
        {
            if (_gameManagerObject != null)
                Object.DestroyImmediate(_gameManagerObject);
            
            if (_saveableObject != null)
                Object.DestroyImmediate(_saveableObject);
                
            if (_saveEventChannel != null)
                Object.DestroyImmediate(_saveEventChannel);
        }

        [UnityTest]
        public IEnumerator FullWorkflow_InitializeAndSave()
        {
            // Simulate GameManager initialization flow
            yield return null;

            // Trigger OnGameObjectsInitialize event
       //     _gameManager.Initialize();

            yield return null;

            // Assert component is initialized
            Assert.IsTrue(_saveableTransform.IsInitialized);

            // Move object and trigger save
            var newPosition = new Vector3(100, 200, 300);
            _saveableObject.transform.position = newPosition;
            
            // Trigger save
            _saveEventChannel.RequestSave();
            
            yield return null;

            // Verify save data was updated
            var saveData = _saveableTransform.GetSaveData();
            Assert.AreEqual(newPosition.x, saveData.PositionX, 0.01f);
            Assert.AreEqual(newPosition.y, saveData.PositionY, 0.01f);
            Assert.AreEqual(newPosition.z, saveData.PositionZ, 0.01f);
        }

        [UnityTest]
        public IEnumerator MultipleComponents_AllGetInitialized()
        {
            // Create multiple saveable objects
            var objects = new GameObject[3];
            var components = new SaveableTransform[3];

            for (int i = 0; i < 3; i++)
            {
                objects[i] = new GameObject($"SaveableObject_{i}");
                components[i] = objects[i].AddComponent<SaveableTransform>();
                components[i].SetSaveEventChannel(_saveEventChannel);
                objects[i].transform.position = new Vector3(i * 10, i * 5, i * 2);
            }

            yield return null;

            // Assert all are initialized
            for (int i = 0; i < 3; i++)
            {
                Assert.IsTrue(components[i].IsInitialized, $"Component {i} should be initialized");
                Assert.IsNotNull(components[i].GetSaveData(), $"Component {i} should have save data");
            }

            // Cleanup
            for (int i = 0; i < 3; i++)
            {
                Object.DestroyImmediate(objects[i]);
            }
        }
    }
}

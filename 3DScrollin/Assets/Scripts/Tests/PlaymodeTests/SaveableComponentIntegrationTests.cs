using System.Collections;
using GameStateSystems;
using NUnit.Framework;
using SavableComponent;
using SaveSystems;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlaymodeTests
{
    public class SaveableComponentIntegrationTests
    {
        private GameObject _testObject;
        private SaveableTransform _saveableTransform;
        private SaveEventChannel _saveEventChannel;
        private Vector3 _originalPosition;
        private Vector3 _testPosition;

        [SetUp]
        public void Setup()
        {
            // Create test GameObject with SaveableTransform
            _testObject = new GameObject("TestObject");
            _saveableTransform = _testObject.AddComponent<SaveableTransform>();

            // Create SaveEventChannel ScriptableObject
            _saveEventChannel = ScriptableObject.CreateInstance<SaveEventChannel>();
            _saveableTransform.SetSaveEventChannel(_saveEventChannel);
            
            // Set up test positions
            _originalPosition = new Vector3(0, 0, 0);
            _testPosition = new Vector3(10, 5, 8);
            _testObject.transform.position = _originalPosition;
        }

        [TearDown]
        public void TearDown()
        {
            if (_testObject != null)
                Object.DestroyImmediate(_testObject);
            
            if (_saveEventChannel != null)
                Object.DestroyImmediate(_saveEventChannel);
        }

        [Test]
        public void Initialize_CreatesSaveDataWithCurrentPosition()
        {
            // Act
            _saveableTransform.Initialize();

            // Assert
            Assert.IsTrue(_saveableTransform.IsInitialized);
            var saveData = _saveableTransform.GetSaveData();
            Assert.IsNotNull(saveData);
            Assert.AreEqual(_originalPosition.x, saveData.PositionX, 0.01f);
            Assert.AreEqual(_originalPosition.y, saveData.PositionY, 0.01f);
            Assert.AreEqual(_originalPosition.z, saveData.PositionZ, 0.01f);
        }

        [Test]
        public void OnSaveRequested_UpdatesDataWithCurrentPosition()
        {
            // Arrange
            _saveableTransform.Initialize();
            _testObject.transform.position = _testPosition;
            _saveEventChannel.RequestSave();
            // Assert
            var saveData = _saveableTransform.GetSaveData();
            Assert.AreEqual(_testPosition.x, saveData.PositionX, 0.01f);
            Assert.AreEqual(_testPosition.y, saveData.PositionY, 0.01f);
            Assert.AreEqual(_testPosition.z, saveData.PositionZ, 0.01f);
        }

        [UnityTest]
        public IEnumerator SaveAndLoad_RestoresTransformPosition()
        {
            // Arrange - Initialize and move to test position
            _saveableTransform.Initialize();
            _testObject.transform.position = _testPosition;
         

            // Move to different position
            var differentPosition = new Vector3(100, 200, 300);
            _testObject.transform.position = differentPosition;
            
            yield return null; // Wait a frame

            // Act - Simulate loading saved data
            var saveData = _saveableTransform.GetSaveData();
            var loadedData = new PlayerSaveData(_testPosition, "TestObject");
            _saveEventChannel.RequestSave();
            
            // Manually trigger OnDataLoaded (simulating load from file)
            var method = typeof(SaveableTransform).GetMethod("OnDataLoaded", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            method.Invoke(_saveableTransform, new object[] { loadedData });

            yield return null; // Wait a frame for position to be applied

            // Assert
            Assert.AreEqual(_testPosition.x, _testObject.transform.position.x, 0.01f);
            Assert.AreEqual(_testPosition.y, _testObject.transform.position.y, 0.01f);
            Assert.AreEqual(_testPosition.z, _testObject.transform.position.z, 0.01f);
        }

        [Test]
        public void WithCharacterController_UsesCorrectTransformHandler()
        {
            // Arrange - Add CharacterController
            var characterController = _testObject.AddComponent<CharacterController>();
            var newSaveableTransform = _testObject.AddComponent<SaveableTransform>();
            newSaveableTransform.SetSaveEventChannel(_saveEventChannel);

            // Act
            newSaveableTransform.Initialize();

            // Assert - Verify it doesn't throw and initializes properly
            Assert.IsTrue(newSaveableTransform.IsInitialized);
            
            // Clean up
            Object.DestroyImmediate(characterController);
            Object.DestroyImmediate(newSaveableTransform);
        }
    }
}

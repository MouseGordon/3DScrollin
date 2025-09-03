using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using GameStateSystems;
using NUnit.Framework;
using SavableComponent;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.PlaymodeTests
{
    public class SaveableComponentPerformanceTests
    {
        private List<GameObject> _testObjects = new List<GameObject>();
        private SaveEventChannel _saveEventChannel;

        [SetUp]
        public void Setup()
        {
            _saveEventChannel = ScriptableObject.CreateInstance<SaveEventChannel>();
        }

        [TearDown]
        public void TearDown()
        {
            foreach (var obj in _testObjects)
            {
                if (obj != null)
                    Object.DestroyImmediate(obj);
            }
            _testObjects.Clear();

            if (_saveEventChannel != null)
                Object.DestroyImmediate(_saveEventChannel);
        }

        [UnityTest]
        public IEnumerator StressTest_100SaveableComponents()
        {
            const int componentCount = 100;
            var stopwatch = new Stopwatch();

            // Create components
            stopwatch.Start();
            for (int i = 0; i < componentCount; i++)
            {
                var obj = new GameObject($"SaveableObject_{i}");
                var component = obj.AddComponent<SaveableTransform>();
                component.SetSaveEventChannel(_saveEventChannel);
                obj.transform.position = new Vector3(i, i * 2, i * 3);
                _testObjects.Add(obj);
            }
            stopwatch.Stop();
            UnityEngine.Debug.Log($"Created {componentCount} components in {stopwatch.ElapsedMilliseconds}ms");

            yield return null;

            // Initialize all
            stopwatch.Restart();
            foreach (var obj in _testObjects)
            {
                obj.GetComponent<SaveableTransform>().Initialize();
            }
            stopwatch.Stop();
            UnityEngine.Debug.Log($"Initialized {componentCount} components in {stopwatch.ElapsedMilliseconds}ms");

            // Trigger mass save
            stopwatch.Restart();
            _saveEventChannel.RequestSave();
            stopwatch.Stop();
            UnityEngine.Debug.Log($"Triggered save for {componentCount} components in {stopwatch.ElapsedMilliseconds}ms");

            yield return null;

            // Verify all are working
            foreach (var obj in _testObjects)
            {
                var component = obj.GetComponent<SaveableTransform>();
                Assert.IsTrue(component.IsInitialized);
                Assert.IsNotNull(component.GetSaveData());
            }

            Assert.Pass($"Successfully handled {componentCount} saveable components");
        }
    }
}

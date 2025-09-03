using System;
using NUnit.Framework;
using SavableComponent;
using SaveSystems;
using Tests.TestDoubles;

namespace Tests.EditorTests.Editor{
    [TestFixture]
    public class SaveableSystemTests{
        private FakeSaveEventChannel _fakeSaveEventChannel;
        private SaveableSystem<TestSaveData> _saveableSystem;

        [SetUp]
        public void Setup(){
            _fakeSaveEventChannel = new FakeSaveEventChannel();
        }

        [TearDown]
        public void TearDown(){
            _saveableSystem?.Dispose();
        }

        [Test]
        public void SaveableSystem_Constructor_SetsProperties(){
            // Arrange & Act
            _saveableSystem = new SaveableSystem<TestSaveData>(
                _fakeSaveEventChannel,
                () => new TestSaveData { Id = Guid.NewGuid() },
                data => { },
                () => { }
            );
            
            // constructor initializes the system, so we can just assert the properties
            //
            Assert.IsTrue(_saveableSystem.IsInitialized);
            // save data is initialized in InitializeSaveData()
            //
            Assert.IsNotNull(_saveableSystem.SaveData);
        }

        [Test]
        public void SaveableSystem_Constructor_NullSaveEventChannel_ThrowsArgumentNullException(){
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
            {
                _saveableSystem = new SaveableSystem<TestSaveData>(
                    null,
                    () => new TestSaveData { Id = Guid.NewGuid() },
                    data => { },
                    () => { }
                );
            });
        }


        [Test]
        public void SaveableSystem_Initialize_ExecutesAllSteps(){
            // Arrange
            bool onDataLoadedCalled = false;
            TestSaveData createdData = null;

            _saveableSystem = new SaveableSystem<TestSaveData>(
                _fakeSaveEventChannel,
                () => {
                    createdData = new TestSaveData { Id = Guid.NewGuid() };
                    return createdData;
                },
                data => onDataLoadedCalled = true,
                () => { }
            );

            // Act - This should hit Initialize(), InitializeSaveData(), LoadExistingData(), SubscribeToSaveEvents()

            // Assert
            Assert.IsTrue(_saveableSystem.IsInitialized);
            Assert.IsNotNull(_saveableSystem.SaveData);
            Assert.AreSame(createdData, _saveableSystem.SaveData);
            Assert.IsTrue(_fakeSaveEventChannel.IsSubscribedToSaveRequested);
        }

        [Test]
        public void SaveableSystem_InitializeTwice_OnlyInitializesOnce(){
            // Arrange
            int createCallCount = 0;
            _saveableSystem = new SaveableSystem<TestSaveData>(
                _fakeSaveEventChannel,
                () => {
                    createCallCount++;
                    return new TestSaveData { Id = Guid.NewGuid() };
                },
                data => { },
                () => { }
            );

            // Act - This should hit the early return in Initialize()
       
            var firstData = _saveableSystem.SaveData;
 

            // Assert
            Assert.AreEqual(1, createCallCount);
            Assert.AreSame(firstData, _saveableSystem.SaveData);
        }

        [Test]
        public void SaveableSystem_LoadExistingData_CallsOnDataLoaded(){
            // Arrange
            var existingData = new TestSaveData { Id = Guid.NewGuid() };
            _fakeSaveEventChannel.SetupLoadData(existingData);

            bool onDataLoadedCalled = false;
            TestSaveData loadedData = null;

            _saveableSystem = new SaveableSystem<TestSaveData>(
                _fakeSaveEventChannel,
                () => new TestSaveData { Id = existingData.Id },
                data => {
                    onDataLoadedCalled = true;
                    loadedData = data;
                },
                () => { }
            );
            
            // Act - This should hit LoadExistingData() and the data loading logic
            _saveableSystem.LoadExistingData();

            // Assert
            Assert.IsTrue(onDataLoadedCalled);
            Assert.AreSame(existingData, loadedData);
        }

        [Test]
        public void SaveableSystem_SaveRequested_UpdatesAndRegisters(){
            // Arrange
            bool updateCalled = false;
            var testData = new TestSaveData { Id = Guid.NewGuid() };

            _saveableSystem = new SaveableSystem<TestSaveData>(
                _fakeSaveEventChannel,
                () => testData,
                data => { },
                () => updateCalled = true
            );
            

            // Act - This should hit OnSaveRequested() method
            _fakeSaveEventChannel.TriggerSaveRequested();

            // Assert
            Assert.IsTrue(updateCalled);
            Assert.Contains(testData, _fakeSaveEventChannel.RegisteredSaveData);
        }

        [Test]
        public void SaveableSystem_SaveRequestedWhenNotInitialized_DoesNothing(){
            // Arrange
            bool updateCalled = false;
            _saveableSystem = new SaveableSystem<TestSaveData>(
                _fakeSaveEventChannel,
                () => new TestSaveData { Id = Guid.NewGuid() },
                data => { },
                () => updateCalled = true
            );

            // Act - This should hit the early return in OnSaveRequested()
            _fakeSaveEventChannel.TriggerSaveRequested();

            // Assert Save data is updated, when the saveable system is created.
            //
            Assert.IsTrue(updateCalled);
            // save dara is registered when the saveable system is created.
            //
            Assert.IsNotEmpty(_fakeSaveEventChannel.RegisteredSaveData);
        }

        [Test]
        public void SaveableSystem_Dispose_UnsubscribesFromEvents(){
            // Arrange
            _saveableSystem = new SaveableSystem<TestSaveData>(
                _fakeSaveEventChannel,
                () => new TestSaveData { Id = Guid.NewGuid() },
                data => { },
                () => { }
            );
            
            Assert.IsTrue(_fakeSaveEventChannel.IsSubscribedToSaveRequested);

            // Act - This should hit Dispose() and UnsubscribeFromSaveEvents()
            _saveableSystem.Dispose();

            // Assert
            Assert.IsFalse(_fakeSaveEventChannel.IsSubscribedToSaveRequested);
        }

        // Test individual methods explicitly
        [Test]
        public void SaveableSystem_InitializeSaveData_CallsCreateFunction(){
            // Arrange
            bool createCalled = false;
            var testData = new TestSaveData { Id = Guid.NewGuid() };

            _saveableSystem = new SaveableSystem<TestSaveData>(
                _fakeSaveEventChannel,
                () => {
                    createCalled = true;
                    return testData;
                },
                data => { },
                () => { }
            );

            // Assert
            Assert.IsTrue(createCalled);
            Assert.AreSame(testData, _saveableSystem.SaveData);
        }
        

        public class TestSaveData : ISaveData{
            private DateTime _saveTimeUtc;
            public Guid Id{ get; set; }

            public DateTime SaveTimeUtc => _saveTimeUtc;
        }
    }
}
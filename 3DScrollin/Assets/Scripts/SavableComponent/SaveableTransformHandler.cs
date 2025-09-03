using SaveSystems;
using UnityEngine;

namespace SavableComponent{
    public class SaveableTransform : SaveableComponentBase<PlayerSaveData>{
        private ITransformHandler _transformHandler;

        protected override void Awake(){
            base.Awake();
            var characterController = GetComponent<CharacterController>();
            _transformHandler = characterController != null
                ? new CharacterControllerTransformHandler(characterController, transform)
                : new BasicTransformHandler(transform);
        }

        protected override PlayerSaveData CreateSaveData(){
            return new PlayerSaveData(transform.position, gameObject.name);
        }

        protected override void UpdateSaveData(){
            GetSaveData()?.SetPositionData(transform.position);
        }

        protected override void OnDataLoaded(PlayerSaveData data){
            var position = new Vector3(data.PositionX, data.PositionY, data.PositionZ);
            _transformHandler.SetPosition(position);
        }
    }
}

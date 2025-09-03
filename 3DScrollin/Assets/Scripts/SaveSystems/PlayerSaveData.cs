using System;
using Newtonsoft.Json;
using UnityEngine;


namespace SaveSystems{
    [Serializable]
    public class PlayerSaveData : BaseSaveData{
        public static readonly Guid TYPE_ID = new("A1B2C3D4-E5F6-47A8-B9C0-D1E2F3A4B5C6");

        public override Guid Id => _cachedId == Guid.Empty
            ? (_cachedId = GenerateConsistentId(_objectName ?? "Unknown"))
            : _cachedId;

        [JsonProperty] private string _objectName;
        [JsonProperty] private string _cachedIdString;
        private Guid _cachedId;

        public float PositionX;
        public float PositionY;
        public float PositionZ;

        public PlayerSaveData(Vector3 position, string objectName){
            SetPositionData(position);
            _objectName = objectName;
            _cachedId = GenerateConsistentId(objectName);
            _cachedIdString = _cachedId.ToString();
        }

        [System.Runtime.Serialization.OnDeserialized]
        private void OnDeserialized(System.Runtime.Serialization.StreamingContext context){
            if (!string.IsNullOrEmpty(_cachedIdString)){
                _cachedId = new Guid(_cachedIdString);
            }
            else if (!string.IsNullOrEmpty(_objectName)){
                _cachedId = GenerateConsistentId(_objectName);
                _cachedIdString = _cachedId.ToString();
            }
        }

        public void SetPositionData(Vector3 position){
            PositionX = position.x;
            PositionY = position.y;
            PositionZ = position.z;
        }

        private static Guid GenerateConsistentId(string name){
            if (string.IsNullOrEmpty(name)) name = "Unknown";

            // Fixed: Ensure exactly 16 bytes for GUID
            var nameBytes = System.Text.Encoding.UTF8.GetBytes(name);
            var typeBytes = TYPE_ID.ToByteArray();

            var combined = new byte[16];
            for (int i = 0; i < 16; i++){
                combined[i] = (byte)(typeBytes[i] ^ (nameBytes[i % nameBytes.Length]));
            }

            return new Guid(combined);
        }
    }
}
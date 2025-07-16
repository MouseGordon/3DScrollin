using System;
using UnityEngine;

namespace GameEvent{
    public abstract class GameEvent<T>:ScriptableObject{
        public Action<T> EventAction;
    
        public void Raise(T data){
            EventAction?.Invoke(data);
        }
        
    }
}

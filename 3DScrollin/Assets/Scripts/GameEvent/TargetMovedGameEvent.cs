using System;
using UnityEngine;

namespace GameEvent{
    [CreateAssetMenu(fileName = "PlayerMovedGameEvent", menuName = "Scriptable Objects/GameEvent/PlayerMoved")]
    public class TargetMovedGameEvent:GameEvent<Transform>{
    }
}
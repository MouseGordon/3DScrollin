using UnityEngine;

[CreateAssetMenu(fileName = "PlayerCoreData", menuName = "Scriptable Objects/PlayerCoreData")]
public class PlayerCoreData : ScriptableObject{
    public float health;
    public float maxHealth;
    public float stamina;
    public float maxStamina;
}

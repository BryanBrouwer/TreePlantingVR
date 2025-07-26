using UnityEngine;

namespace Plant.Action
{
    [CreateAssetMenu(fileName = "NewWaterAction", menuName = "Scriptable Objects/Plant/Action/Water", order = 0)]
    public class WaterAction : ScriptableObject
    {
        [field: SerializeField] public float waterAmount { get; private set; }
    }
}
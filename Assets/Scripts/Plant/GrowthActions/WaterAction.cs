using Tool;
using UnityEngine;

namespace Plant.GrowthActions
{
    [CreateAssetMenu(fileName = "NewWaterAction", menuName = "Scriptable Objects/Plant/Action/Water", order = 0)]
    public class WaterAction : GrowthAction
    {
        [field: SerializeField] public float waterAmount { get; private set; } = 60;

        private void OnEnable()
        {
            isInstantCompletion = false;
            progressTarget = waterAmount;
            toolType = ToolType.Water;
        }
    }
}
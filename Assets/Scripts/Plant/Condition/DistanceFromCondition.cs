using UnityEngine;

namespace Plant.Condition
{
    // Condition that checks if the seed is within or outside a certain distance from a target plant type.
    [CreateAssetMenu(fileName = "NewDistanceFromCondition", menuName = "Scriptable Objects/Plant/Condition/Distance From", order = 0)]
    public class DistanceFromCondition : PlantCondition
    {
        [field: SerializeField] public float checkDistance { get; private set; }
        [field: SerializeField] public bool shouldBeNear { get; private set; }
        [field: SerializeField] public PlantType targetPlantType { get; private set; }

        public override bool CheckCondition()
        {
            return true;
        }
    }
}
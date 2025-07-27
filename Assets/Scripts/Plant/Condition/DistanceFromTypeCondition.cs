using System.Collections.Generic;
using UnityEngine;

namespace Plant.Condition
{
    // Condition that checks if the seed is within or outside a certain distance from a target plant type.
    [CreateAssetMenu(fileName = "NewDistanceFromCondition", menuName = "Scriptable Objects/Plant/Condition/Distance From Type", order = 0)]
    public class DistanceFromTypeCondition : DistanceFromAllCondition
    {
        [field: SerializeField] public PlantType targetPlantType { get; private set; }
        
        protected override List<PlantInstance> GetPlantsToCheck()
        {
            return PlantInstanceManager.Instance.GetPlantInstancesOfType(targetPlantType);
        }
    }
}
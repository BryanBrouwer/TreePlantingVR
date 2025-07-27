using System.Collections.Generic;
using UnityEngine;

namespace Plant.Condition
{
    // Condition that checks if the seed is within or outside a certain distance from a target plant type.
    [CreateAssetMenu(fileName = "NewDistanceFromCondition", menuName = "Scriptable Objects/Plant/Condition/Distance From All", order = 0)]
    public class DistanceFromAllCondition : PlantCondition
    {
        [field: SerializeField] public float checkDistance { get; private set; }
        [field: SerializeField] public bool shouldBeNear { get; private set; }

        protected virtual List<PlantInstance> GetPlantsToCheck()
        {
            return PlantInstanceManager.Instance.GetAllPlantInstances();
            
        }
        
        public override bool CheckCondition(PlantableSeed seed)
        {
            // Since I added the plant instance manager to handle easier filtering of plant types, we just get the list from their, and loop over the relevant plants for a distance calculation
            // for near we only need one success to complete, however for far we need all to succeed.
            var plantToCheck = GetPlantsToCheck();
            foreach (var plant in plantToCheck)
            {
                var distance = Vector3.Distance(seed.GetPlantPosition(), plant.transform.position);
                if (shouldBeNear)
                {
                    if (distance < checkDistance) return true;
                }
                else
                {
                    if (distance < checkDistance) return false;
                }
            }
            // We didnt find any plants within the range, so near would be false, but otherwise default to true for far.
            if (shouldBeNear) 
                return false;
            return true;
        }
    }
}
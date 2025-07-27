using UnityEngine;

namespace Plant.Condition
{
    // Abstract class for plant conditions.
    public abstract class PlantCondition : ScriptableObject
    {
        // Function should be overridden by behavior that checks if the seed is current in a valid state to be planted by this condition rule.
        public abstract bool CheckCondition(PlantableSeed seed);
    }
}

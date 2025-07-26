using System.Collections.Generic;
using Plant.Condition;
using Plant.State;
using UnityEngine;

namespace Plant
{
    [CreateAssetMenu(fileName = "NewSeedData", menuName = "Scriptable Objects/Plant/Seed Data", order = 0)]
    public class SeedData : ScriptableObject
    {
        [field: SerializeField] public PlantType plantType { get; private set; }
        [field: SerializeField] public PlantCondition[] conditions { get; private set; }
        [field: SerializeField] private List<CustomKeyValuePair<LifeState, LifeStateConfig>> lifeStateConfigList = new List<CustomKeyValuePair<LifeState, LifeStateConfig>>();
        private readonly Dictionary<LifeState, LifeStateConfig> _lifeStateConfigs = new Dictionary<LifeState, LifeStateConfig>();
        
        private void OnEnable()
        {
            foreach (var kvp in lifeStateConfigList)
            {
                _lifeStateConfigs[kvp.key] = kvp.value;
            }
        }
        
        public LifeStateConfig GetLifeStateConfig(LifeState lifeState)
        {
            return _lifeStateConfigs[lifeState];
        }
    }
}
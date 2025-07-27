using System.Collections.Generic;
using Tool;
using UnityEngine;

namespace Plant.GrowthActions
{
    [CreateAssetMenu(fileName = "NewWeedingAction", menuName = "Scriptable Objects/Plant/Action/Weeding", order = 0)]
    public class WeedingAction : GrowthAction
    {
        [field: SerializeField] public float weedingAmount { get; private set; } = 20;
        [field: SerializeField] public bool shouldInstantComplete { get; private set; } = true;
        [SerializeField] public GameObject weedingEffectPrefab;

        private void OnEnable()
        {
            isInstantCompletion = shouldInstantComplete;
            progressTarget = weedingAmount;
            toolType = ToolType.Weed;
        }
        
        public override void SetupAction(PlantInstance plantInstance)
        {
            var newWeedingEffect = Instantiate(weedingEffectPrefab, plantInstance.transform.position, Quaternion.identity);
            newWeedingEffect.transform.parent = plantInstance.transform;
            plantInstance.GrowthActionVisualsDictionary[this] = new List<GameObject> { newWeedingEffect };
        }

        public override void CleanupAction(PlantInstance plantInstance)
        {
            foreach (var visualObject in plantInstance.GrowthActionVisualsDictionary[this])
            {
                Destroy(visualObject);
            }
            plantInstance.GrowthActionVisualsDictionary.Remove(this);
        }
    }
}
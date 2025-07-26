using Tool;
using UnityEngine;

namespace Plant.GrowthActions
{
    // Abstract class for growth actions.
    public abstract class GrowthAction : ScriptableObject
    { 
        public delegate void GrowthActionComplete(GrowthAction growthAction); 
        public GrowthActionComplete OnComplete;
        public bool isInstantCompletion { get; protected set; } = true;
        public float progressTarget { get; protected set; } = 100;
        [field: SerializeField] public ToolType toolType { get; protected set; } = ToolType.None;
        [field: SerializeField] public float timeToComplete { get; protected set; } = 30;
        [field: SerializeField] public Sprite actionImage { get; protected set; }

        public virtual void SetupAction(PlantInstance plantInstance)
        {
            return;
        }

        public virtual void CleanupAction(PlantInstance plantInstance)
        {
            return;
        }
    }
}
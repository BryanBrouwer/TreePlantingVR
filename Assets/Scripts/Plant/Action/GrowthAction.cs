using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Plant.Action
{
    // Abstract class for growth actions.
    public abstract class GrowthAction : ScriptableObject
    { 
        public delegate void GrowthActionComplete(GrowthAction growthAction); 
        public GrowthActionComplete OnComplete;
        [field: SerializeField] public float timeToComplete { get; private set; }
        [field: SerializeField] public Sprite actionImage { get; private set; }
        
        public abstract void SetupAction();
    }
}
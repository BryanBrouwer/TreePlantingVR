using Plant.Action;
using UnityEngine;

namespace Plant.State
{
    public struct LifeStateConfig
    {
        public readonly GameObject PlantPrefab;
        public readonly Vector3 ActionUIOffset;
        public readonly int ScorePoints;
        public readonly LifeState NextLifeState;
        public readonly GrowthAction[] GrowthActions;

        public LifeStateConfig(GameObject plantPrefab, Vector3 actionUIOffset, int scorePoints, LifeState nextLifeState, GrowthAction[] growthActions)
        {
            PlantPrefab = plantPrefab;
            ActionUIOffset = actionUIOffset;
            ScorePoints = scorePoints;
            NextLifeState = nextLifeState;
            GrowthActions = growthActions;
        }
    }
}
using System;
using Plant.GrowthActions;
using UnityEngine;
using UnityEngine.Serialization;

namespace Plant.State
{
    [Serializable]
    public struct LifeStateConfig
    {
        public GameObject plantPrefab;
        public Vector3 actionUIOffset;
        public int scorePoints;
        public LifeState nextLifeState;
        public GrowthAction[] growthActions;
        public bool hasNextLifeState;

        public LifeStateConfig(GameObject plantPrefab, Vector3 actionUIOffset, int scorePoints, LifeState nextLifeState, GrowthAction[] growthActions, bool hasNextLifeState)
        {
            this.plantPrefab = plantPrefab;
            this.actionUIOffset = actionUIOffset;
            this.scorePoints = scorePoints;
            this.nextLifeState = nextLifeState;
            this.growthActions = growthActions;
            this.hasNextLifeState = hasNextLifeState;
        }
    }
}
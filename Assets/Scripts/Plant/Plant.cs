using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Plant.Action;
using Plant.State;
using UnityEngine;

namespace Plant
{
    public class PlantInstance : MonoBehaviour
    {
        public SeedData seedData;
        private LifeState currentLifeState { get; set; }
        private GameObject plantPrefabInstance { get; set; }
        private Dictionary<GrowthAction, bool> _growthActionsCompletionStates;

        private bool CheckAllGrowthActionsCompleted()
        {
            return _growthActionsCompletionStates.Keys.All(growthAction => _growthActionsCompletionStates[growthAction]);
        }
        
        private void SetGrowthActionCompleted(GrowthAction action)
        {
            if (!_growthActionsCompletionStates[action])
                return;
            _growthActionsCompletionStates[action] = true;
            
            //Check if all actions are completed, this way we can efficiently transition to the next state following an observer pattern instead of relying on update ticks.
            if (CheckAllGrowthActionsCompleted())
                TransitionToState(seedData.GetLifeStateConfig(currentLifeState).NextLifeState);
        }

        private void TransitionToState(LifeState lifeState)
        {
            if (currentLifeState == lifeState)
                return;
            
            // Cleaning up old plant instance and delegate subscriptions.
            if (plantPrefabInstance)
            {
                Destroy(plantPrefabInstance);
                foreach (var growthAction in _growthActionsCompletionStates.Keys)
                {
                    growthAction.OnComplete -= SetGrowthActionCompleted;
                }
            }

            plantPrefabInstance = Instantiate(seedData.GetLifeStateConfig(lifeState).PlantPrefab, transform);
            currentLifeState = lifeState;

            // Setting up new growth actions and delegate subscriptions.
            _growthActionsCompletionStates = new Dictionary<GrowthAction, bool>();
            foreach (var growthAction in seedData.GetLifeStateConfig(lifeState).GrowthActions)
            {
                growthAction.SetupAction();
                _growthActionsCompletionStates.Add(growthAction, false);
                growthAction.OnComplete += SetGrowthActionCompleted;
                //TODO Setup the UI indicators for the growth actions
            }
        }
        
        private void Start()
        {
            TransitionToState(LifeState.Seed);
        }
    }
}

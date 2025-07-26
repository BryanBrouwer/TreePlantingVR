using System.Collections.Generic;
using System.Linq;
using Plant.GrowthActions;
using Plant.State;
using Tool;
using UnityEngine;

namespace Plant
{
    public class PlantInstance : MonoBehaviour
    {
        public SeedData seedData;
        [field: SerializeField] public Collider plantCollider { get; private set; }
        
        private LifeState _currentLifeState;
        private GameObject _plantPrefabInstance;
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
                TransitionToState(seedData.GetLifeStateConfig(_currentLifeState).nextLifeState);
        }
        
        public void SetupState(LifeState lifeState)
        {
            _plantPrefabInstance = Instantiate(seedData.GetLifeStateConfig(lifeState).plantPrefab, transform);
            _currentLifeState = lifeState;

            // If there is no next life state, we are done.
            if (!seedData.GetLifeStateConfig(_currentLifeState).hasNextLifeState)
                return;
            
            // Setting up new growth actions and delegate subscriptions.
            _growthActionsCompletionStates = new Dictionary<GrowthAction, bool>();
            foreach (var growthAction in seedData.GetLifeStateConfig(lifeState).growthActions)
            {
                growthAction.SetupAction(this);
                _growthActionsCompletionStates.Add(growthAction, false);
                growthAction.OnComplete += SetGrowthActionCompleted;
                //TODO Setup the UI indicators for the growth actions
            }
        }

        public void TransitionToState(LifeState lifeState)
        {
            if (_currentLifeState == lifeState)
                return;
            
            // Cleaning up old plant instance and delegate subscriptions.
            if (_plantPrefabInstance)
            {
                Destroy(_plantPrefabInstance);
                foreach (var growthAction in _growthActionsCompletionStates.Keys)
                {
                    growthAction.OnComplete -= SetGrowthActionCompleted;
                }
            }
            
            SetupState(lifeState);
        }
        
        private void Start()
        {
            if (!seedData)
                return;
            
            TransitionToState(LifeState.Seed);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("PlantTool")) 
                return;
            
            var plantTool = other.GetComponent<IPlantTool>();
            if (plantTool == null) 
                return;
            if (!plantTool.isToolActive)
                return;

            foreach (var growthAction in _growthActionsCompletionStates.Keys)
            {
                //TODO Check if the tool is compatible with the growth action
                //TODO I am thinking about refactoring the growthactions completion states into a list where actions get added on state transation,
                //          and removed upon completion, this would also save addition if statement checks in here
            }
                
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("PlantTool"))
            {
                
            }
        }
    }
}

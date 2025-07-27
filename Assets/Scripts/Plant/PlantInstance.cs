using System.Collections;
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
        
        private readonly Dictionary<GrowthAction, float> _growthActionProgressDictionary = new Dictionary<GrowthAction, float>();
        private readonly Dictionary<(GrowthAction action, GameObject tool), Coroutine> _activeCoroutines = new Dictionary<(GrowthAction action, GameObject tool), Coroutine>();
        private readonly List<IPlantTool> _overlappingTools = new List<IPlantTool>();
        
        private readonly Dictionary<GrowthAction, Coroutine> _actionTimeoutCoroutines = new Dictionary<GrowthAction, Coroutine>();

        private IEnumerator ActionTimeout(GrowthAction action)
        {
            float remainingTime = action.timeToComplete;
            float interval = action.timeToComplete / 3f;
            int currentInterval = 0;

            while (remainingTime > 0f)
            {
                yield return null;
                remainingTime -= Time.deltaTime;
                
                int newInterval = Mathf.FloorToInt((action.timeToComplete - remainingTime) / interval);
                if (newInterval != currentInterval)
                {
                    currentInterval = newInterval;
                    // TODO: Trigger UI update to reflect background interval change
                    Debug.Log("Current interval changed");
                }
            }

            // If the action is still active after timeout, plant dies
            if (_growthActionProgressDictionary.ContainsKey(action))
            {
                TransitionToState(LifeState.Dead);
            }
            _actionTimeoutCoroutines.Remove(action);
        }
        
        private IEnumerator ProgressGrowthAction(GrowthAction action, IPlantTool tool)
        {
            while (_growthActionProgressDictionary.ContainsKey(action))
            {
                if (!tool.isToolActive)
                {
                    // If the tool becomes inactive, wait until it becomes active again
                    yield return null;
                    continue;
                }

                _growthActionProgressDictionary[action] += tool.completionRatePerSecond * Time.deltaTime;

                // Check if progress is complete
                if (_growthActionProgressDictionary[action] >= action.progressTarget)
                {
                    SetGrowthActionCompleted(action);
                    _activeCoroutines.Remove((action, (tool as Component)?.gameObject));
                    yield break;
                }

                yield return null;
            }
        }
        
        private void SetGrowthActionCompleted(GrowthAction action)
        {
            if (!_growthActionProgressDictionary.Remove(action))
                return;
            
            // Stop the dying timeout coroutine
            if (_actionTimeoutCoroutines.TryGetValue(action, out Coroutine timeout))
            {
                StopCoroutine(timeout);
                _actionTimeoutCoroutines.Remove(action);
            }
            
            // Stop all coroutines associated with this action
            var keysToRemove = new List<(GrowthAction, GameObject)>();
            foreach (var kvp in _activeCoroutines)
            {
                if (kvp.Key.action != action) 
                    continue;
                StopCoroutine(kvp.Value);
                keysToRemove.Add(kvp.Key);
            }
            foreach (var key in keysToRemove)
                _activeCoroutines.Remove(key);
            
            // Check if all actions are completed, this way we can efficiently transition to the next state following an observer pattern instead of relying on update ticks.
            if (_growthActionProgressDictionary.Count == 0)
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
            _growthActionProgressDictionary.Clear();
            foreach (var growthAction in seedData.GetLifeStateConfig(lifeState).growthActions)
            {
                growthAction.SetupAction(this);
                _growthActionProgressDictionary.Add(growthAction, 0);
                // Start a coroutine per action to handle the dying timeout of the plant
                Coroutine timeout = StartCoroutine(ActionTimeout(growthAction));
                _actionTimeoutCoroutines[growthAction] = timeout;
            }
        }

        public void TransitionToState(LifeState lifeState)
        {
            if (_currentLifeState == lifeState)
                return;

            if (_plantPrefabInstance)
                Destroy(_plantPrefabInstance);

            SetupState(lifeState);

            RecheckOverlaps();
        }

        private void HandleToolInteraction(IPlantTool tool, GameObject toolObject)
        {
            foreach (var growthAction in _growthActionProgressDictionary.Keys.ToList())
            {
                if (growthAction.toolType != tool.toolType)
                    continue;

                if (growthAction.isInstantCompletion)
                {
                    SetGrowthActionCompleted(growthAction);
                }
                else
                {
                    var key = (growthAction, toolObject);
                    if (_activeCoroutines.ContainsKey(key))
                        continue;

                    Coroutine routine = StartCoroutine(ProgressGrowthAction(growthAction, tool));
                    _activeCoroutines[key] = routine;
                }
            }
        }
        
        private void RecheckOverlaps()
        {
            foreach (var plantTool in _overlappingTools)
            {
                if (plantTool != null && plantTool.isToolActive)
                {
                    HandleToolInteraction(plantTool, (plantTool as Component)?.gameObject);
                }
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("PlantTool"))
                return;
            Debug.Log("Plant tool entered");

            var plantTool = other.GetComponent<IPlantTool>();
            if (plantTool == null || !plantTool.isToolActive)
                return;
            
            if (!_overlappingTools.Contains(plantTool))
                _overlappingTools.Add(plantTool);
            
            HandleToolInteraction(plantTool, other.gameObject);
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("PlantTool"))
                return;
            
            var plantTool = other.GetComponent<IPlantTool>();
            if (plantTool != null && _overlappingTools.Contains(plantTool))
                _overlappingTools.Remove(plantTool);

            // Stop all coroutines associated with this tool
            var keysToRemove = new List<(GrowthAction, GameObject)>();
            foreach (var kvp in _activeCoroutines)
            {
                if (kvp.Key.tool != other.gameObject) 
                    continue;
                StopCoroutine(kvp.Value);
                keysToRemove.Add(kvp.Key);
            }
            foreach (var key in keysToRemove)
                _activeCoroutines.Remove(key);
        }

        private void Start()
        {
            if (!seedData)
                return;
            
            SetupState(LifeState.Seed);
        }

        private void OnDisable()
        {
            //To avoid memory leaks, stop all coroutines when the object is disabled.
            foreach (var kvp in _activeCoroutines)
            {
                if (kvp.Value != null)
                    StopCoroutine(kvp.Value);
            }
            _activeCoroutines.Clear();
        }
    }
}

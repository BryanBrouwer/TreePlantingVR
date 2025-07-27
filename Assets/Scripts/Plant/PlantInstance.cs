using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Plant.GrowthActions;
using Plant.State;
using Tool;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Plant
{
    public class PlantInstance : MonoBehaviour
    {
        public SeedData seedData;
        [field: SerializeField] public Collider plantCollider { get; private set; }
        
        private LifeState _currentLifeState;
        private GameObject _plantPrefabInstance;
        
        private readonly List<GrowthActionRuntime> _activeGrowthActions = new List<GrowthActionRuntime>();
        private readonly Dictionary<(GrowthActionRuntime action, GameObject tool), Coroutine> _activeCoroutines = new Dictionary<(GrowthActionRuntime action, GameObject tool), Coroutine>();
        private readonly List<IPlantTool> _overlappingTools = new List<IPlantTool>();
        
        private IEnumerator ProgressGrowthAction(GrowthActionRuntime actionRuntime, IPlantTool tool)
        {
            while (_activeGrowthActions.Contains(actionRuntime))
            {
                if (!tool.isToolActive)
                {
                    // If the tool becomes inactive, wait until it becomes active again
                    yield return null;
                    continue;
                }

                actionRuntime.AddProgress(tool.completionRatePerSecond * Time.deltaTime);;

                // Check if progress is complete
                if (actionRuntime.IsComplete())
                {
                    SetGrowthActionCompleted(actionRuntime);
                    _activeCoroutines.Remove((actionRuntime, (tool as Component)?.gameObject));
                    yield break;
                }

                yield return null;
            }
        }
        
        private void SetGrowthActionCompleted(GrowthActionRuntime actionRuntime)
        {
            if (!_activeGrowthActions.Contains(actionRuntime))
                return;
            
            actionRuntime.StopTimeout();
            actionRuntime.OnTimeout -= OnActionTimeout;
            _activeGrowthActions.Remove(actionRuntime);
            
            // Stop all coroutines associated with this action
            var keysToRemove = new List<(GrowthActionRuntime, GameObject)>();
            foreach (var kvp in _activeCoroutines)
            {
                if (kvp.Key.action != actionRuntime) 
                    continue;
                StopCoroutine(kvp.Value);
                keysToRemove.Add(kvp.Key);
            }
            foreach (var key in keysToRemove)
                _activeCoroutines.Remove(key);
            
            // Check if all actions are completed, this way we can efficiently transition to the next state following an observer pattern instead of relying on update ticks.
            if (_activeGrowthActions.Count == 0)
                TransitionToState(seedData.GetLifeStateConfig(_currentLifeState).nextLifeState);
        }
        
        public void SetupState(LifeState lifeState)
        {
            var lifeStateConfig = seedData.GetLifeStateConfig(lifeState);
            _plantPrefabInstance = Instantiate(lifeStateConfig.plantPrefab, transform);
            _currentLifeState = lifeState;
            
            ScoreManager.Instance.AddScore(lifeStateConfig.scorePoints);

            // If there is no next life state, we are done.
            if (!seedData.GetLifeStateConfig(_currentLifeState).hasNextLifeState)
                return;
            
            // Setting up new growth actions and delegate subscriptions.
            foreach (var growthAction in seedData.GetLifeStateConfig(lifeState).growthActions)
            {
                growthAction.SetupAction(this);
                GrowthActionRuntime newActionRuntime = new GrowthActionRuntime(this, growthAction);
                newActionRuntime.OnTimeout += OnActionTimeout;
                newActionRuntime.StartTimeout();
                _activeGrowthActions.Add(newActionRuntime);
            }
        }
        
        private void OnActionTimeout(GrowthActionRuntime actionRuntime)
        {
            TransitionToState(LifeState.Dead);
        }
        
        public void TransitionToState(LifeState lifeState)
        {
            if (_currentLifeState == lifeState)
                return;
            
            foreach (var runtime in _activeGrowthActions)
            {
                runtime.StopTimeout();
                runtime.OnTimeout -= OnActionTimeout;
            }
            _activeGrowthActions.Clear();

            if (_plantPrefabInstance)
                Destroy(_plantPrefabInstance);

            SetupState(lifeState);
            RecheckOverlaps();
        }

        public void Initialize(SeedData seedData)
        {
            this.seedData = seedData;
            SetupState(LifeState.Seed);
            PlantInstanceManager.Instance.AddPlantInstance(this);
        }

        private void HandleToolInteraction(IPlantTool tool, GameObject toolObject)
        {
            foreach (var actionRuntime in _activeGrowthActions.ToList())
            {
                if (actionRuntime.growthAction.toolType != tool.toolType)
                    continue;

                if (actionRuntime.growthAction.isInstantCompletion)
                {
                    SetGrowthActionCompleted(actionRuntime);
                }
                else
                {
                    var key = (actionRuntime, toolObject);
                    if (_activeCoroutines.ContainsKey(key))
                        continue;

                    Coroutine routine = StartCoroutine(ProgressGrowthAction(actionRuntime, tool));
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
            var keysToRemove = new List<(GrowthActionRuntime, GameObject)>();
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

        private void OnEnable()
        {
            if (!seedData)
                return;
            
            PlantInstanceManager.Instance.AddPlantInstance(this);
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
            
            PlantInstanceManager.Instance.RemovePlantInstance(this);
        }
    }
}

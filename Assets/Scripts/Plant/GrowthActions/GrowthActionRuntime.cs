using System;
using System.Collections;
using Plant.State;
using UnityEngine;

namespace Plant.GrowthActions
{
    public class GrowthActionRuntime
    {
        public GrowthAction growthAction { get; private set; }
        public float currentProgress { get; private set; } = 0;
        public float timeRemaining { get; private set; }
        
        private readonly PlantInstance _owner;
        private Coroutine _timeoutCoroutine;
        
        public event Action<GrowthActionRuntime, int> OnTimeoutIntervalChanged;
        public event Action<GrowthActionRuntime> OnTimeout;
        
        public GrowthActionRuntime(PlantInstance owner, GrowthAction growthAction)
        {
            _owner = owner;
            this.growthAction = growthAction;
            timeRemaining = growthAction.timeToComplete;
            currentProgress = 0;
        }
        
        private IEnumerator ActionTimeout()
        {
            float totalTime = growthAction.timeToComplete;
            float interval = growthAction.timeToComplete / 3f;
            int currentInterval = 0;

            while (timeRemaining > 0f)
            {
                yield return null;
                timeRemaining -= Time.deltaTime;
                
                int newInterval = Mathf.FloorToInt((totalTime - timeRemaining) / interval);
                if (newInterval != currentInterval)
                {
                    currentInterval = newInterval;
                    OnTimeoutIntervalChanged?.Invoke(this, currentInterval);
                    // TODO: Link future UI to this event
                    Debug.Log("Current interval changed");
                }
            }
            // Since it was not canceled, we can assume it timed out.
            OnTimeout?.Invoke(this);
        }

        public void StartTimeout()
        {
            // In case the coroutine might still be running.
            StopTimeout();
            _timeoutCoroutine = _owner.StartCoroutine(ActionTimeout());
        }
        
        public void StopTimeout()
        {
            if (_timeoutCoroutine != null)
            {
                _owner.StopCoroutine(_timeoutCoroutine);
                _timeoutCoroutine = null;
            }
        }
        
        public void AddProgress(float amount)
        {
            currentProgress += amount;
        }

        public bool IsComplete()
        {
            return currentProgress >= growthAction.progressTarget;
        }
    }
}
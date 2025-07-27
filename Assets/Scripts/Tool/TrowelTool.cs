using System;
using UnityEngine;

namespace Tool
{
    public class TrowelTool : MonoBehaviour, IPlantTool
    {
        public float completionRatePerSecond { get; } = 10;
        public bool isToolActive { get; } = true;
        public ToolType toolType { get; } = ToolType.Weed;
        public event Action<IPlantTool> OnToolActivated;
        public event Action<IPlantTool> OnToolDeactivated;

        private void OnOnToolActivated(IPlantTool obj)
        {
            OnToolActivated?.Invoke(obj);
        }

        private void OnOnToolDeactivated(IPlantTool obj)
        {
            OnToolDeactivated?.Invoke(obj);
        }
    }
}
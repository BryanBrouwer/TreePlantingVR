using System;
using UnityEngine;

namespace Tool
{
    public interface IPlantTool
    {
        float completionRatePerSecond { get; }
        bool isToolActive { get; }
        ToolType toolType { get; }
        event Action<IPlantTool> OnToolActivated;
        event Action<IPlantTool> OnToolDeactivated;
    }
}
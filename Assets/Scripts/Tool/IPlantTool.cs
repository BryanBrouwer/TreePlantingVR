using UnityEngine;

namespace Tool
{
    public interface IPlantTool
    {
        float completionRatePerSecond { get; }
        bool isToolActive { get; }
        ToolType toolType { get; }
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Plant.Condition
{
    // Conditions that checks if the plantable ground underneath the seed has a certain material.
    [CreateAssetMenu(fileName = "NewGroundMaterialCondition", menuName = "Scriptable Objects/Plant/Condition/Ground Material", order = 0)]
    public class GroundMaterialCondition : PlantCondition
    {
        [field: SerializeField] public Material relevantMaterial { get; private set; }
        [field: SerializeField] public bool shouldBeMaterial { get; private set; } = true;
        
        public override bool CheckCondition(PlantableSeed seed)
        {
            // Raycast from the seed position + offset downward to get the correct object, and then check if it has the allowed material.
            LayerMask layerMask = LayerMask.GetMask("Default");
            Vector3 startPos = seed.GetPlantPosition() + Vector3.up * 0.5f;
            Vector3 direction = Vector3.down;
            if (Physics.Raycast(startPos, direction, out var hit, 5, layerMask))
            {
                if (!hit.collider.CompareTag("PlantableSurface"))
                    return false;
                
                if (hit.collider.TryGetComponent<Renderer>(out var renderer))
                {
                    // In a search found that shared materials are the original materials as in the editor, so we can just check if the material is in this list to avoid dealing with instanced mats.
                    if (renderer.sharedMaterials.Contains(relevantMaterial))
                        return shouldBeMaterial;
                }
            }
            return !shouldBeMaterial;
        }
    }
}
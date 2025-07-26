using UnityEngine;

namespace Plant
{
    [CreateAssetMenu(fileName = "NewPlantType", menuName = "Scriptable Objects/Plant/PlantType", order = 0)]
    public class PlantType : ScriptableObject
    {
        public string plantName;
        public Sprite plantImage;
        public string plantDescription;
    }
}
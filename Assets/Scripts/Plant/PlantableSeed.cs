using System.Linq;
using Plant.Condition;
using UnityEngine;

namespace Plant
{
    public class PlantableSeed : MonoBehaviour
    {
        public SeedData seedData;
        [field: SerializeField] public Collider seedCollider { get; private set; }
        [field: SerializeField] public GameObject plantPrefab { get; private set; }

        //Checks if all conditions are met to plant the seed.
        public bool CheckPlantable()
        {
            PlantCondition[] plantConditions = seedData.conditions;
            //Since we only need to loop over one function, we can use the All() method from .Net
            return plantConditions.All(condition => condition.CheckCondition());
        }
        
        public void AttemptPlant()
        {
            if (CheckPlantable())
            {
                //Get the hit point between the seed and ground colliders, then instantiate the plant prefab.
                
                Plant(transform);
            }
        }

        private void Plant(Transform plantTransform)
        {
            Instantiate(plantPrefab, plantTransform.position, plantTransform.rotation);
            Destroy(gameObject);
        }
    }
}
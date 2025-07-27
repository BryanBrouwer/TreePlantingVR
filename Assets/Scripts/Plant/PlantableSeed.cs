using System.Linq;
using Plant.Condition;
using Plant.State;
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
            //Since we only need to loop over one function, we can use the All() enumerator from .Net
            return plantConditions.All(condition => condition.CheckCondition(this));
        }
        
        public void AttemptPlant()
        {
            if (CheckPlantable())
            {
                Plant(transform);
            }
        }

        private void Plant(Transform plantTransform)
        {
            var newPlant = Instantiate(plantPrefab, plantTransform.position, Quaternion.identity);
            PlantInstance plantInstance = newPlant.GetComponent<PlantInstance>();
            if (!plantInstance)
            {
                plantInstance = newPlant.AddComponent<PlantInstance>();
            }
            plantInstance.Initialize(seedData);
            
            Destroy(gameObject);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("PlantableSurface"))
            {
                AttemptPlant();
            }
        }
    }
}
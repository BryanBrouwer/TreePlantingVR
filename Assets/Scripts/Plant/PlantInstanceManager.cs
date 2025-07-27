using System.Collections.Generic;

namespace Plant
{
    public class PlantInstanceManager
    {
        private static PlantInstanceManager _instance;

        public static PlantInstanceManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PlantInstanceManager();
                return _instance;
            }
        }
        
        private readonly Dictionary<PlantType, List<PlantInstance>> _plantInstances = new Dictionary<PlantType, List<PlantInstance>>();
        
        private PlantInstanceManager()
        {
        }

        public void AddPlantInstance(PlantInstance plantInstance)
        {
            if (!_plantInstances.ContainsKey(plantInstance.seedData.plantType))
                _plantInstances.Add(plantInstance.seedData.plantType, new List<PlantInstance>());
            
            if (!_plantInstances[plantInstance.seedData.plantType].Contains(plantInstance))
                _plantInstances[plantInstance.seedData.plantType].Add(plantInstance);
        }
        
        public void RemovePlantInstance(PlantInstance plantInstance)
        {
            if (_plantInstances.TryGetValue(plantInstance.seedData.plantType, out var instance))
                instance.Remove(plantInstance);
        }
        
        public List<PlantInstance> GetPlantInstancesOfType(PlantType plantType)
        {
            if (!_plantInstances.ContainsKey(plantType))
                return new List<PlantInstance>();
            
            return _plantInstances[plantType];
        }

        public List<PlantInstance> GetAllPlantInstances()
        {
            var allPlantInstances = new List<PlantInstance>();
            foreach (var plantInstances in _plantInstances.Values)
            {
                allPlantInstances.AddRange(plantInstances);
            }
            return allPlantInstances;
        }
    }
}
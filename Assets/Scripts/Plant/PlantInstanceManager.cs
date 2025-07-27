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
            _plantInstances[plantInstance.seedData.plantType].Add(plantInstance);
        }
        
        public void RemovePlantInstance(PlantInstance plantInstance)
        {
            if (_plantInstances.TryGetValue(plantInstance.seedData.plantType, out var instance))
                instance.Remove(plantInstance);
        }
        
        public List<PlantInstance> GetPlantInstances(PlantType plantType)
        {
            return _plantInstances[plantType];
        }
    }
}
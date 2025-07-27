using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Plant
{
    public class SeedSpawner : MonoBehaviour
    {
        [field: SerializeField] public SeedData seedData { get; private set;}
        [SerializeField] private TextMeshProUGUI seedNameText;
        [SerializeField] private TextMeshProUGUI seedDescriptionText;
        [SerializeField] private Image seedImage;
        [SerializeField] private GameObject seedPrefab;
        private PlantableSeed _seedInstance;

        private void OnSessionStart()
        {
            SpawnSeed();
        }
        
        private void OnSessionEnd()
        {
            DestroySeed();
        }
        
        private void CleanupSeed(PlantableSeed seed)
        {
            _seedInstance.OnSeedDestroyed -= CleanupSeed;
            _seedInstance = null;
            if (SessionManager.Instance.isSessionActive)
                SpawnSeed();
        }
        
        public void SpawnSeed()
        {
            if (_seedInstance)
                return;
            var seedObject = Instantiate(seedPrefab, transform.position, Quaternion.identity);
            _seedInstance = seedObject.GetComponent<PlantableSeed>();
            _seedInstance.seedData = seedData;

            if (seedData.seedBagsMesh)
            {
                var meshFilter = seedObject.GetComponent<MeshFilter>();
                meshFilter.mesh = seedData.seedBagsMesh;
            }
            
            _seedInstance.OnSeedDestroyed += CleanupSeed;
        }
        
        public void DestroySeed()
        {
            _seedInstance.OnSeedDestroyed -= CleanupSeed;
            if (_seedInstance)
                Destroy(_seedInstance.gameObject);
            _seedInstance = null;
        }

        private void OnEnable()
        {
            seedNameText.text = seedData.plantType.plantName;
            seedDescriptionText.text = seedData.plantType.plantDescription;
            seedImage.sprite = seedData.plantType.plantImage;
            
            // Subscribe to the session manager to control seed spawning for a fluent session experience, this way you can finish any plants already planted, but any seeds left will disappear.
            SessionManager.Instance.OnSessionStarted += OnSessionStart;
            SessionManager.Instance.OnSessionEnded += OnSessionEnd;
        }
        
        private void OnDisable()
        {
            SessionManager.Instance.OnSessionStarted -= OnSessionStart;
            SessionManager.Instance.OnSessionEnded -= OnSessionEnd;
        }
    }
}
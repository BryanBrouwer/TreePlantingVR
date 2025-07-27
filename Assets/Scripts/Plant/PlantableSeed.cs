using System;
using System.Collections;
using System.Linq;
using Plant.Condition;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Plant
{
    [RequireComponent(typeof(LineRenderer))]
    public class PlantableSeed : MonoBehaviour
    {
        public SeedData seedData;
        [field: SerializeField] public Collider seedCollider { get; private set; }
        [field: SerializeField] public GameObject plantPrefab { get; private set; }
        [field: SerializeField] public float maxRaycastDistance { get; private set; } = 10;
        private bool isPlantingMode { get; set; } = false;
        private Coroutine _plantingModeCoroutine;
        private GameObject _plantPreview;
        private LineRenderer _lineRenderer;
        private bool _isCurrentlyPlantable = false;
        public event Action<PlantableSeed> OnSeedDestroyed;

        //Checks if all conditions are met to plant the seed.
        private bool CheckPlantable()
        {
            PlantCondition[] plantConditions = seedData.conditions;
            //Since we only need to loop over one function, we can use the All() enumerator from .Net
            return plantConditions.All(condition => condition.CheckCondition(this));
        }

        private void AttemptPlant(Vector3 plantPosition)
        {
            //We double-check that the conditions are actually currently met before planting.
            if (CheckPlantable())
            {
                Plant(plantPosition);
            }
        }

        private void Plant(Vector3 plantPosition)
        {
            var newPlant = Instantiate(plantPrefab, plantPosition, Quaternion.identity);
            PlantInstance plantInstance = newPlant.GetComponent<PlantInstance>();
            if (!plantInstance)
            {
                plantInstance = newPlant.AddComponent<PlantInstance>();
            }
            plantInstance.Initialize(seedData);
            OnSeedDestroyed?.Invoke(this);
            Destroy(gameObject);
        }
        
        private IEnumerator PlantingMode()
        {
            var plantPreviewRenderer = _plantPreview.GetComponent<Renderer>();
            while (isPlantingMode && _plantPreview != null)
            {
                LayerMask layerMask = LayerMask.GetMask("Default");
                Vector3 startPos = transform.position;
                Vector3 direction = transform.TransformDirection(Vector3.forward);
                _isCurrentlyPlantable = false;
                _lineRenderer.enabled = false;

                if (Physics.Raycast(startPos, direction, out var hit, maxRaycastDistance, layerMask))
                {
                    _plantPreview.SetActive(true);
                    _plantPreview.transform.position = hit.point;
                    _lineRenderer.SetPosition(0, startPos);
                    _lineRenderer.SetPosition(1, hit.point);
                    _lineRenderer.enabled = true;
                    Color targetColor = Color.red;
                    
                    if (hit.collider.CompareTag("PlantableSurface"))
                        targetColor = Color.green;

                    if (CheckPlantable())
                    {
                        _isCurrentlyPlantable = true;
                        foreach (var material in plantPreviewRenderer.materials)
                        {
                            material.color = Color.green;
                        }
                    }
                    else
                    {
                        _isCurrentlyPlantable = false;
                        foreach (var material in plantPreviewRenderer.materials)
                        {
                            material.color = Color.red;
                        }
                    }
                    
                    _lineRenderer.startColor = targetColor;
                    _lineRenderer.endColor = targetColor;
                }
                else
                {
                    _plantPreview.SetActive(false);
                    _lineRenderer.enabled = false;
                    _isCurrentlyPlantable = false;
                }
                
                yield return null;
            }
        }
        
        private void EnablePlantingMode(ActivateEventArgs arg0)
        {
            _plantPreview = Instantiate(seedData.plantPreviewPrefab, transform.position, Quaternion.identity);
            _plantPreview.transform.localScale = Vector3.one * 0.5f;
            _plantPreview.GetComponent<Renderer>().material.color = Color.green;
            _plantPreview.SetActive(false);
            isPlantingMode = true;
            _plantingModeCoroutine = StartCoroutine(PlantingMode());
        }
        
        private void DisablePlantingMode(DeactivateEventArgs arg0)
        {
            if (_plantPreview)
            {
                if (_isCurrentlyPlantable)
                    AttemptPlant(_plantPreview.transform.position);
                Destroy(_plantPreview);
            }

            if (_plantingModeCoroutine != null)
                StopCoroutine(_plantingModeCoroutine);
            isPlantingMode = false;
            _lineRenderer.enabled = false;
        }

        private void OnSelectExit(SelectExitEventArgs arg0)
        {
            if (isPlantingMode)
            {
                DisablePlantingMode(null);
            }
        }
        private void OnEnable()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.positionCount = 2;
            var xrGrabInteractable = gameObject.GetComponent<XRGrabInteractable>();
            xrGrabInteractable.activated.AddListener(EnablePlantingMode);
            xrGrabInteractable.deactivated.AddListener(DisablePlantingMode);
            xrGrabInteractable.selectExited.AddListener(OnSelectExit);
        }
        
        private void OnDisable()
        {
            var xrGrabInteractable = gameObject.GetComponent<XRGrabInteractable>();
            xrGrabInteractable.activated.RemoveListener(EnablePlantingMode);
            xrGrabInteractable.deactivated.RemoveListener(DisablePlantingMode);
            xrGrabInteractable.selectExited.RemoveListener(OnSelectExit);
            if (_plantPreview)
                Destroy(_plantPreview);
        }

        public Vector3 GetPlantPosition()
        {
            return _plantPreview ? _plantPreview.transform.position : transform.position;
        }
    }
}
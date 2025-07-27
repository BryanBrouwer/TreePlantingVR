using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tool
{
    public class WateringCan : MonoBehaviour, IPlantTool
    { 
        [field: SerializeField] public float completionRatePerSecond { get; private set; } = 20;
        public bool isToolActive { get; private set; } = false;
        public ToolType toolType { get; private set; } = ToolType.Water;
        public event Action<IPlantTool> OnToolActivated;
        public event Action<IPlantTool> OnToolDeactivated;
        [SerializeField] private GameObject waterVisuals;
        [SerializeField] private float tiltAngleThreshold = 25;
        private ParticleSystem _particles;

        private void OnEnable()
        {
            _particles = waterVisuals.GetComponent<ParticleSystem>();
        }

        public void Update()
        {
            // Checking if the forward vector is tilted downwards enough to be considered active.
            Vector3 forward = transform.forward;
            float angleToDown = Vector3.Angle(forward, Vector3.down);
            var newToolActiveState = angleToDown < tiltAngleThreshold;
            if (newToolActiveState != isToolActive)
            {
                isToolActive = newToolActiveState;
                if (isToolActive)
                    OnToolActivated?.Invoke(this);
                else
                    OnToolDeactivated?.Invoke(this);
            }

            // Showing or hiding the water visuals.
            if (isToolActive != waterVisuals.activeSelf)
            {
                waterVisuals.SetActive(isToolActive);
                if (!_particles) return;
                if (isToolActive)
                {
                    if (!_particles.isPlaying)
                        _particles.Play();
                }
                else
                {
                    _particles.Stop();
                }
            }
        }
    }
}
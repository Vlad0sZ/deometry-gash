using UnityEngine;

namespace GameplayScripts.Runtime
{
    public class VFXSystem : MonoBehaviour
    {
        [SerializeField] private ParticleSystem diedParticles;

        private void OnEnable()
        {
            EventManager.OnPlayerDied += OnDied;
        }

        private void OnDisable()
        {
            EventManager.OnPlayerDied -= OnDied;
        }

        private void OnDied(Vector2 position)
        {
            if (diedParticles == null) return;
            Instantiate(diedParticles, position, Quaternion.identity);
        }
    }
}
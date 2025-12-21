using UnityEngine;

namespace animation
{
    /// <summary>
    /// Spawns and manages visual effects
    /// </summary>
    public class VFXSpawner : MonoBehaviour
    {
        public static VFXSpawner Instance;
        
        [Header("Default VFX")]
        [SerializeField] private GameObject defaultHitVFX;
        [SerializeField] private GameObject defaultHealVFX;
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        
        /// <summary>
        /// Spawns a VFX prefab at position and auto-destroys it
        /// </summary>
        public GameObject SpawnVFX(GameObject vfxPrefab, Vector3 position, float lifetime = 2f)
        {
            if (vfxPrefab == null)
            {
                Debug.LogWarning("Tried to spawn null VFX");
                return null;
            }
            
            GameObject vfx = Instantiate(vfxPrefab, position, Quaternion.identity);
            Destroy(vfx, lifetime);
            return vfx;
        }
        
        public void SpawnHitVFX(Vector3 position)
        {
            if (defaultHitVFX != null)
                SpawnVFX(defaultHitVFX, position);
        }
        
        public void SpawnHealVFX(Vector3 position)
        {
            if (defaultHealVFX != null)
                SpawnVFX(defaultHealVFX, position);
        }
    }
}
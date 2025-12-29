using UnityEngine;

namespace persistence
{
    [System.Serializable]
    public class HeroData
    {
        public string heroId;
        public Sprite icon;
        public string prefabName;       // Name of the prefab to spawn (e.g., "KnightPrefab")
        public int currentHealth;       // health to persist between battles
        public int experienceGained;    // XP tracking
        
        public HeroData(string prefabName)
        {
            heroId = System.Guid.NewGuid().ToString();
            this.prefabName = prefabName;
            currentHealth = -1; // -1 means "use prefab default"
            experienceGained = 0;
        }
        
        public HeroData(string prefabName, Sprite icon)
        {
            heroId = System.Guid.NewGuid().ToString();
            this.prefabName = prefabName;
            this.icon = icon;
            currentHealth = -1; // -1 means "use prefab default"
            experienceGained = 0;
        }
    }
}
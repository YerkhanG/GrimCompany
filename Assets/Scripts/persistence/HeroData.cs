using UnityEngine;

namespace persistence
{
    [System.Serializable]
    public class HeroData
    {
        public string heroId;
        public Sprite icon;
        public string prefabName;       // Name of the prefab to spawn (e.g., "KnightPrefab")
        public int currentHealth; 
        public int experienceGained;  
        
        public HeroData(string prefabName)
        {
            heroId = System.Guid.NewGuid().ToString();
            this.prefabName = prefabName;
        }
        public HeroData(string prefabName, Sprite icon)
        {
            heroId = System.Guid.NewGuid().ToString();
            this.prefabName = prefabName;
            this.icon = icon;
        }
        public HeroData(string prefabName, Sprite icon, int hp)
        {
            heroId = System.Guid.NewGuid().ToString();
            this.prefabName = prefabName;
            this.icon = icon;
            currentHealth = hp;
        }
        public void UpdateHP(int newCurrentHP)
        {
            currentHealth = newCurrentHP;
        }
    }
}
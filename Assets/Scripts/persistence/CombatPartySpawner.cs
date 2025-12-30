using System.Collections.Generic;
using combat;
using entity;
using UnityEngine;

namespace persistence
{
    public class CombatPartySpawner : MonoBehaviour
    {
        [Header("Hero prefabs")]
        [Tooltip("Reference to all prefabs for the sake of loading the chosen ones in battle later on")]
        [SerializeField] private List<GameObject> heroPrefabs = new List<GameObject>();
        
        [Header("Runtime")]
        private List<GameObject> _spawnedHeroes = new List<GameObject>();
        
        private void Awake()
        {
            if (PositionManager.Instance == null)
            {
                Debug.LogError("PositionManager not initialized!");
                return;
            }
    
            if (CombatManager.Instance == null)
            {
                Debug.LogError("CombatManager not initialized!");
                return;
            }
            SpawnParty();
        }

        private void SpawnParty()
        {
            // The spawn Hero function is for the GameObject instantiation.
            // The register with combat manager function is for the backend operations.
            // It just puts the Entity classes inside the combat manager. 
            var party = RunManager.Instance.party;
            for(int i =0;i< party.Count;i++)
            {
                HeroData hero =  party[i];    
                SpawnHero(hero, i);
            }
            RegisterWithCombatManager();
        }
        
        private void SpawnHero(HeroData heroData, int i)
        {
            var prefab = FindPrefabByName(heroData);
            
            if (prefab == null)
            {
                Debug.LogError($"[CombatPartySpawner] Prefab '{heroData.prefabName}' not found! " +
                               $"Make sure it's in the heroPrefabs list and the name matches exactly.");
                return;
            }
            var spawnPoint = PositionManager.Instance.playableCharPositions[i];
            var spawnedHero = Instantiate(prefab,spawnPoint.position, Quaternion.identity);
            spawnedHero.GetComponentInChildren<PlayableCharacter>().currentHealth = heroData.currentHealth;
            spawnedHero.transform.SetParent(spawnPoint);
            _spawnedHeroes.Add(spawnedHero);
        }

        private GameObject FindPrefabByName(HeroData heroData)
        {
            if (heroPrefabs == null || heroPrefabs.Count == 0)
            {
                Debug.LogError("[CombatPartySpawner] heroPrefabs list is empty! Assign hero prefabs in inspector.");
                return null;
            }

            foreach (var prefab in heroPrefabs)
            {
                if (prefab != null && prefab.name == heroData.prefabName)
                    return prefab;
            }
            return null;
        }

        private void RegisterWithCombatManager()
        {
            foreach (var hero in _spawnedHeroes)
            {
                var spawnedHero = hero.GetComponentInChildren<PlayableCharacter>();
                CombatManager.Instance.playerList.Add(spawnedHero);
            }
        }
    }
}
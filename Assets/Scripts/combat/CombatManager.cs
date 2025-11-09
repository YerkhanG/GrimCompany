using System;
using System.Collections.Generic;
using System.Linq;
using entity;
using combat;
using events;
using UnityEditor;
using UnityEngine;

namespace controller.combat
{
    public class CombatManager : MonoBehaviour
    {
        public static CombatManager Instance;
        [Header("Teams reference")]
        public List<Entity> playerList = new List<Entity>();
        public List<Entity> enemyList = new  List<Entity>();
        public List<Entity> allCombatants = new  List<Entity>();
        [Header("State")]
        public CombatState combatState;
        
        private Queue<Entity> turnQueue = new Queue<Entity>();
        private Entity currentActor;

        public Entity getCurrentActor()
        {
            return currentActor;
        }
        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        private void Start()
        {
            SetUpCombat();
        }

        private void SetUpCombat()
        {
            combatState = CombatState.Setup;
            allCombatants.AddRange(playerList);
            allCombatants.AddRange(enemyList);
            allCombatants.Sort((a, b) => b.ActionSpeed.CompareTo(a.ActionSpeed));

            CreateActionQueue(allCombatants);
            StartNextTurn();
        }
        public void EndCurrentTurn()
        {
            if (currentActor != null)
            {
                currentActor.EndTurn();
            }
            StartNextTurn();
        }
        private void StartNextTurn()
        {
            Debug.Log($"StartNextTurn called - Frame: {Time.frameCount}");
            if (playerList.All(e => !e.isAlive))
            {
                LoseGame();
                return;
            }
            if (enemyList.All(e => !e.isAlive))
            {
                WinGame();
                return;
            }
            if (turnQueue.Count == 0)
            {
                CreateActionQueue(allCombatants);    
            }
            currentActor = turnQueue.Dequeue();
            if (!currentActor.isAlive)
            {
                StartNextTurn();
                return;
            }

            if (currentActor.isPlayable) combatState = CombatState.PlayerTurn;
            else combatState = CombatState.EnemyTurn;
            currentActor.StartTurn();
        }

        private void LoseGame()
        {
            Debug.Log($"LoseGame called - Frame: {Time.frameCount}");
            combatState = CombatState.Defeat;
        }

        private void WinGame()
        {
            Debug.Log($"WinGame called - Frame: {Time.frameCount}");
            combatState = CombatState.Victory;
        }
        
        private void CreateActionQueue(List<Entity> allCombatants)
        {
            foreach (var e in allCombatants)
            {
                if (e.isAlive)
                {
                    turnQueue.Enqueue(e);
                }
            }
        }
        void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
            CombatEvents.ClearAllEvents();
        }
    }
}
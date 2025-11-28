using System;
using System.Collections.Generic;
using System.Linq;
using entity;
using combat;
using events;
using UnityEditor;
using UnityEngine;

namespace combat
{
    public class CombatManager : MonoBehaviour
    {
        public static CombatManager Instance;
        [Header("Teams reference")] public List<Entity> playerList = new List<Entity>();
        public List<Entity> enemyList = new List<Entity>();
        public List<Entity> allCombatants = new List<Entity>();
        [Header("State")] public CombatState combatState;
        [Header("Managers")] [SerializeField] private PositionManager positionManager;
        private Queue<Entity> turnQueue = new Queue<Entity>();
        private Entity currentActor;

        public Entity getCurrentActor()
        {
            return currentActor;
        }

        private void Awake()
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
            SetupEntityPositions();
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
            CombatEvents.RaiseCurrentActorPicked(currentActor);
        }

        private void LoseGame()
        {
            Debug.Log($"LoseGame called - Frame: {Time.frameCount}");
            combatState = CombatState.Defeat;

            // Notify the run / map system
            if (RunManager.Instance != null)
            {
                RunManager.Instance.OnBattleLost();
            }
        }

        private void WinGame()
        {
            Debug.Log($"WinGame called - Frame: {Time.frameCount}");
            combatState = CombatState.Victory;

            // Notify the run / map system
            if (RunManager.Instance != null)
            {
                RunManager.Instance.OnBattleWon();
            }
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

        private void SetupEntityPositions()
        {
            // Register player positions
            for (int i = 0; i < playerList.Count; i++)
            {
                positionManager.RegisterEntityPosition(playerList[i], i);
                if (i < positionManager.playableCharPositions.Count)
                {
                    playerList[i].transform.position = positionManager.playableCharPositions[i].position;
                }
            }

            // Register enemy positions
            for (int i = 0; i < enemyList.Count; i++)
            {
                positionManager.RegisterEntityPosition(enemyList[i], i);

                if (i < positionManager.enemyPositions.Count)
                {
                    enemyList[i].transform.position = positionManager.enemyPositions[i].position;
                }
            }

            Debug.Log($"Registered {playerList.Count} player positions and {enemyList.Count} enemy positions");
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
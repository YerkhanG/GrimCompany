using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using data;
using entity;
using events;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace combat
{
    public class PositionManager : MonoBehaviour
    {
        public static PositionManager Instance;
        [Header("Positions")]
        public List<Transform> playableCharPositions = new List<Transform>(4);
        public List<Transform> enemyPositions = new List<Transform>(4);
        
        [Header("Repositioning Settings")]
        [SerializeField] private bool autoRepositionOnDeath = true;
        [SerializeField] private float repositionDelay = 0.3f;
        
        private Dictionary<Entity, int> entityPositions = new Dictionary<Entity, int>();
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        void OnEnable()
        {
            CombatEvents.OnAttackButtonClicked += HandleAttackRequest;
            CombatEvents.OnUtilityButtonClicked += HandleUtilityButtonClicked;
            CombatEvents.OnEntityDied += HandleEntityDeath;
        }

        void OnDisable()
        {
            CombatEvents.OnAttackButtonClicked -= HandleAttackRequest;
            CombatEvents.OnUtilityButtonClicked -= HandleUtilityButtonClicked;
            CombatEvents.OnEntityDied -= HandleEntityDeath;
        }
        private List<Entity> CalculateUtilityTargets(PlayableCharacter user)
        {
            TargetType targetType = user.utilityData.targetType;

            if (targetType == TargetType.AllAllies)
            {
                return new List<Entity>(CombatManager.Instance.playerList);
            }

            if (targetType == TargetType.AllEnemies)
            {
                return CalculateTargetsInRange(user, CombatManager.Instance.enemyList);
            }
            
            return new List<Entity>();
        }
        private List<Entity> CalculateTargetsInRange(PlayableCharacter actor, List<Entity> potentialTargets)
        {
            List<Entity> targets = new List<Entity>();
            
            if (!entityPositions.ContainsKey(actor))
                return targets;
            
            int actorPosition = entityPositions[actor];
            int weaponRange = actor.GetRange();
            
            foreach (var target in potentialTargets)
            {
                if (!target.isAlive || !entityPositions.ContainsKey(target))
                    continue;
                
                int targetPosition = entityPositions[target];
                int distance = CalculateDistance(actorPosition, targetPosition);
            
                if (distance <= weaponRange)
                {
                    targets.Add(target);
                }
            }
        
            return targets;
        }
        private void HandleAttackRequest()
        {   
            var currentActor = (PlayableCharacter)CombatManager.Instance.getCurrentActor();
            List<Entity> targets = CalculateTargetsInRange(currentActor,CombatManager.Instance.enemyList);
            CombatEvents.RaiseTargetCalculated(targets);
        }
        
        private void HandleUtilityButtonClicked()
        {
            var currentActor = (PlayableCharacter)CombatManager.Instance.getCurrentActor();
            List<Entity> targets = CalculateUtilityTargets(currentActor);
            CombatEvents.RaiseUtilityTargetCalculated(targets, currentActor.utilityData.targetType);
        }
        private void HandleEntityDeath(Entity deadEntity)
        {
            if (!entityPositions.ContainsKey(deadEntity))
            {
                Debug.LogWarning($"Dead entity {deadEntity.name} not found by PositionManager");
                return;
            }

            if (autoRepositionOnDeath)
            {
                StartCoroutine(DelayedReposition(deadEntity));
            }
        }

        private IEnumerator DelayedReposition(Entity entity)
        {
            yield return new WaitForSeconds(repositionDelay);
            bool isPlayer = CombatManager.Instance.playerList.Contains(entity);
            List<Entity> team = isPlayer ? CombatManager.Instance.playerList : CombatManager.Instance.enemyList;
            List<Transform> positions = isPlayer ? playableCharPositions : enemyPositions;
            
            ReorganizeTeam(team, positions);
        }

        private void ReorganizeTeam(List<Entity> team, List<Transform> positions)
        {
            var livingEntities = team
                .Where(e => e.isAlive && entityPositions.ContainsKey(e))
                .OrderBy(e => entityPositions[e])
                .ToList();
            for (int i = 0; i < livingEntities.Count; i++)
            {
                Entity entity = livingEntities[i];
                int oldPosition = entityPositions[entity];
                int newPosition = i;
                
                entityPositions[entity] = newPosition;
                entity.currentPosition = newPosition;
                
                if (oldPosition != newPosition && newPosition < positions.Count)
                {
                    Vector3 newWorldPosition = positions[newPosition].position;
                    entity.SetTargetPosition(newWorldPosition);
                    Debug.Log($"{entity.entityName} moving: position {oldPosition} → {newPosition}");
                }
                var deadEntities = team.Where(e => !e.isAlive).ToList();
                foreach (var dead in deadEntities)
                {
                    if (entityPositions.ContainsKey(dead))
                    {
                        entityPositions.Remove(dead);
                        Debug.Log($"Removed {dead.entityName} from position tracking");
                    }
                }
            }
        }

        public void RegisterEntityPosition(Entity entity, int positionIndex)
        {
            entityPositions[entity] = positionIndex;
        }
        private int CalculateDistance(int attackerPos, int enemyPos)
        {
            return attackerPos + enemyPos + 1;
        }
    }
}
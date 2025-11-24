using System;
using System.Collections.Generic;
using data;
using entity;
using events;
using JetBrains.Annotations;
using UnityEngine;

namespace combat
{
    public class PositionManager : MonoBehaviour
    {
        [Header("Positions")]
        public List<Transform> playableCharPositions = new List<Transform>(4);
        public List<Transform> enemyPositions = new List<Transform>(4);
        
        private Dictionary<Entity, int> entityPositions = new Dictionary<Entity, int>();
        void OnEnable()
        {
            CombatEvents.OnAttackButtonClicked += HandleAttackRequest;
            CombatEvents.OnUtilityButtonClicked += HandleUtilityButtonClicked;
        }
        void OnDisable()
        {
            CombatEvents.OnAttackButtonClicked -= HandleAttackRequest;
            CombatEvents.OnUtilityButtonClicked -= HandleUtilityButtonClicked;
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
            int weaponRange = actor.GetWeaponRange();
            
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
        public void RegisterEntityPosition(Entity entity, int positionIndex)
        {
            entityPositions[entity] = positionIndex;
        }
        private int CalculateDistance(int attackerPos, int enemyPos)
        {
            // Attacker in position 0 (front) to enemy in position 0 (front) = distance 1
            // Attacker in position 0 to enemy in position 1 = distance 2
            // Attacker in position 1 to enemy in position 0 = distance 2 (1 back + 1 across)
        
            return attackerPos + enemyPos + 1;
        }
    }
}
using System;
using System.Collections.Generic;
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
        }

        void OnDisable()
        {
            CombatEvents.OnAttackButtonClicked -= HandleAttackRequest;
        }
        
        private List<Entity> CalculateAvailableTargets(PlayableCharacter attacker)
        {
            List<Entity> targets = new List<Entity>();
            int attackerPosition = entityPositions[attacker];
            int weaponRange = attacker.GetWeaponRange();
            
            foreach (var enemy in CombatManager.Instance.enemyList)
            {
                if (!enemy.isAlive || !entityPositions.ContainsKey(enemy))
                    continue;
                
                int enemyPosition = entityPositions[enemy];
                int distance = CalculateDistance(attackerPosition, enemyPosition);
            
                if (distance <= weaponRange)
                {
                    targets.Add(enemy);
                }
            }
        
            return targets;
        }
        private void HandleAttackRequest()
        {   
            var currentActor = CombatManager.Instance.getCurrentActor();
            List<Entity> targets = CalculateAvailableTargets((PlayableCharacter)currentActor);
            CombatEvents.RaiseTargetCalculated(targets);
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
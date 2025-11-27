using System;
using System.Collections.Generic;
using data;
using entity;
using UnityEngine;

namespace events
{
    public static class CombatEvents
    {
        public static event Action OnAttackButtonClicked;
        public static event Action OnShieldButtonClicked;
        public static event Action OnUtilityButtonClicked;
        public static event Action OnCancelButtonClicked;
        public static event Action<List<Entity>> OnTargetCalculated;
        public static event Action<List<Entity>, TargetType> OnUtilityTargetCalculated;
        public static event Action<Entity> OnTargetSelected;
        public static event Action<Entity> OnUtilityTargetSelected;
        public static event Action<Entity> OnPlayerTurnStarted;
        
        //For now this event is made for the indicator of whose turn is it
        public static event Action<Entity> OnCurrentActorPicked;
        public static event Action OnPlayerTurnEnded;
        public static event Action<Entity, int> OnDamageTaken;
        public static event Action<Entity> OnEntityDied;
        public static event Action<GameObject> OnEntityDeathAnimation;

        public static void RaiseTargetCalculated(List<Entity> targets)
        {
            OnTargetCalculated?.Invoke(targets);
        }
        public static void RaiseUtilityTargetCalculated(List<Entity> targets, TargetType type)
        {
            OnUtilityTargetCalculated?.Invoke(targets, type);
        }
        public static void RaiseTargetSelected(Entity entity)
        {
            OnTargetSelected?.Invoke(entity);
        }
        public static void RaiseUtilityTargetSelected(Entity entity)
        {
            OnUtilityTargetSelected?.Invoke(entity);
        }
        public static void RaiseAttackButtonClicked()
        {
            OnAttackButtonClicked?.Invoke();
        }
        
        public static void RaisePlayerTurnStarted(Entity entity)
        {
            OnPlayerTurnStarted?.Invoke(entity);
        }

        public static void RaiseCurrentActorPicked(Entity entity)
        {
            OnCurrentActorPicked?.Invoke(entity);
        }
        public static void RaisePlayerTurnEnded()
        {
            OnPlayerTurnEnded?.Invoke();
        }
        
        public static void RaiseDamageTaken(Entity entity, int damage)
        {
            OnDamageTaken?.Invoke(entity, damage);
        }
        
        public static void RaiseEntityDied(Entity entity)
        {
            OnEntityDied?.Invoke(entity);
        }
        
        public static void RaiseEntityDeathAnimation(GameObject deadEntity)
        {
            OnEntityDeathAnimation?.Invoke(deadEntity);
        }
        public static void RaiseCancelButtonClicked()
        {
            OnCancelButtonClicked?.Invoke();
        }

        public static void RaiseShieldButtonClicked()
        {
            OnShieldButtonClicked?.Invoke();
        }

        public static void RaiseUtilityButtonClicked()
        {
            OnUtilityButtonClicked?.Invoke();
        }
        public static void ClearAllEvents()
        {
            OnAttackButtonClicked = null;
            OnPlayerTurnStarted = null;
            OnPlayerTurnEnded = null;
            OnDamageTaken = null;
            OnEntityDied = null;
            OnTargetCalculated = null;
            OnTargetSelected = null;
            OnCancelButtonClicked = null;
            OnShieldButtonClicked = null;
            OnUtilityButtonClicked = null;
        }
    }
}
using System;
using System.Collections.Generic;
using entity;

namespace events
{
    public static class CombatEvents
    {
        public static event Action OnAttackButtonClicked;
        public static event Action OnCancelButtonClicked;
        public static event Action<List<Entity>> OnTargetCalculated;
        
        public static event Action<Entity> OnTargetSelected;
        public static event Action<Entity> OnPlayerTurnStarted;
        
        public static event Action OnPlayerTurnEnded;
        public static event Action<Entity, int> OnDamageTaken;
        public static event Action<Entity> OnEntityDied;

        public static void RaiseTargetCalculated(List<Entity> targets)
        {
            OnTargetCalculated?.Invoke(targets);
        }

        public static void RaiseTargetSelected(Entity entity)
        {
            OnTargetSelected?.Invoke(entity);
        }
        public static void RaiseAttackButtonClicked()
        {
            OnAttackButtonClicked?.Invoke();
        }
        
        public static void RaiseTurnStarted(Entity entity)
        {
            OnPlayerTurnStarted?.Invoke(entity);
        }
        
        public static void RaiseTurnEnded()
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

        public static void RaiseCancelButtonClicked()
        {
            OnCancelButtonClicked?.Invoke();
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
        }
    }
}
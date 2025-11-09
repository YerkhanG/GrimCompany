using System;
using entity;

namespace events
{
    public static class CombatEvents
    {
        public static event Action<Entity> OnAttackButtonClicked;
        
        public static event Action<Entity> OnPlayerTurnStarted;
        
        public static event Action OnPlayerTurnEnded;
        public static event Action<Entity, int> OnDamageTaken;
        public static event Action<Entity> OnEntityDied;
        
        public static void RaiseAttackButtonClicked(Entity target)
        {
            OnAttackButtonClicked?.Invoke(target);
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
        public static void ClearAllEvents()
        {
            OnAttackButtonClicked = null;
            OnPlayerTurnStarted = null;
            OnPlayerTurnEnded = null;
            OnDamageTaken = null;
            OnEntityDied = null;

        }
    }
}
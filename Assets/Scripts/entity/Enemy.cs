using System;
using System.Linq;
using combat;
using UnityEngine;

namespace entity
{
    public class Enemy : Entity
    {
        public override void StartTurn()
        {
            base.StartTurn();
    
            if (IsStunned) 
                return;
        
            Invoke(nameof(PerformAiAction), 0.5f);
        }

        public void PerformAiAction()
        {
            if (CombatManager.Instance.getCurrentActor() != this)
                return;
        
            Entity target = FindRandomTarget();
            if (target != null)
            {
                Attack(target);
            }
            CombatManager.Instance.EndCurrentTurn();
        }

        private Entity FindRandomTarget()
        {
            var alivePlayers = CombatManager.Instance.playerList.Where(p => p.isAlive).ToList();
            
            if (alivePlayers.Count == 0)
                return null;
            
            int randomIndex = UnityEngine.Random.Range(0, alivePlayers.Count);
            return alivePlayers[randomIndex];
        }
    }
}
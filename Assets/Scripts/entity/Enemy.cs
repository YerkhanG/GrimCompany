using System;
using System.Linq;
using combat;
using UnityEngine;
using animation;

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
                // Play attack animation, then end turn when complete
                if (entityAnimator != null)
                {
                    AnimationController.Instance.PlayAnimation(
                        () => entityAnimator.PlayAttackAnimation(target, null),
                        () => CombatManager.Instance.EndCurrentTurn()
                    );
                }
                else
                {
                    // Fallback without animation
                    Attack(target);
                    CombatManager.Instance.EndCurrentTurn();
                }
            }
            else
            {
                CombatManager.Instance.EndCurrentTurn();
            }
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
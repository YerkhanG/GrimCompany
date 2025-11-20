using System;
using combat;

namespace entity
{
    public class Enemy : Entity
    {
        public override void StartTurn()
        {
            base.StartTurn();
            Invoke(nameof(PerformAiAction), 0.5f);
        }

        public void PerformAiAction()
        {
            Entity target = FindRandomTarget();
            if (target != null)
            {
                Attack(target);
            }
            CombatManager.Instance.EndCurrentTurn();
        }

        private Entity FindRandomTarget()
        {
            int randomIndex = new Random().Next(0, CombatManager.Instance.playerList.Count);
            return CombatManager.Instance.playerList[randomIndex];
        }
    }
}
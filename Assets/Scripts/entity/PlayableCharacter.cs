using controller.combat;
using events;
using Unity.VisualScripting;
using UnityEngine;

namespace entity
{
    public class PlayableCharacter : Entity
    {
        void OnEnable()
        {
            CombatEvents.OnAttackButtonClicked += HandleAttackRequest;
        }

        void OnDisable()
        {
            CombatEvents.OnAttackButtonClicked -= HandleAttackRequest;
        }

        private void HandleAttackRequest(Entity target)
        {
            if (CombatManager.Instance.getCurrentActor() != this)
                return;
            
            Attack(target);
            CombatEvents.RaiseTurnEnded();
            CombatManager.Instance.EndCurrentTurn();
        }

        public override void StartTurn()
        {
            base.StartTurn();
            CombatEvents.RaiseTurnStarted(this);
        }
    }
}
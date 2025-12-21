using System;
using combat;
using data;
using events;
using UnityEngine;
using animation;

namespace entity
{
    public class PlayableCharacter : Entity
    {
        [Header("Utility data")]
        public UtilityData utilityData;
        
        [Header("Buff/Debuff State")]
        private int damageBonus = 0;
        private int defenseBonus = 0;
        private int buffTurnsRemaining = 0;
        
        [Header("Shield State")]
        private bool isShielding = false;
        private int shieldTurnsRemaining = 0;
        
        public int GetRange()
        {
            return entityData.range;
        }
        
        private void HandleTargetSelected(Entity target)
        {
            if (CombatManager.Instance.getCurrentActor() != this)
                return;
         
            Debug.Log("Target selected");
            // Play attack animation, then end turn when complete
            if (entityAnimator != null)
            {
                Debug.Log("Animating playable character");
                AnimationController.Instance.PlayAnimation(
                    () => entityAnimator.PlayAttackAnimation(target, null),
                    () => {
                        CombatEvents.RaisePlayerTurnEnded();
                        CombatManager.Instance.EndCurrentTurn();
                    }
                );
            }
            else
            {
                Debug.Log("Not animating playable character");
                // Fallback without animation
                Attack(target);
                CombatEvents.RaisePlayerTurnEnded();
                CombatManager.Instance.EndCurrentTurn();
            }
        }
        
        private void HandleUtilityTargetSelected(Entity target)
        {
            if (CombatManager.Instance.getCurrentActor() != this)
                return;
            
            // Play utility animation, execute effect, then end turn
            if (entityAnimator != null)
            {
                AnimationController.Instance.PlayAnimation(
                    () => {
                        entityAnimator.PlayUtilityAnimation(target, null);
                        // Execute utility effect at animation event frame
                        utilityData.Execute(this, target);
                    },
                    () => {
                        CombatEvents.RaisePlayerTurnEnded();
                        CombatManager.Instance.EndCurrentTurn();
                    }
                );
            }
            else
            {
                // Fallback without animation
                utilityData.Execute(this, target);
                CombatEvents.RaisePlayerTurnEnded();
                CombatManager.Instance.EndCurrentTurn();
            }
        }
        
        public override void StartTurn()
        {
            InitSubscriptions();
            base.StartTurn();
            
            // Play attack ready animation when turn starts
            if (entityAnimator != null)
                entityAnimator.PlayAttackReadyAnimation();
            
            CombatEvents.RaisePlayerTurnStarted(this);
        }
        
        private void HandleShieldClicked()
        {
            if (CombatManager.Instance.getCurrentActor() != this)
                return;
            
            ActivateShield();
            CombatEvents.RaisePlayerTurnEnded();
            CombatManager.Instance.EndCurrentTurn();
        }

        private void ActivateShield()
        {
            if (CombatManager.Instance.getCurrentActor() != this)
                return;
            
            isShielding = true;
            shieldTurnsRemaining = entityData.shieldDuration;
            Debug.Log($"{entityName} shields up! {entityData.shieldDamageReduction * 100}% reduction for {entityData.shieldDuration} turns");
        }

        public override void TakeDamage(int damage)
        {
            if (isShielding)
            {
                damage = Mathf.RoundToInt(damage * (1f - entityData.shieldDamageReduction));
                Debug.Log($"Shield blocked damage! Taking {damage} instead");
            }
            damage = Mathf.Max(0, damage - defenseBonus);
            
            base.TakeDamage(damage);
        }

        public override void Attack(Entity target)
        {
            target.TakeDamage(BaseDamage + damageBonus);
        }

        public void ApplyBuff(float damageBoost, int defenseBoost, int duration)
        {
            damageBonus += (int)(BaseDamage * damageBoost);  
            defenseBonus += defenseBoost;                     
            buffTurnsRemaining = Mathf.Max(buffTurnsRemaining, duration);
            Debug.Log($"{entityName} buffed! +{damageBoost} damage, +{defenseBoost} defense for {duration} turns");
        }
        
        public override void EndTurn()
        {
            InitUnsubscriptions();
            
            // Return to idle animation
            if (entityAnimator != null)
                entityAnimator.PlayIdleAnimation();
            
            base.EndTurn();
            
            if (buffTurnsRemaining > 0)
            {
                buffTurnsRemaining--;
                if (buffTurnsRemaining == 0)
                {
                    damageBonus = 0;
                    defenseBonus = 0;
                    Debug.Log($"{entityName}'s buff wore off");
                }
            }

            if (shieldTurnsRemaining > 0)
            {
                shieldTurnsRemaining--;
                if (shieldTurnsRemaining == 0)
                {
                    isShielding = false;
                    Debug.Log($"{entityName}'s shield wore off");
                }
            }
        }

        private void InitSubscriptions()
        {
            CombatEvents.OnTargetSelected += HandleTargetSelected;
            CombatEvents.OnUtilityTargetSelected += HandleUtilityTargetSelected;
            CombatEvents.OnShieldButtonClicked += HandleShieldClicked;
        }
        
        private void InitUnsubscriptions()
        {
            CombatEvents.OnTargetSelected -= HandleTargetSelected;
            CombatEvents.OnUtilityTargetSelected -= HandleUtilityTargetSelected;
            CombatEvents.OnShieldButtonClicked -= HandleShieldClicked;        
        }
    }
}
using combat;
using data;
using events;
using UnityEngine;

namespace entity
{
    public class PlayableCharacter : Entity
    {
        [Header("Weapon data")] 
        [SerializeField]private WeaponData weaponData;
        
        
        void OnEnable()
        {
            CombatEvents.OnTargetSelected += HandleTargetSelected;
        }

        void OnDisable()
        {
            CombatEvents.OnTargetSelected -= HandleTargetSelected;
        }
        public int GetWeaponRange()
        {
            return weaponData.range;
        }
        private void HandleTargetSelected(Entity target)
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
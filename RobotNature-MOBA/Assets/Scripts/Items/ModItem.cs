using Entities;
using UnityEngine;

namespace Items
{
    public class ModItem : Item
    {
        private IActiveLifeable lifeable; 
        private IMovable moveable;
        private IAttackable attackable;
        // TODO:
        // - Add a way to resistance
        // - Add a way to reduce cooldown

        protected override void OnItemAddedEffects(Entity entity)
        {
            lifeable = entity.GetComponent<IActiveLifeable>();
            lifeable?.RequestIncreaseMaxHp(((ModItemSO)AssociatedItemSO()).healthMod);
            
            moveable = entity.GetComponent<IMovable>();
            moveable?.RequestIncreaseCurrentMoveSpeed(((ModItemSO)AssociatedItemSO()).speedMod);
            
            attackable = entity.GetComponent<IAttackable>();
            attackable?.RequestIncreaseAttackSpeed(((ModItemSO)AssociatedItemSO()).attackSpeedMod);
            attackable?.RequestIncreaseAttackDamage(((ModItemSO)AssociatedItemSO()).damageMod);
        }

        protected override void OnItemAddedEffectsFeedback(Entity entity)
        {
            Debug.Log($"Gained {((ModItemSO)AssociatedItemSO()).healthMod} hp");
            Debug.Log($"Gained {((ModItemSO)AssociatedItemSO()).speedMod} speed");
            Debug.Log($"Gained {((ModItemSO)AssociatedItemSO()).attackSpeedMod} attack speed");
            Debug.Log($"Gained {((ModItemSO)AssociatedItemSO()).damageMod} damage");
        }

        protected override void OnItemRemovedEffects(Entity entity)
        {
            lifeable?.RequestDecreaseMaxHp(((ModItemSO)AssociatedItemSO()).healthMod);
            moveable?.RequestDecreaseCurrentMoveSpeed(((ModItemSO)AssociatedItemSO()).speedMod);
            attackable?.RequestDecreaseAttackSpeed(((ModItemSO)AssociatedItemSO()).attackSpeedMod);
            attackable?.RequestDecreaseAttackDamage(((ModItemSO)AssociatedItemSO()).damageMod);
        }

        protected override void OnItemRemovedEffectsFeedback(Entity entity)
        {
            Debug.Log($"Removed {((ModItemSO)AssociatedItemSO()).healthMod} hp");
            Debug.Log($"Removed {((ModItemSO)AssociatedItemSO()).speedMod} speed");
            Debug.Log($"Removed {((ModItemSO)AssociatedItemSO()).attackSpeedMod} attack speed");
            Debug.Log($"Removed {((ModItemSO)AssociatedItemSO()).damageMod} damage");
        }
    }
}
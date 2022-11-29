using Entities.Inventory;
using UnityEngine;

namespace Items
{
    public class ModItem : Item
    {
        // private IActiveLifeable lifeable;
        // private IMoveable moveable; 
        // TODO:
        // - Add a way to increase damage
        // - Add a way to increase attack speed
        // - Add a way to resistance
        // - Add a way to reduce cooldown

        protected override void OnItemAddedEffects(Entity entity)
        {
            // lifeable = entity.GetComponent<IActiveLifeable>();
            // lifeable?.IncreaseMaxHpRPC(((ModItemSO)AssociatedItemSO()).healthMod);
            // moveable = entity.GetComponent<IMoveable>();
            // moveable?.IncreaseSpeedRPC(((ModItemSO)AssociatedItemSO()).speedMod);
        }

        protected override void OnItemAddedEffectsFeedback(Entity entity)
        {
            Debug.Log($"Gained {((ModItemSO)AssociatedItemSO()).healthMod} hp");
        }

        protected override void OnItemRemovedEffects(Entity entity)
        {
            // lifeable?.DecreaseMaxHpRPC(((ModItemSO)AssociatedItemSO()).healthMod);
        }

        protected override void OnItemRemovedEffectsFeedback(Entity entity)
        {
            Debug.Log($"Removed {((ModItemSO)AssociatedItemSO()).healthMod} hp");
        }
    }
}
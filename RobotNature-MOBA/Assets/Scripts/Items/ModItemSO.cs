using System;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(menuName = "ItemSO/ModItem", fileName = "new ModItemSO")]
    public class ModItemSO : ItemSO
    {
        public int healthMod;
        public int powerMod;
        public int resistanceMod;
        public int speedMod;
        public int attackSpeedMod;
        public int cooldownMod;
        
        public override Type AssociatedType()
        {
            return typeof(ModItem);
        }
    }
}


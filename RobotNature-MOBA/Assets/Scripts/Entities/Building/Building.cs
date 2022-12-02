using UnityEngine;

namespace Entities.Building
{
    public class Building : Entity
    {
        [Space]
        [Header("Life Building settings")]
        public bool isAlive;
        public float maxHealth;
        public float currentHealth;

        protected override void OnStart()
        {
            base.OnStart();
            currentHealth = maxHealth;
        }
    }
}
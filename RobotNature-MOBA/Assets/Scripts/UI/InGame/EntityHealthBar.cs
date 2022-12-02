using UnityEngine;
using UnityEngine.UI;

namespace UI.InGame
{
    public class EntityHealthBar : MonoBehaviour
    {
        [SerializeField] private Image healthBar;
        private IActiveLifeable lifeable;
        
        public void InitHealthBar(Entity entity)
        {
            lifeable = (IActiveLifeable)entity;

            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
            healthBar.fillAmount = lifeable.GetCurrentHp() / lifeable.GetMaxHp();
            lifeable.OnSetCurrentHpFeedback += UpdateFillPercent;
            lifeable.OnIncreaseCurrentHpFeedback += UpdateFillPercent;
            lifeable.OnDecreaseCurrentHpFeedback += UpdateFillPercent;
            lifeable.OnIncreaseMaxHpFeedback += UpdateFillPercent;
            lifeable.OnDecreaseMaxHpFeedback += UpdateFillPercent;
        }

        private void UpdateFillPercent(float value)
        {
            healthBar.fillAmount = value / lifeable.GetMaxHp();
        }
    }
}
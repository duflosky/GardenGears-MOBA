using Entities;
using UnityEngine;
using UnityEngine.UI;

namespace UI.InGame
{
    public class EntityHealthBar : MonoBehaviour
    {
        [SerializeField] private Image healthBar;
        private IActiveLifeable lifeable;
        private IDeadable deadable;
        
        public void InitHealthBar(Entity entity)
        {
            lifeable = (IActiveLifeable)entity;
            deadable = (IDeadable)entity;

            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
            healthBar.fillAmount = lifeable.GetCurrentHp() / lifeable.GetMaxHp();
            lifeable.OnSetCurrentHpFeedback += UpdateFillPercent;
            lifeable.OnIncreaseCurrentHpFeedback += UpdateFillPercent;
            lifeable.OnDecreaseCurrentHpFeedback += UpdateFillPercent;
            lifeable.OnIncreaseMaxHpFeedback += UpdateFillPercent;
            lifeable.OnDecreaseMaxHpFeedback += UpdateFillPercent;
            
            deadable.OnDieFeedback += ActivateHealthBar;
            deadable.OnReviveFeedback += ActivateHealthBar;
        }

        private void UpdateFillPercent(float value)
        {
            healthBar.fillAmount = value / lifeable.GetMaxHp();
        }

        private void ActivateHealthBar()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}
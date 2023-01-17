using Entities;
using Entities.Minion;
using GameStates;
using UnityEngine;
using UnityEngine.UI;

namespace UI.InGame
{
    public class EntityHealthBar : MonoBehaviour
    {
        [SerializeField] private Image healthBar;
        private IActiveLifeable liveable;
        private IDeadable deadable;

        public void InitHealthBar(Entity entity)
        {
            liveable = (IActiveLifeable)entity;
            deadable = (IDeadable)entity;

            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
            if (GameStateMachine.Instance.GetPlayerTeam() != entity.team) healthBar.color = Color.red;
            healthBar.fillAmount = liveable.GetCurrentHp() / liveable.GetMaxHp();
            liveable.OnSetCurrentHpFeedback += UpdateFillPercent;
            liveable.OnIncreaseCurrentHpFeedback += UpdateFillPercent;
            liveable.OnDecreaseCurrentHpFeedback += UpdateFillPercent;
            liveable.OnDecreaseCurrentHpCapacityFeedback += UpdateFillPercent;
            liveable.OnIncreaseMaxHpFeedback += UpdateFillPercent;
            liveable.OnDecreaseMaxHpFeedback += UpdateFillPercent;

            deadable.OnDieFeedback += ActivateHealthBar;
            deadable.OnReviveFeedback += ActivateHealthBar;
        }

        private void UpdateFillPercent(float value)
        {
            healthBar.fillAmount = value / liveable.GetMaxHp();
        }
        
        private void UpdateFillPercent(float value, byte capacityIndex)
        {
            healthBar.fillAmount = value / liveable.GetMaxHp();
        }

        private void ActivateHealthBar()
        {
            if (deadable != null)
            {
                gameObject.SetActive(deadable.IsAlive());
                if (deadable.IsAlive() && deadable as Minion)
                {
                    var minion = (Minion)deadable;
                    gameObject.SetActive(GameStateMachine.Instance.GetPlayerTeam() == minion.team);
                }
            }
        }
    }
}
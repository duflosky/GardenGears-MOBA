using Entities;
using UnityEngine;
using UnityEngine.UI;

namespace UI.InGame
{
    public class EntityResourceBar : MonoBehaviour
    {
        [SerializeField] private Image resourceBar;
        private IResourceable resourceable;
        private IDeadable deadable;

        public void InitResourceBar(Entity entity)
        {
            resourceable = (IResourceable)entity;
            deadable = (IDeadable)entity;

            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
            resourceBar.fillAmount = resourceable.GetCurrentResourcePercent();

            resourceable.OnSetCurrentResourceFeedback += UpdateFillPercent;
            resourceable.OnIncreaseCurrentResourceFeedback += UpdateFillPercent;
            resourceable.OnDecreaseCurrentResourceFeedback += UpdateFillPercent;
            resourceable.OnIncreaseMaxResourceFeedback += UpdateFillPercent;
            resourceable.OnDecreaseMaxResourceFeedback += UpdateFillPercent;

            deadable.OnDieFeedback += ActivateResourceBar;
            deadable.OnReviveFeedback += ActivateResourceBar;
        }

        private void UpdateFillPercent(float value)
        {
            resourceBar.fillAmount = resourceable.GetCurrentResource() / resourceable.GetMaxResource();
        }

        private void ActivateResourceBar()
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}
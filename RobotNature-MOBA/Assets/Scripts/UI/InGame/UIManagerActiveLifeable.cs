using Entities;
using GameStates;
using UnityEngine;

namespace UI.InGame
{
    public partial class UIManager
    {
        [Header("HealthBar Elements")]
        [SerializeField] private GameObject healthBarPrefab;

        public void InstantiateHealthBarForEntity(int entityIndex)
        {
            var entity = EntityCollectionManager.GetEntityByIndex(entityIndex);
            if (entity == null) return;
            if (entity.GetComponent<IActiveLifeable>() == null) return;
            var canvasHealth = Instantiate(healthBarPrefab, entity.TransformUI.position + entity.OffsetUI, Quaternion.identity, entity.TransformUI);
            entity.elementsToShow.Add(canvasHealth);
            if (entity.team != GameStateMachine.Instance.GetPlayerTeam()) canvasHealth.SetActive(false);
            canvasHealth.GetComponent<EntityHealthBar>().InitHealthBar(entity);
        }
    }
}

using Entities;
using GameStates;
using UnityEngine;
using UnityEngine.Animations;

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
            var canvasHealth = Instantiate(healthBarPrefab, entity.TransformUI.position + entity.OffsetUI, Quaternion.identity);
            constraintSource.sourceTransform = entity.transform;
            constraintSource.weight = 1;
            canvasHealth.GetComponent<PositionConstraint>().AddSource(constraintSource);
            canvasHealth.GetComponent<PositionConstraint>().translationOffset += entity.OffsetUI;
            canvasHealth.GetComponent<PositionConstraint>().constraintActive = true;
            entity.elementsToShow.Add(canvasHealth);
            if (entity.team != GameStateMachine.Instance.GetPlayerTeam()) canvasHealth.SetActive(false);
            canvasHealth.GetComponent<EntityHealthBar>().InitHealthBar(entity);
        }
    }
}

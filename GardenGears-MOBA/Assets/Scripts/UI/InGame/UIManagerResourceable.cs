using Entities;
using GameStates;
using UnityEngine;
using UnityEngine.Animations;

namespace UI.InGame
{
    public partial class UIManager
    {
        [Header("ResourceBar Elements")] [SerializeField]
        private GameObject resourceBarPrefab;

        public void InstantiateResourceBarForEntity(int entityIndex)
        {
            var entity = EntityCollectionManager.GetEntityByIndex(entityIndex);
            if (entity == null) return;
            if (entity.GetComponent<IResourceable>() == null) return;
            var canvasResource = Instantiate(resourceBarPrefab, entity.TransformUI.position + entity.OffsetUI, Quaternion.identity, entity.TransformUI);
            constraintSource.sourceTransform = entity.transform;
            constraintSource.weight = 1;
            canvasResource.GetComponent<PositionConstraint>().AddSource(constraintSource);
            canvasResource.GetComponent<PositionConstraint>().translationOffset += entity.OffsetUI;
            canvasResource.GetComponent<PositionConstraint>().constraintActive = true;
            entity.elementsToShow.Add(canvasResource);
            entity.neverHideElements.Add(canvasResource);
            // if (entity.team != GameStateMachine.Instance.GetPlayerTeam()) canvasResource.SetActive(false);
            canvasResource.GetComponent<EntityResourceBar>().InitResourceBar(entity);
        }
    }
}
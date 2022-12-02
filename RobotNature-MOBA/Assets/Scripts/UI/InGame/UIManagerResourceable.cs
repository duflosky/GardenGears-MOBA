using Entities;
using GameStates;
using UnityEngine;

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
            entity.elementsToShow.Add(canvasResource);
            canvasResource.GetComponent<EntityResourceBar>().InitResourceBar(entity);
            if (entity.team != GameStateMachine.Instance.GetPlayerTeam())
            {
                canvasResource.SetActive(false);
            }
        }
    }
}
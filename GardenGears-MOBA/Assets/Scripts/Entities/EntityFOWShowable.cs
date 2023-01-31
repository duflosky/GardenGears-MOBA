using System.Collections.Generic;
using Entities.FogOfWar;
using GameStates;
using Photon.Pun;
using UnityEngine;

namespace  Entities
{
    public abstract partial class Entity : IFOWShowable
    {
        public List<IFOWViewable> enemiesThatCanSeeMe = new();
        public bool canShow;
        public bool canHide;
        public List<GameObject> elementsToShow = new();
        public List<GameObject> neverHideElements = new();

        
        public bool CanShow() 
        {
            return canShow;
        }

        public bool CanHide()
        {
            return canHide;
        }

        public void RequestSetCanShow(bool value)
        {
            photonView.RPC("SetCanShowRPC",RpcTarget.MasterClient,value);
        }

        public void SyncSetCanShowRPC(bool value)
        {
            canShow = value;
            OnSetCanShowFeedback?.Invoke(value);
        }

        [PunRPC]
        public void SetCanShowRPC(bool value)
        {
            canShow = value;
            OnSetCanShow?.Invoke(value);
            photonView.RPC("SyncSetCanShowRPC",RpcTarget.All,value);
        }

        public event GlobalDelegates.BoolDelegate OnSetCanShow;
        public event GlobalDelegates.BoolDelegate OnSetCanShowFeedback;
        
        public void RequestSetCanHide(bool value)
        {
            photonView.RPC("SetCanHideRPC",RpcTarget.MasterClient,value);
        }

        [PunRPC]
        public void SyncSetCanHideRPC(bool value)
        {
            canHide = value;
            OnSetCanHideFeedback?.Invoke(value);
        }

        [PunRPC]
        public void SetCanHideRPC(bool value)
        {
            canHide = value;
            OnSetCanHide?.Invoke(value);
            photonView.RPC("SyncSetCanHideRPC",RpcTarget.All,value);
        }

        public event GlobalDelegates.BoolDelegate OnSetCanHide;
        public event GlobalDelegates.BoolDelegate OnSetCanHideFeedback;
        
        public void TryAddFOWViewable(int viewableIndex)
        {
            var entity = EntityCollectionManager.GetEntityByIndex(viewableIndex);
            if(entity == null) return;

            if (entity != null) TryAddFOWViewable(entity);
        }

        public void TryAddFOWViewable(Entity viewable)
        {
            if (!GetEnemyTeams().Contains(viewable.GetTeam()) || enemiesThatCanSeeMe.Contains(viewable)) return;

            var show = enemiesThatCanSeeMe.Count == 0;
            
            enemiesThatCanSeeMe.Add(viewable);
            if (show) ShowElements();
        }
        
        [PunRPC]
        public void SyncTryAddViewableRPC(int viewableIndex,bool show)
        {
            var viewable = EntityCollectionManager.GetEntityByIndex(viewableIndex).GetComponent<IFOWViewable>();
            if (viewable == null) return;
            if (enemiesThatCanSeeMe.Contains(viewable)) return;
            
            enemiesThatCanSeeMe.Add(viewable);
            if (show) ShowElements();
        }

        public void ShowElements()
        {
            for (int i = 0; i < elementsToShow.Count; i++)
            {
                elementsToShow[i].SetActive(true);
            }
            OnShowElementFeedback?.Invoke();
        }

        public event GlobalDelegates.NoParameterDelegate OnShowElement;
        public event GlobalDelegates.NoParameterDelegate OnShowElementFeedback;
        
        public void TryRemoveFOWViewable(int viewableIndex)
        {
            var entity = EntityCollectionManager.GetEntityByIndex(viewableIndex);
            if(entity == null) return;
            
            var viewable = entity.GetComponent<IFOWViewable>();
            if(viewable == null) return;
            
            TryRemoveFOWViewable(viewable);
        }

        public void TryRemoveFOWViewable(IFOWViewable viewable)
        {
            if (!enemiesThatCanSeeMe.Contains(viewable)) return;

            var hide = enemiesThatCanSeeMe.Count == 1;
            
            enemiesThatCanSeeMe.Remove(viewable);
            if (hide) HideElements();
        }

        [PunRPC]
        public void SyncTryRemoveViewableRPC(int viewableIndex,bool hide)
        {
            var viewable = EntityCollectionManager.GetEntityByIndex(viewableIndex).GetComponent<IFOWViewable>();
            if (viewable == null) return;
            if (!enemiesThatCanSeeMe.Contains(viewable)) return;
            enemiesThatCanSeeMe.Remove(viewable);
            if(hide) HideElements();
        }

        public void HideElements()
        {
            for (int i = 0; i < elementsToShow.Count; i++)
            {
                var element = elementsToShow[i];
                if (element != null)
                {
                    if(GameStateMachine.Instance.GetPlayerTeam() != team) element.SetActive(false);
                    else
                    {
                        if (!neverHideElements.Contains(element))
                        {
                            element.SetActive(false);
                        }
                    }
                }
            }
            OnHideElementFeedback?.Invoke();
        }

        public event GlobalDelegates.NoParameterDelegate OnHideElement;
        public event GlobalDelegates.NoParameterDelegate OnHideElementFeedback;
    }
}
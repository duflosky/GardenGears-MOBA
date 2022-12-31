using Entities;
using Entities.FogOfWar;
using GameStates;
using Photon.Pun;
using UnityEngine;

public class AnimationCallbacks : MonoBehaviourPun
{
    [SerializeField] private Transform castTransform;
    public ICastable caster;
    
    public void AnimationCast()
    {
        if(photonView.IsMine)photonView.RPC("AnimationCastRPC", RpcTarget.MasterClient);
    }

    [PunRPC]
    void AnimationCastRPC()
    {
        caster.CastAnimationCast(castTransform);
    }

    public void AnimationEnd()
    {
        if(photonView.IsMine)photonView.RPC("AnimationEndRPC", RpcTarget.MasterClient);
    }

    [PunRPC]
    public void AnimationEndRPC()
    {
        caster.CastAnimationEnd();
    }
    
    public void OnDieEnd()
    {
        Champion champion = GetComponentInParent<Champion>();
        if (champion.isAlive) return;
        photonView.RPC("OnDieEndRPC", RpcTarget.All, champion.entityIndex);
    }

    [PunRPC]
    public void OnDieEndRPC(int indexChampion)
    {
        Champion champion = EntityCollectionManager.GetEntityByIndex(indexChampion) as Champion;
        champion.rotateParent.gameObject.SetActive(false);
        champion.TransformUI.gameObject.SetActive(false);
        FogOfWarManager.Instance.RemoveFOWViewable(champion);
        GameStateMachine.Instance.OnTick += champion.Revive;
    }
}
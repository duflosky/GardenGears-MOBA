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
    
}

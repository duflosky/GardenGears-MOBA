using Entities;
using Entities.FogOfWar;
using Entities.Minion;
using GameStates;
using Photon.Pun;
using UnityEngine;

public class AnimationCallbacks : MonoBehaviourPun
{
    [SerializeField] private Transform castTransform;
    public ICastable caster;


    public void AnimationShotEffect()
    {
        caster.CastAnimationShotEffect(castTransform);
    }
    
    public void AnimationCast()
    {
        caster.CastAnimationCast(castTransform);
    }

    public void AnimationEnd()
    {
      caster.CastAnimationEnd();
    }
    
    public void AnimationFeedback()
    {
        caster.CastAnimationFeedback();
    }
    
    public void OnChampionDieEnd()
    {
        Champion champion = GetComponentInParent<Champion>();
        if (champion.isAlive) return;
        photonView.RPC("OnChampionDieEndRPC", RpcTarget.All, champion.entityIndex);
    }

    [PunRPC]
    public void OnChampionDieEndRPC(int indexChampion)
    {
        Champion champion = EntityCollectionManager.GetEntityByIndex(indexChampion) as Champion;
        champion.rotateParent.gameObject.SetActive(false);
        FogOfWarManager.Instance.RemoveFOWViewable(champion);
        GameStateMachine.Instance.OnTick += champion.Revive;
    }
    
    public void OnMinionDieEnd()
    {
        Minion minion = GetComponent<Minion>();
        if (minion.isAlive) return;
        photonView.RPC("OnMinionDieEndRPC", RpcTarget.All, minion.entityIndex);
    }
    
    [PunRPC]
    public void OnMinionDieEndRPC(int indexMinion)
    {
        Minion minion = EntityCollectionManager.GetEntityByIndex(indexMinion) as Minion;
        FogOfWarManager.Instance.RemoveFOWViewable(minion);
        gameObject.SetActive(false);
    }
}
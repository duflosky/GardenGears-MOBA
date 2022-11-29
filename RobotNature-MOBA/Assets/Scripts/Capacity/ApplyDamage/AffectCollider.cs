using System.Collections;
using System.Collections.Generic;
using Entities.Capacities;
using Photon.Pun;
using UnityEngine;

public class AffectCollider : MonoBehaviour
{
    [HideInInspector] public Entity caster;
    [HideInInspector] public ActiveCapacity capacitySender;
    [HideInInspector] public float speed;
    [SerializeField] private List<byte> effectIndex = new List<byte>();

    private void OnTriggerEnter(Collider other)
    {
        Entity entity = other.GetComponent<Entity>();

        if (entity && entity != caster)
        {
            IActiveLifeable activeLifeable = entity.GetComponent<IActiveLifeable>();
                
            if (PhotonNetwork.IsMasterClient)
            {
                capacitySender.CollideEffect(entity);
                /*activeLifeable.DecreaseCurrentHpRPC(damage);
                    
                gameObject.SetActive(false);
                    
                foreach (byte index in effectIndex)
                {
                    //TODO entity.AddPassive(index)
                }*/
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Entities.Capacities;
using Entities.FogOfWar;
using Photon.Pun;
using UnityEngine;

namespace Entities
{
    [RequireComponent(typeof(PhotonView))]
    public abstract partial class Entity : MonoBehaviourPun
    {
        /// <summary>
        /// The viewID of the photonView of the entity.
        /// </summary>
        public int entityIndex;

        /// <summary>
        /// True if passiveCapacities can be added to the entity's passiveCapacitiesList. False if not.
        /// </summary>
        [SerializeField] private bool canAddPassiveCapacity = true;

        /// <summary>
        /// True if passiveCapacities can be removed from the entity's passiveCapacitiesList. False if not.
        /// </summary>
        [SerializeField] private bool canRemovePassiveCapacity = true;

        /// <summary>
        /// The list of PassiveCapacity on the entity.
        /// </summary>
        public List<PassiveCapacity> passiveCapacitiesList = new List<PassiveCapacity>();

        /// <summary>
        /// The transform of the UI of the entity.
        /// </summary>
        public Transform TransformUI;

        /// <summary>
        /// The offset of the UI of the entity.
        /// </summary>
        public Vector3 OffsetUI = new Vector3(0, 2f, 0);

        void Start()
        { 
            entityIndex = photonView.ViewID;
            EntityCollectionManager.AddEntity(this); 
            OnStart();
        }

        protected virtual void OnStart()
        {
            if(canView)FogOfWarManager.Instance.AddFOWViewable(this);
        }
    
        void Update()
        {
            OnUpdate();   
        }
        protected virtual void OnUpdate(){}


        public PassiveCapacity GetPassiveCapacity(byte soIndex)
        {
            var passif = TryGetPassiveCapacity(soIndex);
            if (passif == default)
            {
                Debug.Log("Create Passif");
                passif = CapacitySOCollectionManager.Instance.CreatePassiveCapacity(soIndex, this);
                passiveCapacitiesList.Add(passif);
            }
            return passif;
        }
        
        public PassiveCapacity GetPassiveCapacity(Type type)
        {
            var passif = TryGetPassiveCapacity(type);
            return passif;
        }
        
        public PassiveCapacity TryGetPassiveCapacity(byte soIndex)
        {
            return passiveCapacitiesList.FirstOrDefault(item => item.indexOfSo == soIndex);
        }
        
        public PassiveCapacity TryGetPassiveCapacity(Type type)
        {
            return passiveCapacitiesList.FirstOrDefault(item => item.GetType() == type);
        }
    }
}

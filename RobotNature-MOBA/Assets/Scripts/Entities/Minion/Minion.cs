using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Capacities;
using Entities.FogOfWar;
using GameStates;
using GameStates.States;
using Photon.Pun;
using UI.InGame;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace Entities.Minion
{
    public class Minion : Entity, IActiveLifeable, IAttackable, ICastable, IDeadable, IMovable 
    {
        #region Minion Variables

        [SerializeField] private NavMeshAgent myNavMeshAgent;
        [SerializeField] private NavMeshObstacle myNavMeshObstacle;
        private MinionController myController;
        private Animator animator;

        [Header("Pathfinding")] 
        public List<Transform> myWaypoints = new();
        public List<Building.Building> towersList = new();
        [SerializeField] private int waypointIndex;
        [SerializeField] private int towerIndex;

        private enum MinionAggroState
        {
            None,
            Minion,
            Champion
        };

        [Header("Attack Logic")]
        public float attackDamage;
        public GameObject currentAttackTarget;
        [SerializeField] private MinionAggroState currentAggroState = MinionAggroState.None;
        [SerializeField] private LayerMask enemyMinionMask;
        [SerializeField] private bool attackCycle;
        [SerializeField] private ActiveCapacitySO attackAbility;
        
        private byte attackAbilityIndex;
        private double timer;
        
        public Transform meshParent;

        #endregion

        protected override void OnStart()
        {
            base.OnStart();
            myNavMeshAgent = GetComponent<NavMeshAgent>();
            myController = GetComponent<MinionController>();
            if (GetComponent<Animator>()) animator = GetComponent<Animator>();
            UIManager.Instance.InstantiateHealthBarForEntity(entityIndex);
            UIManager.Instance.InstantiateResourceBarForEntity(entityIndex);
            elementsToShow.Add(meshParent.gameObject);
            attackAbilityIndex = CapacitySOCollectionManager.GetActiveCapacitySOIndex(attackAbility);
            if (animator) animator.GetComponent<AnimationCallbacks>().caster = this;
        }

        private void OnEnable()
        {
            attackCycle = false;
            RequestSetCurrentHp(maxHp);
            waypointIndex = 0;
            myNavMeshAgent.enabled = true;
            myNavMeshObstacle.enabled = false;
        }

        private IEnumerator AttackLogic()
        {
            if (!PhotonNetwork.IsMasterClient) yield break;
            if (towersList is null) yield break;
            attackCycle = true;
            int[] targetEntity = { currentAttackTarget.GetComponent<Entity>().entityIndex };
            RequestAttack(attackAbilityIndex, targetEntity, Array.Empty<Vector3>());
            yield return new WaitForSeconds(attackAbility.cooldown);
            attackCycle = false;
        }

        #region State Methods

        public void IdleState()
        {
            if (!gameObject.activeSelf) return;
            if (animator is not null) animator.SetBool("isMoving", false);
            myNavMeshAgent.enabled = false;
            myNavMeshObstacle.enabled = true;
            CheckEnemies();
        }

        public void WalkingState()
        {
            if (animator is not null) animator.SetBool("isMoving", true);
            myNavMeshAgent.enabled = true;
            myNavMeshObstacle.enabled = false;
            CheckMyWaypoints();
            CheckObjectives();
            CheckEnemies();
        }

        public void LookingForPathingState()
        {
            if (myWaypoints is null) return;
            if (!gameObject.activeSelf) return;
            if (animator is not null) animator.SetBool("isMoving", true);
            myNavMeshAgent.enabled = true;
            myNavMeshObstacle.enabled = false;
            myNavMeshAgent.SetDestination(myWaypoints[waypointIndex].position);
            myController.currentState = MinionController.MinionState.Walking;
        }

        public void AttackingState()
        {
            if (Vector3.Distance(transform.position, currentAttackTarget.transform.position) > attackAbility.maxRange)
            {
                myController.currentState = MinionController.MinionState.LookingForPathing;
                currentAggroState = MinionAggroState.None;
                currentAttackTarget = null;
            }
            if (animator is not null) animator.SetBool("isMoving", false);
            if (currentAttackTarget is null)
            {
                myController.currentState = MinionController.MinionState.LookingForPathing;
                currentAggroState = MinionAggroState.None;
                currentAttackTarget = null;
                return;
            }
            if (!currentAttackTarget.GetComponent<IDeadable>().IsAlive())
            {
                myController.currentState = MinionController.MinionState.LookingForPathing;
                currentAggroState = MinionAggroState.None;
                currentAttackTarget = null;
                return;
            }
            myNavMeshAgent.enabled = false;
            myNavMeshObstacle.enabled = true;
            switch (currentAggroState)
            {
                case MinionAggroState.Minion:
                    if (currentAttackTarget.activeSelf && gameObject.activeSelf)
                    {
                        var q = Quaternion.LookRotation(currentAttackTarget.transform.position - transform.position);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 50f * Time.deltaTime);
                        if (!attackCycle)
                        {
                            StartCoroutine(AttackLogic());
                        }
                    }
                    else
                    {
                        myController.currentState = MinionController.MinionState.LookingForPathing;
                        currentAggroState = MinionAggroState.None;
                        currentAttackTarget = null;
                    }
                    break;
                
                case MinionAggroState.Champion:
                    if (currentAttackTarget != null && currentAttackTarget.activeSelf && gameObject.activeSelf)
                    {
                        var q = Quaternion.LookRotation(currentAttackTarget.transform.position - transform.position);
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 50f * Time.deltaTime);
                        if (!attackCycle)
                        {
                            StartCoroutine(AttackLogic());
                        }
                    }
                    else
                    {
                        myController.currentState = MinionController.MinionState.LookingForPathing;
                        currentAggroState = MinionAggroState.None;
                        currentAttackTarget = null;
                    }
                    break;
                
                case MinionAggroState.None:
                    myController.currentState = MinionController.MinionState.LookingForPathing;
                    currentAggroState = MinionAggroState.None;
                    currentAttackTarget = null;
                    break;
                
                default: throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Check Methods

        private void CheckMyWaypoints()
        {
            if (!gameObject.activeSelf) return;
            if (myWaypoints is null) return;
            var minionPosition = new Vector3(transform.position.x, 0, myWaypoints[waypointIndex].transform.position.z);
            var waypointPosition = new Vector3(myWaypoints[waypointIndex].transform.position.x, 0, myWaypoints[waypointIndex].transform.position.z);
            if (!(Vector3.Distance(minionPosition, waypointPosition) <= myNavMeshAgent.stoppingDistance)) return;
            if (waypointIndex < myWaypoints.Count - 1)
            {
                waypointIndex++;
                if (animator is not null) animator.SetBool("isMoving", true);
                myNavMeshAgent.SetDestination(myWaypoints[waypointIndex].position);
            }
            else
            {
                if (animator is not null) animator.SetBool("isMoving", true);
                myNavMeshAgent.SetDestination(towersList[towerIndex].GetComponent<Tower>().minionSpot.position);
            }
        }

        private void CheckObjectives()
        {
            if (towersList is null) return;

            if (!(Vector3.Distance(transform.position, towersList[towerIndex].GetComponent<Tower>().minionSpot.position) < 1)) return;
            ((InGameState)GameStateMachine.Instance.currentState).AddPoint(team);
            RequestDie();
        }

        private void CheckEnemies()
        {
            if (!gameObject.activeSelf) return;
            foreach (var objects in Physics.OverlapSphere(transform.position, attackAbility.maxRange, enemyMinionMask))
            {
                if (!objects.GetComponent<Entity>()) continue;
                var entity = objects.GetComponent<Entity>();
                if (!entity.GetComponent<IDeadable>().IsAlive()) continue;
                if (entity.team == team) continue;
                if (Vector3.Distance(transform.position, entity.transform.position) > attackAbility.maxRange) continue;
                if (entity is Minion)
                {
                    if (animator is not null) animator.SetBool("isMoving", false);
                    // myNavMeshAgent.SetDestination(transform.position);
                    myController.currentState = MinionController.MinionState.Attacking;
                    currentAggroState = MinionAggroState.Minion;
                    currentAttackTarget = entity.gameObject;
                    break;
                }
                if (entity is global::Champion)
                {
                    if (animator is not null) animator.SetBool("isMoving", false);
                    // myNavMeshAgent.SetDestination(transform.position);
                    myController.currentState = MinionController.MinionState.Attacking;
                    currentAggroState = MinionAggroState.Champion;
                    currentAttackTarget = entity.gameObject;
                    break;
                }
            }
        }

        #endregion

        #region Attackable

        private bool canAttack = true;
        
        public bool CanAttack()
        {
            return canAttack;
        }

        public void RequestSetCanAttack(bool value)
        {
            photonView.RPC("SetCanAttackRPC", RpcTarget.MasterClient, value);
        }

        public void SyncSetCanAttackRPC(bool value)
        {
            OnSetCanAttackFeedback?.Invoke(value);
        }

        public void SetCanAttackRPC(bool value)
        {
            OnSetCanAttack?.Invoke(value);
            photonView.RPC("SyncSetCanAttackRPC", RpcTarget.All, value);
        }

        public event GlobalDelegates.BoolDelegate OnSetCanAttack;
        public event GlobalDelegates.BoolDelegate OnSetCanAttackFeedback;

        public float GetAttackDamage()
        {
            return attackDamage;
        }

        public void RequestSetAttackDamage(float value)
        {
            photonView.RPC("SetAttackDamageRPC", RpcTarget.MasterClient, value);
        }

        [PunRPC]
        public void SyncSetAttackDamageRPC(float value)
        {
            OnSetAttackDamageFeedback?.Invoke(value);
        }

        [PunRPC]
        public void SetAttackDamageRPC(float value)
        {
            OnSetAttackDamage?.Invoke(value);
            photonView.RPC("SyncSetAttackDamageRPC", RpcTarget.All, value);
        }

        public event GlobalDelegates.FloatDelegate OnSetAttackDamage;
        public event GlobalDelegates.FloatDelegate OnSetAttackDamageFeedback;

        public void RequestAttack(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
        {
            photonView.RPC("AttackRPC", RpcTarget.MasterClient, capacityIndex, targetedEntities, targetedPositions);
        }

        [PunRPC]
        public void SyncAttackRPC(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
        {
            var attackCapacity = CapacitySOCollectionManager.CreateActiveCapacity(capacityIndex, this);
            if (animator is not null)
            {
                animator.SetTrigger("isAttacking");
            }
            attackCapacity.PlayFeedback(capacityIndex, targetedEntities, targetedPositions);
            OnAttackFeedback?.Invoke(capacityIndex, targetedEntities, targetedPositions);
        }

        [PunRPC]
        public void AttackRPC(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
        {
            var attackCapacity = CapacitySOCollectionManager.CreateActiveCapacity(capacityIndex, this);

            if (!attackCapacity.TryCast(targetedEntities, targetedPositions)) return;

            OnAttack?.Invoke(capacityIndex, targetedEntities, targetedPositions);
            photonView.RPC("SyncAttackRPC", RpcTarget.All, capacityIndex, targetedEntities, targetedPositions);
        }

        public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnAttack;
        public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnAttackFeedback;

        public void RequestIncreaseAttackDamage(float value)
        {
            photonView.RPC("IncreaseAttackDamageRPC", RpcTarget.MasterClient, value);
        }

        public void SyncIncreaseAttackDamageRPC(float value)
        {
            OnIncreaseAttackDamageFeedback?.Invoke(value);
        }

        public void IncreaseAttackDamageRPC(float value)
        {
            OnIncreaseAttackDamage?.Invoke(value);
            photonView.RPC("SyncIncreaseAttackDamageRPC", RpcTarget.All, value);
        }

        public event GlobalDelegates.FloatDelegate OnIncreaseAttackDamage;
        public event GlobalDelegates.FloatDelegate OnIncreaseAttackDamageFeedback;

        public void RequestDecreaseAttackDamage(float value)
        {
            photonView.RPC("DecreaseAttackDamageRPC", RpcTarget.MasterClient, value);
        }

        public void SyncDecreaseAttackDamageRPC(float value)
        {
            OnDecreaseAttackDamageFeedback?.Invoke(value);
        }

        public void DecreaseAttackDamageRPC(float value)
        {
            OnDecreaseAttackDamage?.Invoke(value);
            photonView.RPC("SyncDecreaseAttackDamageRPC", RpcTarget.All, value);
        }

        public event GlobalDelegates.FloatDelegate OnDecreaseAttackDamage;
        public event GlobalDelegates.FloatDelegate OnDecreaseAttackDamageFeedback;

        public void RequestIncreaseAttackSpeed(float value)
        {
            photonView.RPC("IncreaseAttackSpeedRPC", RpcTarget.MasterClient, value);
        }

        public void SyncIncreaseAttackSpeedRPC(float value)
        {
            OnIncreaseAttackSpeedFeedback?.Invoke(value);
        }

        public void IncreaseAttackSpeedRPC(float value)
        {
            OnIncreaseAttackSpeed?.Invoke(value);
            photonView.RPC("SyncIncreaseAttackSpeedRPC", RpcTarget.All, value);
        }

        public event GlobalDelegates.FloatDelegate OnIncreaseAttackSpeed;
        public event GlobalDelegates.FloatDelegate OnIncreaseAttackSpeedFeedback;

        public void RequestDecreaseAttackSpeed(float value)
        {
            photonView.RPC("DecreaseAttackSpeedRPC", RpcTarget.MasterClient, value);
        }

        public void SyncDecreaseAttackSpeedRPC(float value)
        {
            OnDecreaseAttackSpeedFeedback?.Invoke(value);
        }

        public void DecreaseAttackSpeedRPC(float value)
        {
            OnDecreaseAttackSpeed?.Invoke(value);
            photonView.RPC("SyncDecreaseAttackSpeedRPC", RpcTarget.All, value);
        }

        public event GlobalDelegates.FloatDelegate OnDecreaseAttackSpeed;
        public event GlobalDelegates.FloatDelegate OnDecreaseAttackSpeedFeedback;

        #endregion
        
        #region Moveable

        public float GetReferenceMoveSpeed()
        {
            throw new System.NotImplementedException();
        }

        public float GetCurrentMoveSpeed()
        {
            throw new System.NotImplementedException();
        }

        public void RequestSetReferenceMoveSpeed(float value)
        {
            throw new System.NotImplementedException();
        }

        public void SyncSetReferenceMoveSpeedRPC(float value)
        {
            throw new System.NotImplementedException();
        }

        public void SetReferenceMoveSpeedRPC(float value)
        {
            throw new System.NotImplementedException();
        }

        public event GlobalDelegates.FloatDelegate OnSetReferenceMoveSpeed;
        public event GlobalDelegates.FloatDelegate OnSetReferenceMoveSpeedFeedback;

        public void RequestIncreaseReferenceMoveSpeed(float amount)
        {
            throw new System.NotImplementedException();
        }

        public void SyncIncreaseReferenceMoveSpeedRPC(float amount)
        {
            throw new System.NotImplementedException();
        }

        public void IncreaseReferenceMoveSpeedRPC(float amount)
        {
            throw new System.NotImplementedException();
        }

        public event GlobalDelegates.FloatDelegate OnIncreaseReferenceMoveSpeed;
        public event GlobalDelegates.FloatDelegate OnIncreaseReferenceMoveSpeedFeedback;

        public void RequestDecreaseReferenceMoveSpeed(float amount)
        {
            throw new System.NotImplementedException();
        }

        public void SyncDecreaseReferenceMoveSpeedRPC(float amount)
        {
            throw new System.NotImplementedException();
        }

        public void DecreaseReferenceMoveSpeedRPC(float amount)
        {
            throw new System.NotImplementedException();
        }

        public event GlobalDelegates.FloatDelegate OnDecreaseReferenceMoveSpeed;
        public event GlobalDelegates.FloatDelegate OnDecreaseReferenceMoveSpeedFeedback;

        public void RequestSetCurrentMoveSpeed(float value)
        {
            throw new System.NotImplementedException();
        }

        public void SyncSetCurrentMoveSpeedRPC(float value)
        {
            throw new System.NotImplementedException();
        }

        public void SetCurrentMoveSpeedRPC(float value)
        {
            throw new System.NotImplementedException();
        }

        public event GlobalDelegates.FloatDelegate OnSetCurrentMoveSpeed;
        public event GlobalDelegates.FloatDelegate OnSetCurrentMoveSpeedFeedback;

        public void RequestIncreaseCurrentMoveSpeed(float amount)
        {
            throw new System.NotImplementedException();
        }

        public void SyncIncreaseCurrentMoveSpeedRPC(float amount)
        {
            throw new System.NotImplementedException();
        }

        public void IncreaseCurrentMoveSpeedRPC(float amount)
        {
            throw new System.NotImplementedException();
        }

        public event GlobalDelegates.FloatDelegate OnIncreaseCurrentMoveSpeed;
        public event GlobalDelegates.FloatDelegate OnIncreaseCurrentMoveSpeedFeedback;

        public void RequestDecreaseCurrentMoveSpeed(float amount)
        {
            throw new System.NotImplementedException();
        }

        public void SyncDecreaseCurrentMoveSpeedRPC(float amount)
        {
            throw new System.NotImplementedException();
        }

        public void DecreaseCurrentMoveSpeedRPC(float amount)
        {
            throw new System.NotImplementedException();
        }

        public event GlobalDelegates.FloatDelegate OnDecreaseCurrentMoveSpeed;
        public event GlobalDelegates.FloatDelegate OnDecreaseCurrentMoveSpeedFeedback;
        
        #endregion

        #region ActiveLifeable

        [SerializeField] private bool attackAffected = true;
        [SerializeField] private bool abilitiesAffected = true;
        [SerializeField] private float maxHp;
        private float currentHp;

        public float GetMaxHp()
        {
            return maxHp;
        }

        public float GetCurrentHp()
        {
            return currentHp;
        }

        public bool AttackAffected()
        {
            return attackAffected;
        }

        public bool AbilitiesAffected()
        {
            return abilitiesAffected;
        }

        public void RequestSetMaxHp(float value)
        {
            photonView.RPC("SetMaxHpRPC", RpcTarget.MasterClient, value);
        }

        [PunRPC]
        public void SyncSetMaxHpRPC(float value)
        {
            maxHp = value;
            currentHp = value;
            OnSetMaxHpFeedback?.Invoke(value);
        }

        [PunRPC]
        public void SetMaxHpRPC(float value)
        {
            maxHp = value;
            currentHp = value;
            OnSetMaxHp?.Invoke(value);
            photonView.RPC("SyncSetMaxHpRPC", RpcTarget.All, maxHp);
        }

        public event GlobalDelegates.FloatDelegate OnSetMaxHp;
        public event GlobalDelegates.FloatDelegate OnSetMaxHpFeedback;

        public void RequestIncreaseMaxHp(float amount)
        {
            photonView.RPC("IncreaseMaxHpRPC", RpcTarget.MasterClient, amount);
        }

        [PunRPC]
        public void SyncIncreaseMaxHpRPC(float amount)
        {
            maxHp = amount;
            currentHp = amount;
            OnIncreaseMaxHpFeedback?.Invoke(amount);
        }

        [PunRPC]
        public void IncreaseMaxHpRPC(float amount)
        {
            maxHp += amount;
            currentHp = amount;
            if (maxHp < currentHp) currentHp = maxHp;
            OnIncreaseMaxHp?.Invoke(amount);
            photonView.RPC("SyncIncreaseMaxHpRPC", RpcTarget.All, maxHp);
        }

        public event GlobalDelegates.FloatDelegate OnIncreaseMaxHp;
        public event GlobalDelegates.FloatDelegate OnIncreaseMaxHpFeedback;

        public void RequestDecreaseMaxHp(float amount)
        {
            photonView.RPC("DecreaseMaxHpRPC", RpcTarget.MasterClient, amount);
        }

        [PunRPC]
        public void SyncDecreaseMaxHpRPC(float amount)
        {
            maxHp = amount;
            if (maxHp < currentHp) currentHp = maxHp;
            OnDecreaseMaxHpFeedback?.Invoke(amount);
        }

        [PunRPC]
        public void DecreaseMaxHpRPC(float amount)
        {
            maxHp -= amount;
            if (maxHp < currentHp) currentHp = maxHp;
            OnDecreaseMaxHp?.Invoke(amount);
            photonView.RPC("SyncDecreaseMaxHpRPC", RpcTarget.All, maxHp);
        }

        public event GlobalDelegates.FloatDelegate OnDecreaseMaxHp;
        public event GlobalDelegates.FloatDelegate OnDecreaseMaxHpFeedback;

        public void RequestSetCurrentHp(float value)
        {
            photonView.RPC("SetCurrentHpRPC", RpcTarget.MasterClient, value);
        }

        [PunRPC]
        public void SyncSetCurrentHpRPC(float value)
        {
            currentHp = value;
            OnSetCurrentHpFeedback?.Invoke(value);
        }

        [PunRPC]
        public void SetCurrentHpRPC(float value)
        {
            currentHp = value;
            OnSetCurrentHp?.Invoke(value);
            photonView.RPC("SyncSetCurrentHpRPC", RpcTarget.All, value);
        }

        public event GlobalDelegates.FloatDelegate OnSetCurrentHp;
        public event GlobalDelegates.FloatDelegate OnSetCurrentHpFeedback;

        public void RequestIncreaseCurrentHp(float amount)
        {
            photonView.RPC("IncreaseCurrentHpRPC", RpcTarget.MasterClient, amount);
        }

        [PunRPC]
        public void SyncIncreaseCurrentHpRPC(float amount)
        {
            currentHp = amount;
            OnIncreaseCurrentHpFeedback?.Invoke(amount);
        }

        [PunRPC]
        public void IncreaseCurrentHpRPC(float amount)
        {
            currentHp += amount;
            if (currentHp > maxHp) currentHp = maxHp;
            OnIncreaseCurrentHp?.Invoke(amount);
            photonView.RPC("SyncIncreaseCurrentHpRPC", RpcTarget.All, currentHp);
        }

        public event GlobalDelegates.FloatDelegate OnIncreaseCurrentHp;
        public event GlobalDelegates.FloatDelegate OnIncreaseCurrentHpFeedback;

        public void RequestDecreaseCurrentHp(float amount)
        {
            photonView.RPC("DecreaseCurrentHpRPC", RpcTarget.MasterClient, amount);
        }

        [PunRPC]
        public void SyncDecreaseCurrentHpRPC(float amount) 
        {
            currentHp = amount;
            if (currentHp <= 0)
            {
                currentHp = 0;
                RequestDie();
            }
            OnDecreaseCurrentHpFeedback?.Invoke(amount);
        }

        [PunRPC]
        public void DecreaseCurrentHpRPC(float amount)
        {
            currentHp -= amount;
            OnDecreaseCurrentHp?.Invoke(amount);
            photonView.RPC("SyncDecreaseCurrentHpRPC", RpcTarget.All, currentHp);
        }
        
        public event GlobalDelegates.FloatDelegate OnDecreaseCurrentHp;
        public event GlobalDelegates.FloatDelegate OnDecreaseCurrentHpFeedback;
        
        public void RequestDecreaseCurrentHpByCapacity(float amount, byte capacityIndex)
        {
            photonView.RPC("DecreaseCurrentHpByCapacityRPC", RpcTarget.MasterClient, amount, capacityIndex);
        }
        
        [PunRPC]
        public void SyncDecreaseCurrentHpByCapacityRPC(float amount, byte capacityIndex)
        {
            currentHp = amount;
            if (currentHp <= 0)
            {
                currentHp = 0;
                RequestDie();
            }
            OnDecreaseCurrentHpCapacityFeedback?.Invoke(amount, capacityIndex);
        }

        [PunRPC]
        public void DecreaseCurrentHpByCapacityRPC(float amount, byte capacityIndex)
        {
            currentHp -= amount;
            OnDecreaseCurrentHpCapacity?.Invoke(amount, capacityIndex);
            photonView.RPC("SyncDecreaseCurrentHpByCapacityRPC", RpcTarget.All, currentHp, capacityIndex);
        }

        public event GlobalDelegates.FloatCapacityDelegate OnDecreaseCurrentHpCapacity;
        public event GlobalDelegates.FloatCapacityDelegate OnDecreaseCurrentHpCapacityFeedback;
        
        #endregion
        
        #region Deadable

        public bool isAlive;
        public bool canDie; 
        
        public bool IsAlive()
        {
            return isAlive;
        }

        public bool CanDie()
        {
            return canDie;
        }

        public void RequestSetCanDie(bool value)
        {
            photonView.RPC("SetCanDieRPC", RpcTarget.MasterClient, value);
        }

        public void SyncSetCanDieRPC(bool value)
        {
            canDie = value;
            OnSetCanDieFeedback?.Invoke(value);
        }

        public void SetCanDieRPC(bool value)
        {
            canDie = value;
            OnSetCanDie?.Invoke(value);
            photonView.RPC("SyncSetCanDieRPC", RpcTarget.All, value);
        }

        public event GlobalDelegates.BoolDelegate OnSetCanDie;
        public event GlobalDelegates.BoolDelegate OnSetCanDieFeedback;

        public void RequestDie()
        {
            photonView.RPC("DieRPC", RpcTarget.MasterClient);
        }

        [PunRPC]
        public void SyncDieRPC()
        {
            myController.currentState = MinionController.MinionState.Idle;
            isAlive = false;
            if (animator is not null)
            {
                animator.SetTrigger("isDying");
            }
            else
            {
                FogOfWarManager.Instance.RemoveFOWViewable(this);
                gameObject.SetActive(false);   
            }
            OnDieFeedback?.Invoke();
        }

        [PunRPC]
        public void DieRPC()
        {
            OnDie?.Invoke();
            photonView.RPC("SyncDieRPC", RpcTarget.All);
        }

        public event GlobalDelegates.NoParameterDelegate OnDie;
        public event GlobalDelegates.NoParameterDelegate OnDieFeedback;

        public void RequestRevive()
        {
            photonView.RPC("ReviveRPC", RpcTarget.MasterClient);
        }

        [PunRPC]
        public void SyncReviveRPC()
        {
            isAlive = true;
            OnReviveFeedback?.Invoke();
        }

        [PunRPC]
        public void ReviveRPC()
        {
            OnRevive?.Invoke();
            photonView.RPC("SyncReviveRPC", RpcTarget.All);
        }

        public event GlobalDelegates.NoParameterDelegate OnRevive;
        public event GlobalDelegates.NoParameterDelegate OnReviveFeedback;
        
        #endregion

        #region Castable
        
        public bool CanCast()
        {
            throw new NotImplementedException();
        }

        public void RequestSetCanCast(bool value)
        {
            throw new NotImplementedException();
        }

        public void SetCanCastRPC(bool value)
        {
            throw new NotImplementedException();
        }

        public void SyncSetCanCastRPC(bool value)
        {
            throw new NotImplementedException();
        }

        public event GlobalDelegates.BoolDelegate OnSetCanCast;
        public event GlobalDelegates.BoolDelegate OnSetCanCastFeedback;
        
        public void DecreaseCooldown()
        {
            throw new NotImplementedException();
        }

        public void RequestCast(byte capacityIndex, byte championCapacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
        {
            throw new NotImplementedException();
        }

        public void CastRPC(byte capacityIndex, byte championCapacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
        {
            throw new NotImplementedException();
        }

        public void CastAnimationFeedback()
        {
            throw new NotImplementedException();
        }

        public void CastAnimationShotEffect()
        {
            throw new NotImplementedException();
        }

        public void SyncCastRPC(byte capacityIndex, int[] targetedEntities, Vector3[] targetedPositions)
        {
            throw new NotImplementedException();
        }

        public void CastAnimationCast(Transform transform)
        {
            OnCastAnimationCast?.Invoke(transform);
        }

        public void CastAnimationEnd()
        {
            OnCastAnimationEnd?.Invoke();
        }

        public event GlobalDelegates.ByteIntArrayVector3ArrayDelegate OnCast;
        public event GlobalDelegates.TransformDelegate OnCastAnimationCast;
        public event GlobalDelegates.NoParameterDelegate OnCastAnimationEnd;
        public event GlobalDelegates.NoParameterDelegate OnCastAnimationShotEffect;
        public event GlobalDelegates.ByteIntArrayVector3ArrayCapacityDelegate OnCastFeedback;
        
        #endregion
    }
}
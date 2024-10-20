using System;
using System.Collections.Generic;
using _ScreamJam.Scripts.Agent;
using _ScreamJam.Scripts.Scriptables;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace _ScreamJam.Scripts.Agent
{
    public enum AgentState
    {
        Attack,
        Move,
        Idle,
        FollowPlayer,
        Search,
        Dead,
        Flee,
        Imprisoned,
    }
    
    public class AgentBase : MonoBehaviour, IDamageable
    {
        [Header("Stats")]
        public int Health;
        public int Dmg;
        public float Range;
        public float AtkRange;
        public float ThinkRate;
        public float MoveSpeed;
        public float RotSpeed;
        public bool IsHuman;
        [Header("Physics")]
        public Rigidbody Rb;
        
        public GameObjectListSO HumanSO; 
        public GameObjectListSO GoblinSO;

        public NavMeshAgent NavAgent;
        
        private float _ThinkCD;
        private Transform _Target;
        public AgentState _State;
        private bool _CanAttack;
        private float _GetHitCooldown;
        [SerializeField] private Animator _Animator;
        private float _DistToTargetSq;
        private Vector3 _ToTargetDir;
        private int _FormationIndex;
        private Vector3 _FormationPos;
        private GameObject _Player;
        protected virtual void Start()
        {
            _CanAttack = true;
            _Target = null;
            if(IsHuman)
                HumanSO.Add(gameObject);
            else
                GoblinSO.Add(gameObject);
        }

        protected virtual void OnDestroy()
        {
            if(IsHuman)
                HumanSO.Remove(gameObject);
            else
                GoblinSO.Remove(gameObject);
        }

        //to be called from animator event
        public void Attack()
        {
            if(_CanAttack)
                _CanAttack = false;
        }

        //to be called from animator event
        public void AttackFinish()
        {
            _CanAttack = true;
        }

        public virtual void FollowPlayer(int index, GameObject player)
        {
            if (_State == AgentState.Imprisoned || _State == AgentState.Dead)
                return;
            
            _State = AgentState.FollowPlayer;
            _FormationIndex = index;
            _Player = player;
            _CanAttack = false;

            //rows of 5
            int rowSize = 5;
            float x = (index % rowSize + 1) - rowSize * .5f;
            float y = index / rowSize + 1;
            float diameter = NavAgent.radius * 2.5f;//add buffer
            
            _FormationPos = new Vector3(x, 0.0f, y) * diameter;
        }

        private void OnCollisionEnter(Collision other)
        {

        }

        private void OnTriggerEnter(Collider other)
        {

        }

        protected virtual void Update()
        {
            if (_State == AgentState.Dead || _State == AgentState.Imprisoned)
                return;
            
            if (_ThinkCD < Time.time)
            {
                _ThinkCD = Time.time + ThinkRate;
                float rangeSQ = Range * Range;
                var targets = IsHuman ? GoblinSO.Value : HumanSO.Value;
                var closest = GameplayUtils.GetClosest(transform, targets, Range);
                if (closest is not null)
                {
                    _Target = closest.transform;
                }
                else
                {
                    _Target = null;
                }
            }

            if (_Target is not null)
            {
                _DistToTargetSq = math.distancesq(transform.position, _Target.position);
                _ToTargetDir = (_Target.position - transform.position);
                _ToTargetDir = new Vector3(_ToTargetDir.x, 0.0f, _ToTargetDir.z).normalized;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_ToTargetDir, Vector3.up), Time.deltaTime * RotSpeed);
            }

            if (Health <= 0)
            {
                Destroy(gameObject);
            }
            
            UpdateState();
        }

        protected virtual void UpdateState()
        {
            switch (_State)
            {
                case AgentState.Idle:
                    IdleState();
                    break;
                case AgentState.Move:
                    MoveState();
                    break;
                case AgentState.Attack:
                    AttackState();
                    break;
                case AgentState.FollowPlayer:
                    FollowState();
                    break;
                default:
                    break;
            }
            
        }
        
        protected virtual AgentState GetState()
        {
            return _State;
        }

        protected virtual void AttackState()
        {
            if (_Target is not null)
            {
                if (math.distancesq(_Target.position, transform.position) < AtkRange * AtkRange)
                    PlayAnimation("Attack");
                else
                    _State = AgentState.Move;
            }
            else
            {
                _State = AgentState.Idle;
            }
        }

        protected virtual void MoveState()
        {
            //move to atk range
            if (_Target is not null && _DistToTargetSq > AtkRange * AtkRange)
            {
                // Vector3 move = _ToTargetDir * MoveSpeed * Time.deltaTime;
                // Rb.MovePosition(transform.position + move);
                NavAgent.SetDestination(_Target.position);
                PlayAnimation("Move");
            }
            else if(_Target is not null)
            {
                _State = AgentState.Attack;
            }
            else
            {
                _State = AgentState.Idle;
            }
        }

        protected virtual void IdleState()
        {
            PlayAnimation("Idle");
            if (_Target is not null)
            {
                _State = AgentState.Move;
            }
        }
        
        protected virtual void FollowState()
        {
            //calculate position to follow player
            Transform p = _Player.transform;
            Vector3 formationPos = -p.forward * _FormationPos.z + p.right * _FormationPos.x + p.position;
            float r = NavAgent.stoppingDistance;
            //can start looking for enemies after getting close to pos
            if (math.distancesq(formationPos, transform.position) < r * r * 1.25)
            {
                _CanAttack = true;
                transform.forward = p.forward;
            }

            if (_CanAttack && _Target is not null)
                _State = AgentState.Move;
            NavAgent.SetDestination(formationPos);
        }
        
        protected void PlayAnimation(string name)
        {
            AnimatorStateInfo stateInfo = _Animator.GetCurrentAnimatorStateInfo(0);
            // Only trigger the animation if it's not currently playing
            if (!stateInfo.IsName(name))
            {
                _Animator.Play(name);
            }
        }

        public void TakeDamage(int dmg)
        {
            Health -= dmg;
            if (Health <= 0)
            {
                PlayAnimation("Die");
                _State = AgentState.Dead;
            }
        }

        //called from anim event
        public void OnDieAnim()
        {
            Destroy(gameObject);
        }
    }
}
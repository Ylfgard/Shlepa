using UnityEngine;
using UnityEngine.AI;
using PlayerController;
using System.Collections;

namespace Enemys.AIModules
{
    public class ChargeModule : AIModule
    {
        [Header("Charge")]
        [SerializeField] protected int _chargeDamage;
        [SerializeField] protected float _chargeDistance;
        [SerializeField] protected float _chargeTime;

        protected float _chargeDelay;
        protected bool _chargeUnlocked;
        protected float _inChargeTime;
        protected bool _chargeReady;
        protected bool _inCharge;
        protected float _chargeSpeed;
        protected float _chargeAceleration;
        protected Vector3 _chargeDir;

        protected Transform _transform;
        protected Transform _target;
        protected NavMeshAgent _agent;
        protected Player _player;
        protected Collider _collider;

        public bool InCharge => _inCharge;

        public bool ChargeReady
        { 
            get
            {
                if (_chargeUnlocked)
                    return _chargeReady;
                else
                    return false;
            }
        }

        protected void Start()
        {
            _chargeAceleration = 2 * _chargeDistance / Mathf.Pow(_chargeTime, 2);
        }

        public void Initialize(Enemy enemy, Collider collider)
        {
            _transform = enemy.Transform;
            _player = enemy.Player;
            _target = _player.Mover.Transform;
            _agent = enemy.Agent;
            _collider = collider;
        }

        public void PrepareCharge()
        {
            _chargeReady = true;
            _chargeUnlocked = false;
        }

        public void ChangeParameters(bool unlockState, float chargeDelay)
        {
            _chargeUnlocked = unlockState;
            _chargeDelay = chargeDelay;
        }

        public void Charging()
        {
            _inChargeTime += Time.fixedDeltaTime;
            if (_inChargeTime >= _chargeTime)
            {
                _inCharge = false;
                _collider.isTrigger = false;
                _agent.isStopped = false;
            }
            else
            {
                _chargeSpeed += _chargeAceleration * Time.fixedDeltaTime;
                _transform.position += _chargeDir * _chargeSpeed * Time.fixedDeltaTime;
            }
            return;
        }

        protected void OnTriggerEnter(Collider other)
        {
            if (other.tag == TagsKeeper.Player)
                _player.Parameters.TakeDamage(_chargeDamage);
        }

        public bool TryCharge()
        {
            if (_chargeUnlocked && _chargeReady)
            {
                RaycastHit hit;
                if (Physics.Raycast(_transform.position, _target.position - _transform.position, out hit, _chargeDistance))
                {
                    if (hit.collider.gameObject.tag == TagsKeeper.Player)
                    {
                        Charge();
                        return true;
                    }
                }
                return false;
            }
            return false;
        }

        protected void Charge()
        {
            _collider.isTrigger = true;
            _chargeReady = false;
            _inCharge = true;
            _chargeDir = (_target.position - _transform.position).normalized;
            _chargeDir.y = 0;
            _chargeSpeed = 0;
            _inChargeTime = 0;
            _agent.velocity = Vector3.zero;
            _agent.isStopped = true;
            StartCoroutine(ReloadCharge());
        }

        protected IEnumerator ReloadCharge()
        {
            yield return new WaitForSeconds(_chargeDelay);
            _chargeReady = true;
        }

#if UNITY_EDITOR
        protected void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _chargeDistance);
        }
#endif
    }
}

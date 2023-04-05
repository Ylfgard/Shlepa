using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Enemys.Bosses
{
    public class Slavonian : BossDistance
    {
        [Header ("Boss Stages")]
        [SerializeField] private SlavonianStage[] _stageParameters;

        [Header("Charge")]
        [SerializeField] protected int _chargeDamage;
        [SerializeField] protected float _chargeDistance;
        [SerializeField] protected float _chargeTime;

        protected Dictionary<int, SlavonianStage> _stages;

        protected float _chargeDelay;
        protected bool _chargeUnlocked;
        protected float _inChargeTime;
        protected bool _chargeReady;
        protected bool _inCharge;
        protected float _chargeSpeed;
        protected float _chargeAceleration;
        protected Vector3 _chargeDir;
        protected Collider _collider;

        protected override void Awake()
        {
            base.Awake();
            _stages = new Dictionary<int, SlavonianStage>();
            foreach (var stage in _stageParameters)
            {
                if (_stages.ContainsKey(stage.Index)) Debug.LogError("Wrong stage index! " + stage.Index);
                else _stages.Add(stage.Index, stage);
            }
            _chargeReady = true;
            _collider = GetComponent<Collider>();
        }

        protected override void Start()
        {
            base.Start();
            _chargeAceleration = 2 * _chargeDistance / Mathf.Pow(_chargeTime, 2);
        }

        protected override void FixedUpdate()
        {
            if (_inCharge)
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
            else
            {
                if (_chargeUnlocked && _chargeReady)
                    Charge();
            }
            
            base.FixedUpdate();
        }

        public override void ActivateStage(int stageIndex)
        {
            SlavonianStage stage;
            if (_stages.TryGetValue(stageIndex, out stage))
            {
                _chargeUnlocked = stage.UnlockCharge;
                _chargeDelay = stage.ChargeDelay;
            }
        }

        protected void OnTriggerEnter(Collider other)
        {
            if (other.tag == TagsKeeper.Player)
                _player.Parameters.TakeDamage(_chargeDamage);
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

        protected override void Attack()
        {
            base.Attack();
            _dir = Random.Range(-1, 1);
            MoveToTarget();
        }

        protected override void FastShot()
        {
            base.FastShot();
            MakeStep();
        }

        protected override IEnumerator Shot()
        {
            yield return base.Shot();
            MakeStep();
        }

#if UNITY_EDITOR
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _chargeDistance);
        }
#endif
    }
}

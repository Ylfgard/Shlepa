using UnityEngine;
using Enemys.AIModules;

namespace Enemys.Bosses
{
    public class Slavonian : Boss
    {
        [SerializeField] protected ChargeModule _charger;
        [SerializeField] protected MoveModule _mover;
        [SerializeField] protected AttackModule _attacker;

        [Header ("Boss Stages")]
        [SerializeField] private SlavonianStage[] _stages;

        protected override void Awake()
        {
            base.Awake();
            var collider = GetComponent<Collider>(); _activator.Initialize(this);
            _charger.Initialize(this, collider);
            SendDeath += _charger.Deactivate;
            _mover.Initialize(this);
            SendDeath += _mover.Deactivate;
            _attacker.Initialize(this);
            SendDeath += _attacker.Deactivate;
            foreach (SlavonianStage stage in _stages)
                stage.Initialize(this);
        }

        public override void Initialize(Vector3 position)
        {
            base.Initialize(position);
            _charger.PrepareCharge();
        }

        protected virtual void FixedUpdate()
        {
            if (_isAlive == false) return;

            if (_activator.CheckActivation() == false) return;

            if (_charger.InCharge)
            {
                _charger.Charging();
                return;
            }
            else
            {
                if (_charger.ChargeReady)
                {
                    if (_charger.TryCharge() == false)
                        _mover.MoveToTarget();
                }
                else
                {
                    if (_attacker.AttackReady)
                        Attack();
                    else
                        _mover.Move();
                }
            }
        }
        
        protected override void Attack()
        {
            if (_attacker.TryAttack() == false)
                _mover.MoveToTarget();
            else
                base.Attack();
        }

        public override void ActivateStage(BossStage stage)
        {
            SlavonianStage stg = stage as SlavonianStage;
            _charger.ChangeParameters(stg.UnlockCharge, stg.ChargeDelay);
        }
    }
}

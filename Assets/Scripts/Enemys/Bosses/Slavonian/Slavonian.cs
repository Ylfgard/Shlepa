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
            foreach (SlavonianStage stage in _stages)
                stage.Initialize(this);
        }

        protected override void Start()
        {
            base.Start();
            
            var collider = GetComponent<Collider>(); _activator.Initialize(this);
            _charger.Initialize(this, collider);
            SendDeath += _charger.Deactivate;
            _mover.Initialize(this);
            SendDeath += _mover.Deactivate;
            _attacker.Initialize(this);
            SendDeath += _attacker.Deactivate;
        }

        public override void Initialize(Vector3 position)
        {
            base.Initialize(position);
            _charger.PrepareCharge();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (_charger.InCharge)
            {
                _charger.Charging();
                return;
            }
            else
            {
                if (_charger.TryCharge() == false)
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
            base.Attack();
            if (_attacker.TryAttack() == false)
                _mover.MoveToTarget();
        }

        public override void ActivateStage(BossStage stage)
        {
            SlavonianStage stg = stage as SlavonianStage;
            _charger.ChangeParameters(stg.UnlockCharge, stg.ChargeDelay);
        }
    }
}

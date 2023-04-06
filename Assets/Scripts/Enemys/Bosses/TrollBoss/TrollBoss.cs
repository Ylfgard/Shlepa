using UnityEngine;
using Enemys.AIModules;

namespace Enemys.Bosses
{
    public class TrollBoss : Boss
    {
        [SerializeField] protected MoveModule _mover;
        [SerializeField] protected TrollBossAttackModule _attacker;

        [Header("Boss Stages")]
        [SerializeField] private TrollBossStage[] _stages;

        protected override void Awake()
        {
            base.Awake();
            foreach (TrollBossStage stage in _stages)
                stage.Initialize(this);
        }

        protected override void Start()
        {
            base.Start();

            _mover.Initialize(this);
            SendDeath += _mover.Deactivate;
            _attacker.Initialize(this);
            SendDeath += _attacker.Deactivate;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            
            if (_attacker.AttackReady)
                Attack();
            else
                _mover.Move();
        }

        protected override void Attack()
        {
            base.Attack();
            if (_attacker.TryAttack() == false)
                _mover.MoveToTarget();
        }

        public override void ActivateStage(BossStage stage)
        {
            _attacker.ChangeParameters(stage as TrollBossStage);
        }
    }
}
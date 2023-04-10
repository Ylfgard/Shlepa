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
            _mover.Initialize(this);
            SendDeath += _mover.Deactivate;
            _attacker.Initialize(this);
            SendDeath += _attacker.Deactivate;
            foreach (TrollBossStage stage in _stages)
                stage.Initialize(this);
        }

        protected virtual void FixedUpdate()
        {
            if (_isAlive == false) return;

            if (_activator.CheckActivation() == false) return;

            if (_attacker.AttackReady)
                Attack();
            else
                _mover.Move();
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
            _attacker.ChangeParameters(stage as TrollBossStage);
        }
    }
}
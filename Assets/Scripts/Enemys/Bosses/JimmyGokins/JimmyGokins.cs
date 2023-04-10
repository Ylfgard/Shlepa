using UnityEngine;
using Enemys.AIModules;

namespace Enemys.Bosses
{
    public class JimmyGokins : Boss
    {
        [SerializeField] protected MoveModule _mover;
        [SerializeField] protected AttackModule _attacker;
        [SerializeField] protected JumpModule _jumper;

        [Header("Boss Stages")]
        [SerializeField] private JimmyGokinsStage[] _stages;

        protected override void Awake()
        {
            base.Awake();
            _mover.Initialize(this);
            SendDeath += _mover.Deactivate;
            _attacker.Initialize(this);
            SendDeath += _attacker.Deactivate;
            _jumper.Initialize(this, GetComponent<CharacterController>());
            SendDeath += _jumper.Deactivate;

            _attacker.AttackFinished += _jumper.TryJump;

            foreach (var stage in _stages)
                stage.Initialize(this);
        }

        protected void FixedUpdate()
        {
            if (_isAlive == false) return;

            if (_activator.CheckActivation() == false) return;

            if (_jumper.OnGrounded == false)
            {
                _jumper.Move();
            }
            else
            {
                if (_attacker.AttackReady)
                    Attack();
                else
                    _mover.Move();
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
            JimmyGokinsStage stg = stage as JimmyGokinsStage;
            _attacker.SetAttackDelay(stg.AttackDelay);
        }
    }
}
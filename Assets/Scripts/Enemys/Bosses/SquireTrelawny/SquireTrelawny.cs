using UnityEngine;
using Enemys.AIModules;

namespace Enemys.Bosses
{
    public class SquireTrelawny : Boss
    {
        [SerializeField] protected KeepDistanceModule _mover;
        [SerializeField] protected AttackModule _meleeAttacker;
        [SerializeField] protected RangeAttackModule _rangeAttacker;
        [SerializeField] protected JumpModule _jumper;

        [Header("Boss Stages")]
        [SerializeField] private SquireTrelawnyStage[] _stages;

        protected bool _meleeAttackAllow;

        protected override void Awake()
        {
            base.Awake();
            _mover.Initialize(this);
            SendDeath += _mover.Deactivate;
            _meleeAttacker.Initialize(this);
            SendDeath += _meleeAttacker.Deactivate;
            _rangeAttacker.Initialize(this);
            SendDeath += _rangeAttacker.Deactivate;
            _jumper.Initialize(this, GetComponent<CharacterController>());
            SendDeath += _jumper.Deactivate;

            _meleeAttacker.AttackFinished += _jumper.TryJump;

            foreach (var stage in _stages)
                stage.Initialize(this);
        }

        public override void Initialize(Vector3 position)
        {
            base.Initialize(position);
            _meleeAttackAllow = false;
        }

        protected void FixedUpdate()
        {
            if (_activator.CheckActivation() == false) return;
            
            if (_jumper.OnGrounded == false)
            {
                _jumper.Move();
            }   
            else
            {
                if (_rangeAttacker.AttackReady)
                {
                    Attack();
                }
                else
                {
                    if (_meleeAttackAllow && _meleeAttacker.AttackReady)
                        MeleeAttack();
                    else
                        _mover.Move();
                }
            }
        }

        protected override void Attack()
        {
            if (_rangeAttacker.TryAttack() == false)
                _mover.MoveToTarget();
            else
                base.Attack();
        }

        protected void MeleeAttack()
        {
            if (_meleeAttacker.TryAttack() == false)
                _mover.MoveToTarget();
            else
                base.Attack();
        }

        public override void ActivateStage(BossStage stage)
        {
            SquireTrelawnyStage stg = stage as SquireTrelawnyStage;
            _meleeAttackAllow = stg.AllowMeleeAttack;
        }
    }
}
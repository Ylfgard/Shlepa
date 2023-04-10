using UnityEngine;
using Enemys.AIModules;

namespace Enemys
{
    public class MovingAttackerEnemy : LandingEnemy
    {
        [SerializeField] protected AttackModule _attacker;
        [SerializeField] protected MoveModule _mover;

        protected override void Awake()
        {
            base.Awake();
            _attacker.Initialize(this);
            SendDeath += _attacker.Deactivate;
            _mover.Initialize(this);
            SendDeath += _mover.Deactivate;
        }

        protected virtual void FixedUpdate()
        {
            if (_isAlive == false) return;

            if (_lander.TryLanded() == false) return;

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
    }
}
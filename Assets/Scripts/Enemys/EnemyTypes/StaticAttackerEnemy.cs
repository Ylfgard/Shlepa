using UnityEngine;
using Enemys.AIModules;

namespace Enemys
{
    public class StaticAttackerEnemy : LandingEnemy
    {
        [SerializeField] protected AttackModule _attacker;

        protected override void Awake()
        {
            base.Awake();
            _attacker.Initialize(this);
            SendDeath += _attacker.Deactivate;
        }

        protected virtual void FixedUpdate()
        {
            if (_isAlive == false) return;

            if (_lander.TryLanded() == false) return;

            if (_attacker.AttackReady)
                Attack();
        }

        protected override void Attack()
        {
            if (_attacker.TryAttack())
                base.Attack();
        }
    }
}
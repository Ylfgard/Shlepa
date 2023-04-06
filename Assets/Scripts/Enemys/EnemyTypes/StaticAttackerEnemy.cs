using UnityEngine;
using Enemys.AIModules;

namespace Enemys
{
    public class StaticAttackerEnemy : LandingEnemy
    {
        [SerializeField] protected AttackModule _attacker;

        protected override void Start()
        {
            base.Start();
            _attacker.Initialize(this);
            SendDeath += _attacker.Deactivate;
        }

        protected virtual void FixedUpdate()
        {
            if (_lander.TryLanded() == false) return;

            if (_attacker.AttackReady)
                Attack();
        }

        protected override void Attack()
        {
            base.Attack();
            _attacker.TryAttack();
        }
    }
}
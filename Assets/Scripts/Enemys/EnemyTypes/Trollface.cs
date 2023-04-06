using UnityEngine;
using Enemys.AIModules;

namespace Enemys
{
    public class Trollface : MovingAttackerEnemy
    {
        protected override void Start()
        {
            base.Start();
            _attacker.AttackFinished += Death;
        }

        public override void TakeDamage(int value)
        {
            _curHealth -= value;
            if (_curHealth <= 0)
            {
                _attacker.ForcedActivateAttack();
                Death();
            }
        }
    }
}
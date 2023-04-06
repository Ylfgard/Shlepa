using UnityEngine;
using Enemys.AIModules;

namespace Enemys
{
    public class Trollface : MovingAttackerEnemy
    {
        protected override void Awake()
        {
            base.Awake();
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
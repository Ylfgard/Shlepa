using UnityEngine;

namespace Enemys
{
    public class Head : WeakPoint
    {
        private const float DamageMultiplier = 2;

        public override void Initialize(Enemy enemy)
        {
            base.Initialize(enemy);
        }

        public override void TakeDamage(int value)
        {
            _enemy.TakeDamage(Mathf.RoundToInt(value * DamageMultiplier));
        }
    }
}

using UnityEngine;
using System;

namespace Enemys.Bosses
{
    [Serializable]
    public abstract class BossStage
    {
        [SerializeField] protected int _activateHealthPercent;

        private Boss _boss;

        public void Initialize(Boss boss)
        {
            _boss = boss;
            _boss.TakedDamage += CheckActivation;
        }

        private void CheckActivation(int healthPercent)
        {
            if (healthPercent > _activateHealthPercent) return;
            _boss.ActivateStage(this);
            _boss.TakedDamage -= CheckActivation;
        }
    }
}


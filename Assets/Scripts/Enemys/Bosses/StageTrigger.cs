using UnityEngine;
using System;

namespace Enemys.Bosses
{
    [Serializable]
    public class StageTrigger
    {
        [SerializeField] private int _index;
        [SerializeField] private int _activateHealthPercent;

        private Boss _boss;

        public void Initialize(Boss boss)
        {
            _boss = boss;
            _boss.TakedDamage += CheckActivation;
        }

        private void CheckActivation(int healthPercent)
        {
            if (healthPercent > _activateHealthPercent) return;
            _boss.ActivateStage(_index);
            _boss.TakedDamage -= CheckActivation;
        }
    }
}
using UnityEngine;
using System;

namespace Enemys.Bosses
{
    [Serializable]
    public class SquireTrelawnyStage : BossStage
    {
        [SerializeField] protected bool _allowMeleeAttack;

        public bool AllowMeleeAttack => _allowMeleeAttack;
    }
}
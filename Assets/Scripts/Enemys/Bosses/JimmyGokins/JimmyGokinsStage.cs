using UnityEngine;
using System;

namespace Enemys.Bosses
{
    [Serializable]
    public class JimmyGokinsStage : BossStage
    {
        [SerializeField] protected float _attackDelay;

        public float AttackDelay => _attackDelay;
    }
}
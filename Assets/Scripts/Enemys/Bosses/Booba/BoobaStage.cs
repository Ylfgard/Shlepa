using UnityEngine;
using System;

namespace Enemys.Bosses
{
    [Serializable]
    public class BoobaStage : BossStage
    {
        [SerializeField] private float _attackDelay;
        [SerializeField] private float _jumpTime;

        public float AttackDelay => _attackDelay;
        public float JumpTime => _jumpTime;
    }
}
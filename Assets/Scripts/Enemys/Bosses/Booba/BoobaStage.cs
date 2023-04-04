using UnityEngine;
using System;

namespace Enemys.Bosses
{
    [Serializable]
    public class BoobaStage
    {
        [SerializeField] private int _index;
        [SerializeField] private float _attackDelay;
        [SerializeField] private float _jumpTime;

        public int Index => _index;
        public float AttackDelay => _attackDelay;
        public float JumpTime => _jumpTime;
    }
}
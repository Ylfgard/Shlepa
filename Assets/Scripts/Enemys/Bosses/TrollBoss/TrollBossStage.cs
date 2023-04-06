using UnityEngine;
using System;

namespace Enemys.Bosses
{
    [Serializable]
    public class TrollBossStage : BossStage
    {
        [SerializeField] private float _attackDelay;
        [SerializeField] private float _dispersion;
        [SerializeField] private int _bulletsPerShot;
        [SerializeField] private float _attackDistance;

        public float AttackDelay => _attackDelay;
        public float Dispersion => _dispersion;
        public int BulletsPerShot => _bulletsPerShot;
        public float AttackDistance => _attackDistance;
    }
}
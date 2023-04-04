using UnityEngine;
using System;

namespace Enemys.Bosses
{
    [Serializable]
    public class TrollBossStage
    {
        [SerializeField] private int _index;
        [SerializeField] private float _attackDelay;
        [SerializeField] private float _dispersion;
        [SerializeField] private int _bulletsPerShot;
        [SerializeField] private float _attackDistance;
        [SerializeField] private float _distanceFromTarget;

        public int Index => _index;
        public float AttackDelay => _attackDelay;
        public float Dispersion => _dispersion;
        public int BulletsPerShot => _bulletsPerShot;
        public float AttackDistance => _attackDistance;
        public float DistanceFromTarget => _distanceFromTarget;
    }
}
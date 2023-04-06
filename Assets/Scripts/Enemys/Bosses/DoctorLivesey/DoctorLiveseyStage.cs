using UnityEngine;
using System;

namespace Enemys.Bosses
{
    [Serializable]
    public class DoctorLiveseyStage : BossStage
    {
        [SerializeField] protected bool _unlockCharge;
        [SerializeField] protected float _chargeDelay;
        [SerializeField] protected float _attackDelay;

        public bool UnlockCharge => _unlockCharge;
        public float ChargeDelay => _chargeDelay;
        public float AttackDelay => _attackDelay;
    }
}


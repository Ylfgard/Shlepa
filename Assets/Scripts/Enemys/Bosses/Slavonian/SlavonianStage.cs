using UnityEngine;
using System;

namespace Enemys.Bosses
{
    [Serializable]
    public class SlavonianStage : BossStage
    {
        [SerializeField] private bool _unlockCharge;
        [SerializeField] private float _chargeDelay;

        public bool UnlockCharge => _unlockCharge;
        public float ChargeDelay => _chargeDelay;
    }
}


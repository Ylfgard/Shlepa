using UnityEngine;
using System;

namespace Enemys.Bosses
{
    [Serializable]
    public class SlavonianStage
    {
        [SerializeField] private int _index;
        [SerializeField] private bool _unlockCharge;
        [SerializeField] private float _chargeDelay;

        public int Index => _index;
        public bool UnlockCharge => _unlockCharge;
        public float ChargeDelay => _chargeDelay;
    }
}


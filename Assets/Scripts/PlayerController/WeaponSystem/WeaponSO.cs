using UnityEngine;

namespace PlayerController.WeaponSystem
{
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Scriptable Objects/Weapon")]
    public class WeaponSO : ScriptableObject
    {
        [Header("Balance")]
        [SerializeField] private int _damage;
        [SerializeField] private int _bulletsPerShot;
        [SerializeField] private float _shotDelay;
        [SerializeField] private int _clipCapacity;
        [SerializeField] private float _distance;
        [SerializeField] private float _dispersionX;
        [SerializeField] private float _dispersionY;
        [SerializeField] private float _dispersionSpeed;
        [SerializeField] private float _aimValue;

        [Header("Projectile")]
        [SerializeField] private GameObject _projectile;

        [Header("Parameters")]
        [SerializeField] private float _shakeAngle;
        [SerializeField] private RuntimeAnimatorController _animController;
        [SerializeField] private int _slotIndex;
        [SerializeField] private bool _infiniteAmmo;

        public int Damage => _damage;
        public int BulletsPerShot => _bulletsPerShot;
        public float ShotDelay => _shotDelay;
        public int ClipCapacity => _clipCapacity;
        public float Distance => _distance;
        public float DispersionX => _dispersionX;
        public float DispersionY => _dispersionY;
        public float DispersionSpeed => _dispersionSpeed;
        public float ShakeAngle => _shakeAngle;
        public RuntimeAnimatorController AnimController => _animController;
        public int SlotIndex => _slotIndex;
        public bool InfiniteAmmo => _infiniteAmmo;
        public float AimValue 
        { 
            get
            {
                if (_aimValue < 1)
                    return 1;
                else
                    return _aimValue;
            }
        }
        public GameObject Projectile => _projectile;
    }
}
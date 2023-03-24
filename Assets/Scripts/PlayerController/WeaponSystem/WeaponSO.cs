using UnityEngine;

namespace PlayerController.WeaponSystem
{
    [CreateAssetMenu (fileName = "New Weapon", menuName = "Scriptable Objects/Weapon")]
    public class WeaponSO : ScriptableObject
    {
        [Header ("Balance")]
        [SerializeField] private int _damage;
        [SerializeField] private int _bullets;
        [SerializeField] private float _shotDelay;
        [SerializeField] private int _clipCapacity;
        [SerializeField] private float _reloadTime;
        [SerializeField] private int _ammoCapacity;
        [SerializeField] private float _distance;
        [SerializeField] private float _dispersionX;
        [SerializeField] private float _dispersionY;

        [Header("Parameters")]
        [SerializeField] private GameObject _prefab;
        [SerializeField] private int _slotIndex;

        public int Damage => _damage;
        public int Bullets => _bullets;
        public float ShotDelay => _shotDelay;
        public int ClipCapacity => _clipCapacity;
        public float ReloadTime => _reloadTime;
        public int AmmoCapacity => _ammoCapacity;
        public float Distance => _distance;
        public float DispersionX => _dispersionX;
        public float DispersionY => _dispersionY;
        public GameObject Prefab => _prefab;
        public int SlotIndex => _slotIndex;
    }
}
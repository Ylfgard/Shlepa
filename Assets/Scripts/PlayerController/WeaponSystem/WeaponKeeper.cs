using UnityEngine;
using System.Threading.Tasks;

namespace PlayerController.WeaponSystem
{
    public class WeaponKeeper : MonoBehaviour
    {
        private const int WeaponsCount = 6;

        [SerializeField] private GameObject _hitMarker;
        [SerializeField] private WeaponSO[] _availableWeapons;
        [SerializeField] private LayerMask _canBeShot;
        [SerializeField] private Transform _weaponDisplay;

        private Weapon[] _weapons;
        private int _curWIndx;

        public Weapon CurWeapon => _weapons[_curWIndx];

        private void Awake()
        {
            _weapons = new Weapon[WeaponsCount];
            foreach (var weapon in _availableWeapons)
            {
                _weapons[weapon.SlotIndex - 1] = new Weapon(_weaponDisplay, _canBeShot, _hitMarker, weapon);
                _curWIndx = weapon.SlotIndex - 1;
            }
        }

        public void ChangeWeapon(int slotNumber)
        {
            if (slotNumber < 0 || slotNumber >= WeaponsCount || _weapons[slotNumber - 1] == null) return;
            _weapons[_curWIndx].PutAway();
            _curWIndx = slotNumber - 1;
            _weapons[_curWIndx].TakeInHand();
        }
    }

    public class Weapon
    {
        private LayerMask _canBeShot;
        private int _damage;
        private int _bullets;
        private float _shotDelay;
        private int _clipCapacity;
        private float _reloadTime;
        private int _ammoCapacity;
        private float _distance;
        private float _dispersionX;
        private float _dispersionY;
        private Animator _animator;

        private int _bulletsInClip;
        private int _ammos;
        private bool _readyToShot;
        private bool _reloading;
        private GameObject _hitMarker;

        public int BulletsInClip => _bulletsInClip;
        public int Ammos => _ammos;

        public Weapon(Transform weaponDisplay, LayerMask canBeShot, GameObject hitMarker, WeaponSO parameters)
        {
            _canBeShot = canBeShot;
            _damage = parameters.Damage;
            _bullets = parameters.Bullets;
            _shotDelay = parameters.ShotDelay;
            _clipCapacity = parameters.ClipCapacity;
            _reloadTime = parameters.ReloadTime;
            _ammoCapacity = parameters.AmmoCapacity;
            _distance = parameters.Distance;
            _dispersionX = parameters.DispersionX;
            _dispersionY = parameters.DispersionY;

            // Debag part
            _hitMarker = hitMarker;
            _reloading = false;
            _readyToShot = true;
            _ammos = _ammoCapacity;
            _bulletsInClip = _clipCapacity;
            Object.Instantiate(parameters.Prefab, weaponDisplay);
            // End debag

            //_animator = Object.Instantiate(parameters.Prefab, weaponDisplay).GetComponent<Animator>();
            //_animator.gameObject.SetActive(false);
        }

        public void Shot(Transform weaponDir)
        {
            if (_reloading) return;

            if (_bulletsInClip == 0)
            {
                Reload();
                return;
            }

            if (_readyToShot == false) return;
            _bulletsInClip--;
            LaunchBullet(weaponDir);
            
            _readyToShot = false;
            PrepareWeapon();
        }

        private void LaunchBullet(Transform weaponDir)
        {
            for (int i = 0; i < _bullets; i++)
            {
                Vector3 dir = weaponDir.forward * _distance;
                float x = Random.Range(-_dispersionX, _dispersionX);
                dir += weaponDir.right * x;

                float YRange = Mathf.Sqrt((1 - Mathf.Pow(x, 2) / Mathf.Pow(_dispersionX, 2)) * Mathf.Pow(_dispersionY, 2));
                dir += weaponDir.up * Random.Range(-YRange, YRange);
                
                RaycastHit hit;
                if (Physics.Raycast(weaponDir.position, dir.normalized, out hit, _distance, _canBeShot))
                {
                    var marker = Object.Instantiate(_hitMarker, hit.point, Quaternion.identity);
                    Object.Destroy(marker, 10);
                }
            }

            Debug.Log("Shot bullet");
        }

        private async void PrepareWeapon()
        {
            await Task.Delay(Mathf.RoundToInt(_shotDelay * 1000));
            _readyToShot = true;
            Debug.Log("Weapon Ready");
        }

        public async void Reload()
        {
            if (_ammos == 0) return;

            _reloading = true;
            await Task.Delay(Mathf.RoundToInt(_reloadTime * 1000));
            // сделать async с вызовом анимации, задержкой и возможностью прервать перезарядку
            // либо StateMachine из аниматора

            if (_ammos <= _clipCapacity)
            {
                _bulletsInClip = _ammos;
                _ammos = 0;
            }
            else
            {
                _bulletsInClip = _clipCapacity;
                _ammos -= _clipCapacity;
            }

            _reloading = false;
            _readyToShot = true;
            Debug.Log("End reloading");
        }

        public void PutAway()
        {
            _animator.gameObject.SetActive(false);
        }

        public void TakeInHand()
        {
            _animator.gameObject.SetActive(true);
        }
    }
}
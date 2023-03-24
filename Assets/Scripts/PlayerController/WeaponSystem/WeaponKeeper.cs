using UnityEngine;
using System.Threading.Tasks;
using TMPro;

namespace PlayerController.WeaponSystem
{
    public class WeaponKeeper : MonoBehaviour
    {
        private const int WeaponsCount = 5;

        [SerializeField] private GameObject _hitMarker;
        [SerializeField] private Animator _animator;
        [SerializeField] private TextMeshProUGUI _clipText;
        [SerializeField] private TextMeshProUGUI _ammoText;
        [SerializeField] private WeaponSO[] _availableWeapons;
        [SerializeField] private LayerMask _canBeShot;

        private Weapon[] _weapons;
        private int _curWIndx;

        public Weapon CurWeapon => _weapons[_curWIndx];

        private void Awake()
        {
            _weapons = new Weapon[WeaponsCount];
            foreach (var weapon in _availableWeapons)
            {
                if (_weapons[weapon.SlotIndex - 1] != null)
                {
                    Debug.LogError("Error! Slot " + weapon.SlotIndex + " is taken!");
                    continue;
                }
                _weapons[weapon.SlotIndex - 1] = new Weapon(_canBeShot, _animator, _hitMarker,
                    _clipText, _ammoText, weapon);
            }
            _curWIndx = -1;
            ChangeWeapon(_availableWeapons[0].SlotIndex);
        }

        public async void ChangeWeapon(int slotNumber)
        {
            if (slotNumber <= 0 || slotNumber > WeaponsCount) return;
            if (_curWIndx == slotNumber - 1 || _weapons[slotNumber - 1] == null) return;
            if (_curWIndx != -1)
                await _weapons[_curWIndx].PutAway();
            _curWIndx = slotNumber - 1;
            await _weapons[_curWIndx].TakeInHand();
            Debug.Log("changeWeapon");
        }

        public void ReloadEnded()
        {
            CurWeapon.EndReloading();
        }
    }

    public class Weapon
    {
        private LayerMask _canBeShot;
        private int _damage;
        private int _bullets;
        private float _shotDelay;
        private int _clipCapacity;
        private float _distance;
        private float _dispersionX;
        private float _dispersionY;
        private RuntimeAnimatorController _animController;

        private GameObject _hitMarker;
        private Animator _animator;
        private TextMeshProUGUI _clipText;
        private TextMeshProUGUI _ammoText;

        private int _bulletsInClip;
        private int _ammos;
        private bool _readyToShot;
        private bool _reloading;
        

        public int BulletsInClip => _bulletsInClip;
        public int Ammos => _ammos;

        public Weapon(LayerMask canBeShot, Animator animator, GameObject hitMarker,
            TextMeshProUGUI clipText, TextMeshProUGUI ammoText, WeaponSO parameters)
        {
            _canBeShot = canBeShot;
            _animator = animator;
            _clipText = clipText;
            _ammoText = ammoText;

            _damage = parameters.Damage;
            _bullets = parameters.Bullets;
            _shotDelay = parameters.ShotDelay;
            _clipCapacity = parameters.ClipCapacity;
            _distance = parameters.Distance;
            _dispersionX = parameters.DispersionX;
            _dispersionY = parameters.DispersionY;
            _animController = parameters.AnimController;

            // Debag part
            _hitMarker = hitMarker;
            _reloading = false;
            _readyToShot = true;
            _ammos = 99999;
            _bulletsInClip = _clipCapacity;
            // End debag
        }

        public void Shot(Transform weaponDir)
        {
            if (_bulletsInClip == 0)
            {
                if (_reloading == false) 
                    Reload();
            }
            else
            {
                if (_readyToShot == false) return;
                
                if (_reloading) 
                    _reloading = false;
                
                _bulletsInClip--;
                LaunchBullet(weaponDir);
                _clipText.text = _bulletsInClip.ToString();
                _animator.SetTrigger("Shot");
                _readyToShot = false;
                PrepareWeapon();
            }
        }

        private void LaunchBullet(Transform weaponDir)
        {
            for (int i = 0; i < _bullets; i++)
            {
                Vector3 dir = weaponDir.forward * _distance;
                if (_dispersionX != 0 || _dispersionY != 0)
                {
                    float x = Random.Range(-_dispersionX, _dispersionX);
                    dir += weaponDir.right * x;

                    float YRange = Mathf.Sqrt((1 - Mathf.Pow(x, 2) / Mathf.Pow(_dispersionX, 2)) * Mathf.Pow(_dispersionY, 2));
                    dir += weaponDir.up * Random.Range(-YRange, YRange);
                }
                
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

        public void Reload()
        {
            if (_reloading) return;
            if (_ammos == 0 || _bulletsInClip == _clipCapacity) return;
            _reloading = true;
            _animator.SetTrigger("Reload");
        }

        public void EndReloading()
        {
            if (_bulletsInClip + _ammos <= _clipCapacity)
            {
                _bulletsInClip += _ammos;
                _ammos = 0;
            }
            else
            {
                _ammos += _bulletsInClip - _clipCapacity;
                _bulletsInClip = _clipCapacity;
            }

            _clipText.text = _bulletsInClip.ToString();
            _ammoText.text = _ammos.ToString();
            _reloading = false;
            _readyToShot = true;
            Debug.Log("End reloading");
        }

        public async Task PutAway()
        {
            _animator.SetTrigger("PutAway");
            _reloading = false;
            await Task.Delay(50);
        }

        public async Task TakeInHand()
        {
            _animator.runtimeAnimatorController = _animController;
            _animator.SetTrigger("TakeInHand");
            _ammoText.text = _ammos.ToString();
            _clipText.text = _bulletsInClip.ToString();
            await Task.Delay(50);
        }
    }
}
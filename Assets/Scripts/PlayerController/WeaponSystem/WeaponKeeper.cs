using UnityEngine;
using System.Threading.Tasks;
using TMPro;
using System;
using Enemys;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace PlayerController.WeaponSystem
{
    public class WeaponKeeper : MonoBehaviour
    {
        private const int WeaponsCount = 6;

        [SerializeField] private GameObject _hitMarker;
        [SerializeField] private Animator _animator;
        [SerializeField] private TextMeshProUGUI _clipText;
        [SerializeField] private TextMeshProUGUI _ammoText;
        [SerializeField] private WeaponSO[] _availableWeapons;
        [SerializeField] private LayerMask _canBeShot;

        private EnemyKeeper _enemyKeeper;
        private Weapon[] _weapons;
        private int _curWIndx;
        private Action _updateCall;

        public Weapon CurWeapon => _weapons[_curWIndx];

        private void Start()
        {
            _enemyKeeper = EnemyKeeper.Instance;

            _weapons = new Weapon[WeaponsCount];
            foreach (var weapon in _availableWeapons)
            {
                if (_weapons[weapon.SlotIndex - 1] != null)
                {
                    Debug.LogError("Error! Slot " + weapon.SlotIndex + " is taken!");
                    continue;
                }

                if (weapon.Projectile != null)
                {
                    _weapons[weapon.SlotIndex - 1] = new WeaponProjectile(_canBeShot, _animator, _enemyKeeper,
                        _hitMarker, _clipText, _ammoText, weapon);
                    continue;
                }

                if (weapon.DispersionSpeed > 0)
                {
                    var weaponAuto = new WeaponAuto(_canBeShot, _animator, _enemyKeeper,
                        _hitMarker, _clipText, _ammoText, weapon);
                    _updateCall += weaponAuto.UpdateDisperion;
                    _weapons[weapon.SlotIndex - 1] = weaponAuto;
                    continue;
                }

                _weapons[weapon.SlotIndex - 1] = new Weapon(_canBeShot, _animator, _enemyKeeper,
                    _hitMarker, _clipText, _ammoText, weapon);
            }

            _curWIndx = -1;
            ChangeWeapon(_availableWeapons[0].SlotIndex);
        }

        private void FixedUpdate()
        {
            _updateCall?.Invoke();
        }

        public async void ChangeWeapon(int slotNumber)
        {
            if (slotNumber <= 0 || slotNumber > WeaponsCount) return;
            Debug.Log(_weapons[slotNumber - 1]);
            if (_curWIndx == slotNumber - 1 || _weapons[slotNumber - 1] == null) return;

            if (_curWIndx != -1) await _weapons[_curWIndx].PutAway();
            _curWIndx = slotNumber - 1;
            await _weapons[_curWIndx].TakeInHand();
        }

        public void ReloadEnded()
        {
            CurWeapon.EndReloading();
        }
    }

    public class Weapon
    {
        protected LayerMask _canBeShot;
        protected int _damage;
        protected int _bulletsPerShot;
        protected float _shotDelay;
        protected int _clipCapacity;
        protected float _distance;
        protected float _dispersionX;
        protected float _dispersionY;
        protected float _aimValue;
        protected RuntimeAnimatorController _animController;
        protected bool _infiniteAmmo;

        protected GameObject _hitMarker;
        protected Animator _animator;
        protected EnemyKeeper _enemyKeeper;
        protected TextMeshProUGUI _clipText;
        protected TextMeshProUGUI _ammoText;

        protected int _bulletsInClip;
        protected int _ammos;
        protected bool _readyToShot;
        protected bool _reloading;
        

        public int BulletsInClip => _bulletsInClip;
        public int Ammos => _ammos;
        public float AimValue => _aimValue;

        public Weapon(LayerMask canBeShot, Animator animator, EnemyKeeper enemyKeeper, GameObject hitMarker,
            TextMeshProUGUI clipText, TextMeshProUGUI ammoText, WeaponSO parameters)
        {
            _canBeShot = canBeShot;
            _animator = animator;
            _enemyKeeper = enemyKeeper;
            _clipText = clipText;
            _ammoText = ammoText;

            _damage = parameters.Damage;
            _bulletsPerShot = parameters.BulletsPerShot;
            _shotDelay = parameters.ShotDelay;
            _clipCapacity = parameters.ClipCapacity;
            _distance = parameters.Distance;
            _dispersionX = parameters.DispersionX;
            _dispersionY = parameters.DispersionY;
            _animController = parameters.AnimController;
            _infiniteAmmo = parameters.InfiniteAmmo;
            _aimValue = parameters.AimValue;

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
            if (_clipCapacity == 0)
            {
                if (_ammos == 0 && _infiniteAmmo == false) return;
                if (_readyToShot == false) return;

                _ammos--;
                LaunchBullet(weaponDir);
                _ammoText.text = _ammos.ToString();
                _animator.SetTrigger("Shot");
                _readyToShot = false;
                PrepareWeapon();
            }
            else
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
        }

        protected virtual void LaunchBullet(Transform weaponDir)
        {
            for (int i = 0; i < _bulletsPerShot; i++)
            {
                Vector3 dir = CalculateDirecton(weaponDir);
                RaycastHit hit;
                if (Physics.Raycast(weaponDir.position, dir, out hit, _distance, _canBeShot))
                {
                    var marker = Object.Instantiate(_hitMarker, hit.point, Quaternion.identity);
                    Object.Destroy(marker, 3);
                    _enemyKeeper.MakeDamage(hit.collider.gameObject, _damage, false);
                }
            }
        }

        protected virtual Vector3 CalculateDirecton(Transform weaponDir)
        {
            Vector3 dir = weaponDir.forward * _distance;
            if (_dispersionX != 0 || _dispersionY != 0)
            {
                float x = Random.Range(-_dispersionX, _dispersionX);
                dir += weaponDir.right * x;

                float YRange = Mathf.Sqrt((1 - Mathf.Pow(x, 2) / Mathf.Pow(_dispersionX, 2)) * Mathf.Pow(_dispersionY, 2));
                dir += weaponDir.up * Random.Range(-YRange, YRange);
            }

            return dir.normalized;
        }

        protected async void PrepareWeapon()
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

            if (_infiniteAmmo) _ammoText.gameObject.SetActive(false);
            else _ammoText.gameObject.SetActive(true);
            _ammoText.text = _ammos.ToString();

            if (_clipCapacity == 0) _clipText.gameObject.SetActive(false);
            else _clipText.gameObject.SetActive(true);
            _clipText.text = _bulletsInClip.ToString();
            await Task.Delay(50);
        }
    }

    public class WeaponAuto : Weapon
    {
        private float _dispersionSpeed;
        private float _curDispersion;
        private bool _isShoting;

        public WeaponAuto(LayerMask canBeShot, Animator animator, EnemyKeeper enemyKeeper, GameObject hitMarker,
            TextMeshProUGUI clipText, TextMeshProUGUI ammoText, WeaponSO parameters) :
            base(canBeShot, animator, enemyKeeper, hitMarker, clipText, ammoText, parameters)
        {
            _canBeShot = canBeShot;
            _animator = animator;
            _enemyKeeper = enemyKeeper;
            _clipText = clipText;
            _ammoText = ammoText;

            _damage = parameters.Damage;
            _bulletsPerShot = parameters.BulletsPerShot;
            _shotDelay = parameters.ShotDelay;
            _clipCapacity = parameters.ClipCapacity;
            _distance = parameters.Distance;
            _dispersionX = parameters.DispersionX;
            _dispersionY = parameters.DispersionY;
            _dispersionSpeed = parameters.DispersionSpeed;
            _dispersionSpeed = parameters.DispersionSpeed;
            _animController = parameters.AnimController;
            _infiniteAmmo = parameters.InfiniteAmmo;
            
            _reloading = false;
            _readyToShot = true;
            _curDispersion = 0;
            _isShoting = false;

            // Debag part
            _hitMarker = hitMarker;
            _ammos = 99999;
            _bulletsInClip = _clipCapacity;
            // End debag
        }

        public void UpdateDisperion()
        {
            if (_isShoting)
            {
                if (_curDispersion < _dispersionSpeed)
                    _curDispersion += Time.fixedDeltaTime * 2;
                else
                    _curDispersion = _dispersionSpeed;
            }
            else
            {
                if (_curDispersion > 0)
                    _curDispersion -= Time.fixedDeltaTime / 2;
                else
                    _curDispersion = 0;
            }

            if (_readyToShot)
                _isShoting = false;
        }

        protected override Vector3 CalculateDirecton(Transform weaponDir)
        {
            _isShoting = true;
            Vector3 dir = weaponDir.forward * _distance;
            float disScale = _curDispersion / _dispersionSpeed;
            float curDisX = _dispersionX * disScale;
            float curDisY = _dispersionY * disScale;

            if (curDisX != 0 || curDisY != 0)
            {
                float x = Random.Range(-curDisX, curDisX);
                dir += weaponDir.right * x;

                float YRange = Mathf.Sqrt((1 - Mathf.Pow(x, 2) / Mathf.Pow(curDisX, 2)) * Mathf.Pow(curDisY, 2));
                dir += weaponDir.up * Random.Range(-YRange, YRange);
            }

            return dir.normalized;
        }
    }

    public class WeaponProjectile : Weapon
    {
        private ObjectPool<Grenade> _projectilesPool;

        public WeaponProjectile(LayerMask canBeShot, Animator animator, EnemyKeeper enemyKeeper, GameObject hitMarker,
            TextMeshProUGUI clipText, TextMeshProUGUI ammoText, WeaponSO parameters) :
            base(canBeShot, animator, enemyKeeper, hitMarker, clipText, ammoText, parameters)
        {
            _canBeShot = canBeShot;
            _animator = animator;
            _enemyKeeper = enemyKeeper;
            _clipText = clipText;
            _ammoText = ammoText;

            _damage = parameters.Damage;
            _bulletsPerShot = parameters.BulletsPerShot;
            _shotDelay = parameters.ShotDelay;
            _clipCapacity = parameters.ClipCapacity;
            _distance = parameters.Distance;
            _dispersionX = parameters.DispersionX;
            _dispersionY = parameters.DispersionY;
            _animController = parameters.AnimController;
            _infiniteAmmo = parameters.InfiniteAmmo;
            _projectilesPool = new ObjectPool<Grenade>(parameters.Projectile);

            _reloading = false;
            _readyToShot = true;

            // Debag part
            _hitMarker = hitMarker;
            _ammos = 99999;
            _bulletsInClip = _clipCapacity;
            // End debag
        }

        protected override void LaunchBullet(Transform weaponDir)
        {
            for (int i = 0; i < _bulletsPerShot; i++)
            {
                Vector3 dir = CalculateDirecton(weaponDir);
                _projectilesPool.GetObjectFromPool().Initialize(_enemyKeeper, weaponDir.position, dir, 5);
            }
        }
    }
}
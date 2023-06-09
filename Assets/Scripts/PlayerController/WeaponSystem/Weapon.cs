using UnityEngine;
using Enemys;
using TMPro;
using System.Threading.Tasks;
using VFX;

namespace PlayerController.WeaponSystem
{
    public class Weapon
    {
        protected const float _hitLifeTime = 2f;

        protected LayerMask _canBeCollided;
        protected LayerMask _canBeDamaged;
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
        protected float _shakeAngle;

        protected ObjectPool<HitMarker> _hitGround;
        protected ObjectPool<HitMarker> _hitEnemy;
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

        public Weapon(LayerMask canBeCollided, LayerMask canBeDamaged, Animator animator, EnemyKeeper enemyKeeper,
            ObjectPool<HitMarker> hitGround, ObjectPool<HitMarker> hitEnemy, TextMeshProUGUI clipText, TextMeshProUGUI ammoText, int startAmmo, WeaponSO parameters)
        {
            _canBeCollided = canBeCollided;
            _canBeDamaged = canBeDamaged;
            _animator = animator;
            _enemyKeeper = enemyKeeper;
            _clipText = clipText;
            _ammoText = ammoText;
            _ammos = startAmmo;
            _hitGround = hitGround;
            _hitEnemy = hitEnemy;

            _damage = parameters.Damage;
            _bulletsPerShot = parameters.BulletsPerShot;
            _shotDelay = parameters.ShotDelay;
            _clipCapacity = parameters.ClipCapacity;
            _distance = parameters.Distance;
            _dispersionX = parameters.DispersionX;
            _dispersionY = parameters.DispersionY;
            _shakeAngle = parameters.ShakeAngle;
            _animController = parameters.AnimController;
            _infiniteAmmo = parameters.InfiniteAmmo;
            _aimValue = parameters.AimValue;
            _reloading = false;
            _readyToShot = true;
            _bulletsInClip = _clipCapacity;
        }

        public void AddAmmos(int ammos, bool isActive)
        {
            _ammos += ammos;
            if (isActive) _ammoText.text = _ammos.ToString();
        }

        public virtual void SetAmmos(int ammos, bool isActive)
        {
            _ammos = ammos;
            if (isActive) _ammoText.text = _ammos.ToString();
        }

        public float Shot(Transform weaponDir)
        {
            if (_clipCapacity == 0)
            {
                if (_ammos == 0 && _infiniteAmmo == false) return 0;
                if (_readyToShot == false) return 0;

                _ammos--;
                LaunchBullet(weaponDir);
                _ammoText.text = _ammos.ToString();
                _animator.SetTrigger("Shot");
                _readyToShot = false;
                PrepareWeapon();
                return _shakeAngle;
            }
            else
            {
                if (_bulletsInClip == 0)
                {
                    if (_reloading == false)
                    {
                        Reload();
                    }
                    return 0;
                }
                else
                {
                    if (_readyToShot == false) return 0;

                    if (_reloading)
                        _reloading = false;

                    _bulletsInClip--;
                    LaunchBullet(weaponDir);
                    _clipText.text = _bulletsInClip.ToString();
                    _animator.SetTrigger("Shot");
                    _readyToShot = false;
                    PrepareWeapon();
                    return _shakeAngle;
                }
            }
        }

        protected virtual void LaunchBullet(Transform weaponDir)
        {
            for (int i = 0; i < _bulletsPerShot; i++)
            {
                Vector3 dir = CalculateDirecton(weaponDir);
                RaycastHit hit;
                if (Physics.Raycast(weaponDir.position, dir, out hit, _distance, _canBeCollided))
                {
                    RaycastHit enemyHit;
                    if (Physics.Raycast(weaponDir.position, dir, out enemyHit, Vector3.Distance(hit.point, weaponDir.position) * 1.1f, _canBeDamaged))
                    {
                        _hitEnemy.GetObjectFromPool().Initialize(enemyHit.point, Quaternion.identity, _hitLifeTime);
                        _enemyKeeper.MakeDamage(enemyHit.collider.gameObject, _damage, false);
                    }
                    else
                    {
                        Quaternion quaternion = Quaternion.LookRotation(hit.normal);
                        _hitGround.GetObjectFromPool().Initialize(hit.point, quaternion, _hitLifeTime);
                    }
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
        }

        public void PutAway()
        {
            _animator.SetTrigger("PutAway");
            _reloading = false;
        }

        public void TakeInHand()
        {
            _animator.runtimeAnimatorController = _animController;
            _animator.SetTrigger("TakeInHand");

            if (_infiniteAmmo) _ammoText.gameObject.SetActive(false);
            else _ammoText.gameObject.SetActive(true);
            _ammoText.text = _ammos.ToString();

            if (_clipCapacity == 0) _clipText.gameObject.SetActive(false);
            else _clipText.gameObject.SetActive(true);
            _clipText.text = _bulletsInClip.ToString();
        }
    }
}
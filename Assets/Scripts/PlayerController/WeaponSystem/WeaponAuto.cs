using UnityEngine;
using Enemys;
using TMPro;
    
namespace PlayerController.WeaponSystem
{
    public class WeaponAuto : Weapon
    {
        private float _dispersionSpeed;
        private float _curDispersion;
        private bool _isShoting;

        public WeaponAuto(LayerMask canBeCollided, LayerMask canBeDamaged, Animator animator, EnemyKeeper enemyKeeper, GameObject hitMarker,
            TextMeshProUGUI clipText, TextMeshProUGUI ammoText, int startAmmo, WeaponSO parameters) :
            base(canBeCollided, canBeDamaged, animator, enemyKeeper, hitMarker, clipText, ammoText, startAmmo, parameters)
        {
            _canBeCollided = canBeCollided;
            _canBeDamaged = canBeDamaged;
            _animator = animator;
            _enemyKeeper = enemyKeeper;
            _clipText = clipText;
            _ammoText = ammoText;
            _ammos = startAmmo;
            _bulletsInClip = 0;

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
}
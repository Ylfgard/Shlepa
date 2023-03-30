using UnityEngine;
using Enemys;
using TMPro;

namespace PlayerController.WeaponSystem
{
    public class WeaponProjectile : Weapon
    {
        private ObjectPool<Grenade> _projectilesPool;

        public WeaponProjectile(LayerMask canBeCollided, LayerMask canBeDamaged, Animator animator, EnemyKeeper enemyKeeper, GameObject hitMarker,
            TextMeshProUGUI clipText, TextMeshProUGUI ammoText, WeaponSO parameters) :
            base(canBeCollided, canBeDamaged, animator, enemyKeeper, hitMarker, clipText, ammoText, parameters)
        {
            _canBeCollided = canBeCollided;
            _canBeDamaged = canBeDamaged;
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
                _projectilesPool.GetObjectFromPool().Initialize(_enemyKeeper, weaponDir.position, dir, _damage, 5);
            }
        }
    }
}
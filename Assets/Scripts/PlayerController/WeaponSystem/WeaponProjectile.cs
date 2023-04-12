using UnityEngine;
using Enemys;
using TMPro;
using VFX;

namespace PlayerController.WeaponSystem
{
    public class WeaponProjectile : Weapon
    {
        private ObjectPool<Grenade> _projectilesPool;

        public WeaponProjectile(LayerMask canBeCollided, LayerMask canBeDamaged, Animator animator, EnemyKeeper enemyKeeper, ObjectPool<HitMarker> hitGround, 
            ObjectPool<HitMarker> hitEnemy, TextMeshProUGUI clipText, TextMeshProUGUI ammoText, int startAmmo, WeaponSO parameters) :
            base(canBeCollided, canBeDamaged, animator, enemyKeeper, hitGround, hitEnemy, clipText, ammoText, startAmmo, parameters)
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
            _projectilesPool = new ObjectPool<Grenade>(parameters.Projectile);
            _projectilesPool.PreSpawn(3);

            _reloading = false;
            _readyToShot = true;
            _bulletsInClip = _clipCapacity;
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
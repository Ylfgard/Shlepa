using UnityEngine;
using Enemys.Projectiles;
using System.Collections;

namespace Enemys.AIModules
{
    public class RangeAttackModule : AttackModule
    {
        [Header("Shot Parameters")]
        [SerializeField] protected ProjectileType _projectileType;
        [SerializeField] protected int _bulletsPerShot;
        [SerializeField] protected bool _randomDir;
        [SerializeField] protected float _dispersion;
        [SerializeField] protected float _bulletDelay;
        [SerializeField] protected Transform _shotPoint;
        
        protected float _bulletLifeTime;
        protected ObjectPool<Bullet> _bullets;
        protected LayerMask _canBeCollided;
        protected LayerMask _canBeDamaged;
        protected float _bulletSpeed;

        protected virtual void Start()
        {
            switch (_projectileType)
            {
                case ProjectileType.Bullet:
                    _bullets = ProjectilePoolsKeeper.Instance.Bullets;
                    break;
                case ProjectileType.Cannonball:
                    _bullets = ProjectilePoolsKeeper.Instance.Cannonballs;
                    break;
                case ProjectileType.Bomb:
                    _bullets = ProjectilePoolsKeeper.Instance.Bombs;
                    break;
                case ProjectileType.ExplosiveBullet:
                    _bullets = ProjectilePoolsKeeper.Instance.ExplosiveBullets;
                    break;
                case ProjectileType.BossCannonball:
                    _bullets = ProjectilePoolsKeeper.Instance.BossCannonballs;
                    break;
                default:
                    Debug.LogError("Wrong projectile type!");
                    break;
            }
            var bullet = _bullets.Value;
            _canBeCollided = bullet.CanBeCollided;
            _canBeDamaged = bullet.CanBeDamaged;
            _bulletSpeed = bullet.StartSpeed;
            _bulletLifeTime = _attackDamageArea / _bulletSpeed;
        }

        protected override bool Attack()
        {
            RaycastHit hit;
            if (Physics.Raycast(_shotPoint.position, _target.position - _shotPoint.position, out hit, _attackDistance, _canBeCollided))
            {
                if (hit.collider.gameObject.tag == TagsKeeper.Player)
                {
                    return base.Attack();
                }
            }
            return false;
        }

        protected override IEnumerator ActivateAttack()
        {
            yield return new WaitForSeconds(_delayBeforeActivating);
            if (_bulletDelay > 0)
                StartCoroutine(Shot());
            else
                FastShot();
        }

        protected virtual void FastShot()
        {
            for (int i = 0; i < _bulletsPerShot; i++)
            {
                var bullet = _bullets.GetObjectFromPool();
                Vector3 dir = CalculateBulletDir(i);
                bullet.Initialize(_shotPoint.position, dir, _damage, _bulletLifeTime);
            }
            _agent.isStopped = false;
            AttackFinished?.Invoke();
        }

        protected virtual IEnumerator Shot()
        {
            for (int i = 0; i < _bulletsPerShot; i++)
            {
                var bullet = _bullets.GetObjectFromPool();
                Vector3 dir = CalculateBulletDir(i);
                bullet.Initialize(_shotPoint.position, dir, _damage, _bulletLifeTime);
                yield return new WaitForSeconds(_bulletDelay);
            }
            _agent.isStopped = false;
            AttackFinished?.Invoke();
        }

        protected virtual Vector3 CalculateBulletDir(int number)
        {
            Vector3 dir = _target.position - _shotPoint.position;
            dir = _player.Mover.GetFuturePos(dir.magnitude / _bulletSpeed) - _shotPoint.position;
            if (_dispersion != 0)
            {
                float y = dir.y;
                dir.y = 0;
                float distance = dir.magnitude;
                Vector3 normal = Vector3.right;
                Vector3.OrthoNormalize(ref dir, ref normal);
                float x = _bulletsPerShot / 2;
                if (_randomDir)
                    x = Random.Range(-_dispersion, _dispersion);
                else
                    x = _dispersion * ((number - x) / x);

                dir = dir * _attackDistance + normal * x;
                dir = dir.normalized * distance;
                dir.y = y;
            }
            return dir.normalized;
        }

        public void SetBulletsPerShot(int bullets)
        {
            _bulletsPerShot = bullets;
        }
    }
}
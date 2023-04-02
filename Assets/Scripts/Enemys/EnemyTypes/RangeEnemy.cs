using UnityEngine;
using Enemys.Projectiles;
using System.Collections;

namespace Enemys
{
    public class RangeEnemy : Enemy
    {
        [Header("Shot Parameters")]
        [SerializeField] protected ProjectileType _projectileType;
        [SerializeField] protected int _bulletsPerShot;
        [SerializeField] protected bool _randomDir;
        [SerializeField] protected float _dispersion;
        [SerializeField] protected float _bulletDelay;
        [SerializeField] protected Transform _shotPoint;
        [SerializeField] protected float _bulletLifeTime;

        protected ObjectPool<Bullet> _bullets;
        protected LayerMask _canBeCollided;
        protected LayerMask _canBeDamaged;

        protected override void Start()
        {
            base.Start();
            _animationController.CallBack += MakeShot;
            switch(_projectileType)
            {
                case ProjectileType.Bullet:
                    _bullets = ProjectilePoolsKeeper.Instance.Bullets;
                    break;
                case ProjectileType.Canonball:
                    _bullets = ProjectilePoolsKeeper.Instance.Canonballs;
                    break;
                case ProjectileType.Bomb:
                    _bullets = ProjectilePoolsKeeper.Instance.Bombs;
                    break;
                case ProjectileType.ExplosiveBullet:
                    _bullets = ProjectilePoolsKeeper.Instance.ExplosiveBullets;
                    break;
                default:
                    Debug.LogError("Wrong projectile type!");
                    break;
            }
            var bullet = _bullets.Value;
            _canBeCollided = bullet.CanBeCollided;
            _canBeDamaged = bullet.CanBeDamaged;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (_isAttacking == false)
            {
                if (Vector3.Distance(_transform.position, _target.position) <= _attackDistance)
                    Attack();
            }
        }

        protected override void Attack()
        {
            RaycastHit hit;
            if (Physics.Raycast(_shotPoint.position, _target.position - _shotPoint.position, out hit, _attackDistance, _canBeCollided))
            {
                if (Physics.OverlapSphere(hit.point, 0.01f, _canBeDamaged).Length > 0)
                {
                    base.Attack();
                    _agent.isStopped = true;
                }
            }
        }

        protected void MakeShot()
        {
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
        }

        protected virtual IEnumerator Shot()
        {
            Vector3 dirToTarget = _target.position - _shotPoint.position;
            for (int i = 0; i < _bulletsPerShot; i++)
            {
                var bullet = _bullets.GetObjectFromPool();
                Vector3 dir = CalculateBulletDir(i);
                bullet.Initialize(_shotPoint.position, dir, _damage, _bulletLifeTime);
                yield return new WaitForSeconds(_bulletDelay);
            }
            _agent.isStopped = false;
        }

        protected virtual Vector3 CalculateBulletDir(int number)
        {
            Vector3 dir = _target.position - _shotPoint.position;
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
    }

    public enum ProjectileType
    {
        Bullet,
        Canonball,
        Bomb,
        ExplosiveBullet
    }
}
using UnityEngine;
using Enemys.Projectiles;
using System.Collections;

namespace Enemys
{
    public class RangeEnemy : Enemy
    {
        [Header("Shot Parameters")]
        [SerializeField] protected GameObject _bullet;
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

            //FOR DEBUG!!!
            if (_bullets == null)
                _bullets = new ObjectPool<Bullet>(_bullet);
            var bullet = _bullets.Value;
            _canBeCollided = bullet.CanBeCollided;
            _canBeDamaged = bullet.CanBeDamaged;
        }

        public virtual void Initialize(Vector3 position, bool active, ObjectPool<Bullet> bullets)
        {
            _transform.position = position;
            _curHealth = _maxHealth;
            _isActive = active;
            _isAttacking = false;
            _bullets = bullets;
            var bullet = _bullets.Value;
            _canBeCollided = bullet.CanBeCollided;
            _canBeDamaged = bullet.CanBeDamaged;
        }

        protected virtual void FixedUpdate()
        {
            if (_isActive == false)
            {
                if (Vector3.Distance(_transform.position, _target.position) <= _triggerDistance)
                {
                    _isActive = true;
                }
                return;
            }

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
                if (_canBeDamaged == hit.collider.gameObject.layer)
                    base.Attack();
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
        }

        protected virtual Vector3 CalculateBulletDir(int number)
        {
            Vector3 dir = _target.position - _shotPoint.position;
            if (_dispersion != 0)
            {
                dir = dir.normalized * _attackDistance;
                Vector3 normal = Vector3.forward;
                Vector3.OrthoNormalize(ref dir, ref normal);
                float x = _bulletsPerShot / 2;
                if (_randomDir)
                    x = Random.Range(-_dispersion, _dispersion);
                else
                    x = _dispersion * ((number - x) / x);
                
                dir += normal * x;
            }
            return dir.normalized;
        }
    }
}
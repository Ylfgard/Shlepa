using UnityEngine;
using Enemys.Projectiles;
using System.Threading.Tasks;

namespace Enemys
{
    public class RangeEnemy : Enemy
    {
        [Header("Shot Parameters")]
        [SerializeField] protected GameObject _bullet;
        [SerializeField] protected int _bulletsPerShot;
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

        protected virtual async void MakeShot()
        {
            for (int i = 0; i < _bulletsPerShot; i++)
            {
                var bullet = _bullets.GetObjectFromPool();
                Vector3 dir = CalculateBulletDir();
                bullet.Initialize(_shotPoint.position, dir, _damage, _bulletLifeTime);
                await Task.Delay(Mathf.RoundToInt(_bulletDelay * 1000));
            }
        }

        protected virtual Vector3 CalculateBulletDir()
        {
            Vector3 dir = _target.position - _shotPoint.position;
            return dir.normalized;
        }
    }
}
using UnityEngine;
using Enemys.Projectiles;
using System.Collections;

namespace Enemys
{
    public class RangeEnemy : Enemy
    {
        protected const float _spawnHight = 10;
        protected const float _fallSpeed = 20;

        [Header("Spawn Parameters")]
        [SerializeField] protected LayerMask _groundLayer;
        [SerializeField] protected float _landingAreaRadius;
        [SerializeField] protected int _landingDamage;

        [Header("Shot Parameters")]
        [SerializeField] protected ProjectileType _projectileType;
        [SerializeField] protected int _bulletsPerShot;
        [SerializeField] protected bool _randomDir;
        [SerializeField] protected float _dispersion;
        [SerializeField] protected float _bulletDelay;
        [SerializeField] protected Transform _shotPoint;
        [SerializeField] protected float _bulletLifeTime;

        protected bool _isLanding;
        protected ObjectPool<Bullet> _bullets;
        protected LayerMask _canBeCollided;
        protected LayerMask _canBeDamaged;
        protected float _bulletSpeed;

        protected override void Start()
        {
            base.Start();
            _animationController.CallBack += MakeShot;
            switch(_projectileType)
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
        }

        public override void Initialize(Vector3 position)
        {
            position.y += _spawnHight;
            base.Initialize(position);
            _isLanding = true;
            _agent.enabled = false;
        }

        protected virtual void FixedUpdate()
        {
            if (_isLanding)
            {
                CheckLanding();
                return;
            }

            if (_isAttacking == false)
            {
                if (Vector3.Distance(_transform.position, _target.position) <= _attackDistance)
                    Attack();
            }
        }

        public override float LandingAreaRadius()
        {
            return _landingAreaRadius;
        }

        protected void CheckLanding()
        {
            if (Physics.Raycast(_transform.position, Vector3.down, 2f, _groundLayer))
            {
                Landing();
            }
            else
            {
                Vector3 newPos = _transform.position;
                newPos.y -= _fallSpeed * Time.fixedDeltaTime;
                _transform.position = newPos;
            }
        }

        protected void Landing()
        {
            if (Vector3.Distance(_transform.position, _target.position) <= _landingAreaRadius)
                _player.Parameters.TakeDamage(_landingDamage);
            _isLanding = false;
            _agent.enabled = true;
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

#if UNITY_EDITOR
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            Gizmos.DrawWireSphere(transform.position, _landingAreaRadius);
        }
#endif
    }
}
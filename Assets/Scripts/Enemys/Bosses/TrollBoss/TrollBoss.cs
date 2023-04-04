using UnityEngine;
using System.Collections.Generic;
using Enemys.Projectiles;
using System.Collections;
using Enemys.Cowers;
using UnityEngine.AI;

namespace Enemys.Bosses
{
    public class TrollBoss : Boss
    {
        [SerializeField] private TrollBossStage[] _stageParameters;

        [Header("Shot Parameters")]
        [SerializeField] protected ProjectileType _projectileType;
        [SerializeField] protected int _bulletsPerShot;
        [SerializeField] protected bool _randomDir;
        [SerializeField] protected float _dispersion;
        [SerializeField] protected float _bulletDelay;
        [SerializeField] protected Transform _shotPoint;
        [SerializeField] protected float _bulletLifeTime;

        [Header("Cower")]
        [SerializeField] protected float _distanceFromTarget;
        [SerializeField] protected float _changeCowerDistance;

        protected Dictionary<int, TrollBossStage> _stages;
        protected ObjectPool<Bullet> _bullets;
        protected LayerMask _canBeCollided;
        protected LayerMask _canBeDamaged;
        protected float _bulletSpeed;
        protected CowerKeeper _cowerKeeper;
        protected Cower _curCower;
        protected bool _moveToCover;
        protected bool _moveToAlternativeCower;
        protected bool _inCower;

        protected override void Awake()
        {
            base.Awake();
            _stages = new Dictionary<int, TrollBossStage>();
            foreach (var stage in _stageParameters)
            {
                if (_stages.ContainsKey(stage.Index)) Debug.LogError("Wrong stage index! " + stage.Index);
                else _stages.Add(stage.Index, stage);
            }
        }

        protected override void Start()
        {
            base.Start();
            _cowerKeeper = CowerKeeper.Instance;
            _animationController.CallBack += MakeShot;
            switch (_projectileType)
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
            _bulletSpeed = bullet.StartSpeed;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (_isAttacking == false)
            {
                if (Vector3.Distance(_transform.position, _target.position) <= _attackDistance)
                    Attack();
                else
                    MoveToAttack();
            }
            else
            {
                if (Vector3.Distance(_agent.destination, _transform.position) <= 1f) _inCower = true;
                TakeCover();
            }
        }

        public override void ActivateStage(int stageIndex)
        {
            TrollBossStage stage;
            if (_stages.TryGetValue(stageIndex, out stage))
            {
                _attackDelay = stage.AttackDelay;
                _dispersion = stage.Dispersion;
                _bulletsPerShot = stage.BulletsPerShot;
                _attackDistance = stage.AttackDistance;
                _distanceFromTarget = stage.DistanceFromTarget;
            }
        }

        protected override void Attack()
        {
            _moveToCover = false;
            _moveToAlternativeCower = false;
            _inCower = false;
            RaycastHit hit;
            if (Physics.Raycast(_shotPoint.position, _target.position - _shotPoint.position, out hit, _attackDistance, _canBeCollided))
            {
                if (Physics.OverlapSphere(hit.point, 0.01f, _canBeDamaged).Length > 0)
                {
                    base.Attack();
                    _agent.isStopped = true;
                }
            }
            MoveToAttack();
        }

        protected void MakeShot()
        {
            if (_bulletDelay > 0)
                StartCoroutine(Shot());
            else
                FastShot();
        }

        protected void FastShot()
        {
            for (int i = 0; i < _bulletsPerShot; i++)
            {
                var bullet = _bullets.GetObjectFromPool();
                Vector3 dir = CalculateBulletDir(i);
                bullet.Initialize(_shotPoint.position, dir, _damage, _bulletLifeTime);
            }
            _agent.isStopped = false;
            TakeCover();
        }

        protected IEnumerator Shot()
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
            TakeCover();
        }

        protected Vector3 CalculateBulletDir(int number)
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

        protected void TakeCover()
        {
            if (Vector3.Distance(_transform.position, _target.position) < _changeCowerDistance &&
                _inCower && _moveToAlternativeCower == false) _moveToCover = false;
            
            if (_moveToCover) return;
            _inCower = false;
            _moveToCover = true;
            Vector3 cowerOffset = (_transform.position - _target.position).normalized * _distanceFromTarget;
            Cower cower = _cowerKeeper.GetNearestShelter(_target.position + cowerOffset);
            if (_curCower != cower)
            {
                NavMeshHit destination;
                NavMesh.SamplePosition(cower.GetCowerPoint(_target.position), out destination, 100, NavMesh.AllAreas);
                _agent.SetDestination(destination.position);
                _curCower = cower;
            }
            else
            {
                cower = _cowerKeeper.GetNearestShelter(_target.position - cowerOffset);
                NavMeshHit destination;
                NavMesh.SamplePosition(cower.GetCowerPoint(_target.position), out destination, 100, NavMesh.AllAreas);
                _agent.SetDestination(destination.position);
                _moveToAlternativeCower = true;
            }
        }

        protected void MoveToAttack()
        {
            NavMeshHit destination;
            NavMesh.SamplePosition(_target.position, out destination, 100, NavMesh.AllAreas);
            _agent.SetDestination(destination.position);
            _curCower = null;
        }

        public override void TakeDamage(int value)
        {
            base.TakeDamage(value);
            if (_inCower)
            {
                _moveToCover = false;
                _moveToAlternativeCower = false;
                _inCower = false;
            }
        }

#if UNITY_EDITOR
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _changeCowerDistance);
        }
#endif
    }
}
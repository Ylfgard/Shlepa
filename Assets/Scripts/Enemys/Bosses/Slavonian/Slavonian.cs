using UnityEngine;
using System.Collections.Generic;
using Enemys.Projectiles;
using UnityEngine.AI;
using System.Collections;

namespace Enemys.Bosses
{
    public class Slavonian : Boss
    {
        [SerializeField] private SlavonianStage[] _stageParameters;

        [Header("Shot Parameters")]
        [SerializeField] protected ProjectileType _projectileType;
        [SerializeField] protected int _bulletsPerShot;
        [SerializeField] protected bool _randomDir;
        [SerializeField] protected float _dispersion;
        [SerializeField] protected float _bulletDelay;
        [SerializeField] protected Transform _shotPoint;
        [SerializeField] protected float _bulletLifeTime;

        [Header("Charge")]
        [SerializeField] protected int _chargeDamage;
        [SerializeField] protected float _chargeDistance;
        [SerializeField] protected float _chargeTime;

        [Header("Distance")]
        [SerializeField] protected float _distance;
        [SerializeField] protected float _stopDistance;
        [SerializeField] protected float _step;

        protected Dictionary<int, SlavonianStage> _stages;
        protected ObjectPool<Bullet> _bullets;
        protected LayerMask _canBeCollided;
        protected LayerMask _canBeDamaged;
        protected float _bulletSpeed;

        protected float _chargeDelay;
        protected int _dir;
        protected bool _chargeUnlocked;
        protected float _inChargeTime;
        protected bool _chargeReady;
        protected bool _inCharge;
        protected float _chargeSpeed;
        protected float _chargeAceleration;
        protected Vector3 _chargeDir;
        protected Collider _collider;

        protected override void Awake()
        {
            base.Awake();
            _stages = new Dictionary<int, SlavonianStage>();
            foreach (var stage in _stageParameters)
            {
                if (_stages.ContainsKey(stage.Index)) Debug.LogError("Wrong stage index! " + stage.Index);
                else _stages.Add(stage.Index, stage);
            }
            _chargeReady = true;
            _collider = GetComponent<Collider>();
        }

        protected override void Start()
        {
            base.Start();
            _animationController.CallBack += MakeShot;
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
            _chargeAceleration = 2 * _chargeDistance / Mathf.Pow(_chargeTime, 2);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            
            if (_inCharge)
            {
                _inChargeTime += Time.fixedDeltaTime;
                if (_inChargeTime >= _chargeTime)
                {
                    _inCharge = false;
                    _collider.isTrigger = false;
                }
                else
                {
                    _chargeSpeed += _chargeAceleration * Time.fixedDeltaTime;
                    _transform.position += _chargeDir * _chargeSpeed * Time.fixedDeltaTime;
                }
                return;
            }
            else
            {
                if (_chargeUnlocked && _chargeReady)
                    Charge();
            }

            if (_isAttacking == false)
            {
                if (Vector3.Distance(_transform.position, _target.position) <= _attackDistance)
                    Attack();
                else
                    MoveToTarget();
            }
            else
            {
                MakeStep();
            }
        }

        public override void ActivateStage(int stageIndex)
        {
            SlavonianStage stage;
            if (_stages.TryGetValue(stageIndex, out stage))
            {
                _chargeUnlocked = stage.UnlockCharge;
                _chargeDelay = stage.ChargeDelay;
            }
        }

        protected void OnTriggerEnter(Collider other)
        {
            if (other.tag == TagsKeeper.Player)
                _player.Parameters.TakeDamage(_chargeDamage);
        }

        protected void Charge()
        {
            _collider.isTrigger = true;
            _chargeReady = false;
            _inCharge = true;
            _chargeDir = (_target.position - _transform.position).normalized;
            _chargeDir.y = 0;
            _chargeSpeed = 0;
            _inChargeTime = 0;
            Debug.Log("CHAAARGE!");
            StartCoroutine(ReloadCharge());
        }

        protected IEnumerator ReloadCharge()
        {
            yield return new WaitForSeconds(_chargeDelay);
            _chargeReady = true;
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
                    _dir = Random.Range(-1, 1);
                }
            }
            MoveToTarget();
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
            MakeStep();
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
            MakeStep();
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

        protected void MoveToTarget()
        {
            Vector3 step = (_target.position - _transform.position).normalized * _step;
            NavMeshHit destination;
            NavMesh.SamplePosition(_transform.position + step, out destination, 100, NavMesh.AllAreas);
            _agent.SetDestination(destination.position);
        }

        protected void MakeStep()
        {
            float curDis = Vector3.Distance(_transform.position, _target.position);

            if (Vector3.Distance(_transform.position, _agent.destination) > _stopDistance && curDis >= _distance) return;

            Vector3 dir;
            if (curDis < _distance)
                dir = _transform.position - _target.position;
            else
                dir = _target.position - _transform.position;
            Vector3 normal = Vector3.forward;
            Vector3.OrthoNormalize(ref dir, ref normal);
            if (_dir < 0)
                dir = dir * _step + normal * -_distance;
            else
                dir = dir * _step + normal * _distance;

            dir = dir.normalized * _step;
            NavMeshHit destination;
            NavMesh.SamplePosition(_transform.position + dir, out destination, 100, NavMesh.AllAreas);
            _agent.SetDestination(destination.position);
        }

#if UNITY_EDITOR
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _distance);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _chargeDistance);
        }
#endif
    }
}

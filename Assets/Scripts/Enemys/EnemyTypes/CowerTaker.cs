using UnityEngine;
using Enemys.Cowers;
using Enemys.Projectiles;
using System.Collections;

namespace Enemys
{
    public class CowerTaker : RangeEnemy
    {
        protected CowerKeeper _cowerKeeper;
        protected Cower _curCower;
        protected bool _moveToCover;

        protected override void Start()
        {
            base.Start();
            _cowerKeeper = CowerKeeper.Instance;
        }

        public override void Initialize(Vector3 position, bool active, ObjectPool<Bullet> bullets)
        {
            base.Initialize(position, active, bullets);
            _moveToCover = false;
        }

        protected override void FixedUpdate()
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
                else
                    MoveToAttack();
            }
            else
            {
                TakeCover();
            }
        }

        protected override void Attack()
        {
            RaycastHit hit;
            _moveToCover = false;
            if (Physics.Raycast(_shotPoint.position, _target.position - _shotPoint.position, out hit, _attackDistance, _canBeCollided))
            {
                if (Physics.OverlapSphere(hit.point, 0.01f, _canBeDamaged).Length > 0)
                {
                    _isAttacking = true;
                    _animationController.SetTrigger("Attack");
                    StartCoroutine(PrepareAttack());
                    _agent.isStopped = true;
                    return;
                }
            }
            MoveToAttack();
        }

        protected override void FastShot()
        {
            base.FastShot();
            _agent.isStopped = false;
            TakeCover();
        }

        protected override IEnumerator Shot()
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

        protected void TakeCover()
        {
            if (_moveToCover) return;
            _moveToCover = true;
            Cower cower = _cowerKeeper.GetNearestShelter(_transform.position);
            if (_curCower != cower)
            {
                _agent.SetDestination(cower.GetCowerPoint(_target.position));
                _curCower = cower;
            }
        }

        protected void MoveToAttack()
        {
            _agent.SetDestination(_target.position);
            _curCower = null;
        }
    }
}
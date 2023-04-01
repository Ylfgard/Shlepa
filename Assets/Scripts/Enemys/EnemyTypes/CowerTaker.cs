using UnityEngine;
using Enemys.Cowers;
using Enemys.Projectiles;
using System.Collections;
using UnityEngine.AI;

namespace Enemys
{
    public class CowerTaker : RangeEnemy
    {
        [Header ("Cower")]
        [SerializeField] protected float _distanceFromTarget;
        [SerializeField] protected float _changeCowerDistance;

        protected CowerKeeper _cowerKeeper;
        protected Cower _curCower;
        protected bool _moveToCover;

        protected override void Start()
        {
            base.Start();
            _cowerKeeper = CowerKeeper.Instance;
        }

        //public override void Initialize(Vector3 position, bool active, ObjectPool<Bullet> bullets)
        //{
        //    base.Initialize(position, active, bullets);
        //    _moveToCover = false;
        //}

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
            _moveToCover = false;
            base.Attack();
            MoveToAttack();
        }

        protected override void FastShot()
        {
            base.FastShot();
            TakeCover();
        }

        protected override IEnumerator Shot()
        {

            yield return base.Shot();
            TakeCover();
        }

        protected void TakeCover()
        {
            if (Vector3.Distance(_transform.position, _target.position) < _changeCowerDistance) _moveToCover = false;
            if (_moveToCover) return;
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
        }

        protected void MoveToAttack()
        {
            NavMeshHit destination;
            NavMesh.SamplePosition(_target.position, out destination, 100, NavMesh.AllAreas);
            _agent.SetDestination(destination.position);
            _curCower = null;
        }
    }
}
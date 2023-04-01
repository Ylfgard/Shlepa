using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Enemys
{
    public class DistanceKeepEnemy : RangeEnemy
    {
        [Header("Distance")]
        [SerializeField] protected float _distance;
        [SerializeField] protected float _step;

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
                    MoveToTarget();
            }
            else
            {
                MakeStep();
            }
        }

        protected override void Attack()
        {
            base.Attack();
            MoveToTarget();
        }

        protected override void FastShot()
        {
            base.FastShot();
            MakeStep();
        }

        protected override IEnumerator Shot()
        {
            yield return base.Shot();
            MakeStep();
        }

        protected void MoveToTarget()
        {
            Vector3 destination = (_target.position - _transform.position).normalized * _step;
            _agent.SetDestination(_transform.position + destination);
        }

        protected void MakeStep()
        {
            float curDis = Vector3.Distance(_transform.position, _target.position);

            if (Vector3.Distance(_transform.position, _agent.destination) > 1f && curDis >= _distance) return;

            Vector3 dir;
            if (curDis < _distance)
                dir = _transform.position - _target.position;
            else
                dir = _target.position - _transform.position;
            Vector3 normal = Vector3.forward;
            Vector3.OrthoNormalize(ref dir, ref normal);
            float x = Random.Range(-1, 1);
            if (x < 0)
                x = -_distance;
            else
                x = _distance;

            dir = dir * _step + normal * x;
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
        }
#endif
    }
}
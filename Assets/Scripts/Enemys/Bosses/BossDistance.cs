using UnityEngine;
using UnityEngine.AI;

namespace Enemys.Bosses
{
    public abstract class BossDistance : BossRange
    {
        [Header("Distance")]
        [SerializeField] protected float _distance;
        [SerializeField] protected float _stopDistance;
        [SerializeField] protected float _step;

        protected int _dir;

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

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
            dir.y = 0;
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
        }
#endif
    }
}
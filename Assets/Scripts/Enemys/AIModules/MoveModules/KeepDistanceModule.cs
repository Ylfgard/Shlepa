using UnityEngine;
using UnityEngine.AI;
using PlayerController;

namespace Enemys.AIModules
{
    public class KeepDistanceModule : MoveModule
    {
        [Header("Distance")]
        [SerializeField] protected float _distance;
        [SerializeField] protected float _stopDistance;
        [SerializeField] protected float _step;

        protected int _dir;

        public float Distance => _distance;

        public override void Initialize(Enemy enemy)
        {
            base.Initialize(enemy);
            ChangeDirection();
            enemy.SendAttack += ChangeDirection;
        }

        public void ChangeDirection()
        {
            _dir = Random.Range(-1, 1);
        }

        public float GetCurDistance()
        {
            float distance = Vector2.Distance(new Vector2(_transform.position.x, _transform.position.z),
                new Vector2(_target.position.x, _target.position.z));
            return distance;
        }

        public override void Move()
        {
            if (_agent.stoppingDistance != 0) _agent.stoppingDistance = 0;
            float curDis = Vector2.Distance(new Vector2(_transform.position.x, _transform.position.z),
                new Vector2(_target.position.x, _target.position.z));

            if (Vector2.Distance(new Vector2(_transform.position.x, _transform.position.z),
                new Vector2(_agent.destination.x, _agent.destination.z)) > _stopDistance && curDis >= _distance) return;

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

            CheckMove();
        }

        public override void MoveToTarget()
        {
            if (_agent.stoppingDistance != 1f) _agent.stoppingDistance = 1f;
            Vector3 step = (_target.position - _transform.position).normalized * _step;
            NavMeshHit destination;
            NavMesh.SamplePosition(_transform.position + step, out destination, 100, NavMesh.AllAreas);
            _agent.SetDestination(destination.position);
            
            CheckMove();
        }

#if UNITY_EDITOR
        protected void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _distance);
        }
#endif
    }
}
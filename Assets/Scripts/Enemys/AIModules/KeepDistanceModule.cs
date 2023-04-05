using UnityEngine;
using UnityEngine.AI;
using PlayerController;

namespace Enemys.AIModules
{
    public class KeepDistanceModule : MonoBehaviour
    {
        [Header("Distance")]
        [SerializeField] protected float _distance;
        [SerializeField] protected float _stopDistance;
        [SerializeField] protected float _step;

        protected int _dir;

        protected Transform _transform;
        protected Transform _target;
        protected NavMeshAgent _agent;

        public void Initialize(Transform myTransform, Player player, NavMeshAgent agent)
        {
            _transform = myTransform;
            _target = player.Mover.Transform;
            _agent = agent;
            ChangeDirection();
        }

        public void ChangeDirection()
        {
            _dir = Random.Range(-1, 1);
        }

        public void MakeStep()
        {
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
        }

#if UNITY_EDITOR
        protected void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _distance);
        }
#endif
    }
}
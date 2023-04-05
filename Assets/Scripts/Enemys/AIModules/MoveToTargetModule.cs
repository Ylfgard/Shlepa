using UnityEngine;
using UnityEngine.AI;
using PlayerController;

namespace Enemys.AIModules
{
    public class MoveToTargetModule : MonoBehaviour
    {
        protected NavMeshAgent _agent;
        protected Transform _target;
        protected Transform _transform;

        public virtual void Initialize(Transform myTransform, NavMeshAgent agent, Player player)
        {
            _transform = myTransform;
            _agent = agent;
            _target = player.Mover.Transform;
        }

        public void MoveToAttack(float distance)
        {
            Vector3 step = (_target.position - _transform.position).normalized * distance;
            NavMeshHit destination;
            NavMesh.SamplePosition(_transform.position + step, out destination, 100, NavMesh.AllAreas);
            _agent.SetDestination(destination.position);
        }

        public void MoveToAttack()
        {
            NavMeshHit destination;
            NavMesh.SamplePosition(_target.position, out destination, 100, NavMesh.AllAreas);
            _agent.SetDestination(destination.position);
        }
    }
}

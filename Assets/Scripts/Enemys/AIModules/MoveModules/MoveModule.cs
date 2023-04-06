using UnityEngine;
using UnityEngine.AI;

namespace Enemys.AIModules
{
    public class MoveModule : AIModule
    {
        [Header ("Parameters")]
        [SerializeField] protected float _speed;

        protected NavMeshAgent _agent;
        protected Transform _target;
        protected Transform _transform;

        public virtual void Initialize(Enemy enemy)
        {
            _transform = enemy.Transform;
            _target = enemy.Player.Mover.Transform;
            _agent = enemy.Agent;
            _agent.speed = _speed;
            _agent.angularSpeed = 360;
            _agent.acceleration = 100;
        }

        public virtual void Move()
        {
            MoveToTarget();
        }

        public virtual void MoveToTarget()
        {
            if (_agent.stoppingDistance != 1.5f) _agent.stoppingDistance = 1.5f;
            NavMeshHit destination;
            NavMesh.SamplePosition(_target.position, out destination, 100, NavMesh.AllAreas);
            _agent.SetDestination(destination.position);
        }
    }
}

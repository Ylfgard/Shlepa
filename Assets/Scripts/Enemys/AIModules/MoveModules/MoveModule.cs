using UnityEngine;
using UnityEngine.AI;

namespace Enemys.AIModules
{
    public class MoveModule : AIModule
    {
        [Header ("Parameters")]
        [SerializeField] protected float _speed;

        protected NavMeshAgent _agent;
        protected AnimationController _animationController;
        protected Transform _target;
        protected Transform _transform;

        public virtual void Initialize(Enemy enemy)
        {
            _transform = enemy.Transform;
            _target = enemy.Player.Mover.Transform;
            _agent = enemy.Agent;
            _animationController = enemy.AnimationController;
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
            if (_agent.stoppingDistance != 1f) _agent.stoppingDistance = 1f;
            NavMeshHit destination;
            NavMesh.SamplePosition(_target.position, out destination, 100, NavMesh.AllAreas);
            _agent.SetDestination(destination.position);
            CheckMove();
        }

        protected void CheckMove()
        {
            if (_agent.isStopped == false && Vector2.Distance(new Vector2(_agent.destination.x, _agent.destination.z),
                new Vector2(_transform.position.x, _transform.position.z)) > _agent.stoppingDistance)
            {
                _animationController.SetTrigger("Move");
            }
        }
    }
}

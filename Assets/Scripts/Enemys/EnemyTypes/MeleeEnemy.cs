using UnityEngine;

namespace Enemys
{
    public class MeleeEnemy : Enemy
    {

        protected override void Start()
        {
            base.Start();
            _animationController.CallBack += MakeDamage;
        }

        public override void Initialize(Vector3 position)
        {
            base.Initialize(position);
            Move();
        }

        protected void FixedUpdate()
        {
            if (_isAttacking == false)
            {
                if (Vector3.Distance(_transform.position, _target.position) <= _attackDistance)
                    Attack();
                else
                    Move();
            }
        }

        protected override void Attack()
        {
            base.Attack();
            _agent.isStopped = true;
        }

        protected virtual void Move()
        {
            _animationController.SetTrigger("Move");
            if (_agent.isStopped == true)
                _agent.isStopped = false;

            if (_agent.destination != _target.position)
                _agent.SetDestination(_target.position);
        }

        protected virtual void MakeDamage()
        {
            _player.Parameters.TakeDamage(_damage);
            _agent.isStopped = false;
        }
    }
}
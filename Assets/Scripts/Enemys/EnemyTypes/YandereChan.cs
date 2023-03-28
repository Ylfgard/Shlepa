using UnityEngine;

namespace Enemys
{
    public class YandereChan : Enemy
    {
        protected Transform _target;

        protected override void Start()
        {
            base.Start();
            _target = _player.Mover.Transform;
            _animationController.CallBack += MakeDamage;
            Initialize(true);
        }

        public override void Initialize(bool active)
        {
            base.Initialize(active);
            _isActive = active;
            if (active) Move();

        }

        private void FixedUpdate()
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
                    Move();
            }
        }

        protected override void Attack()
        {
            base.Attack();
            _agent.isStopped = true;
        }

        protected void Move()
        {
            _animationController.SetTrigger("Move");
            if (_agent.isStopped == true)
                _agent.isStopped = false;

            if (_agent.destination != _target.position)
                _agent.SetDestination(_target.position);
        }

        private void MakeDamage()
        {
            _player.Parameters.TakeDamage(_damage);
            _agent.isStopped = true;
        }
    }
}
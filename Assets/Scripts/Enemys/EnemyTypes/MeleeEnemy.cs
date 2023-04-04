using UnityEngine;

namespace Enemys
{
    public class MeleeEnemy : Enemy
    {
        protected const float _spawnHight = 10;
        protected const float _fallSpeed = 20;

        [Header("Spawn Parameters")]
        [SerializeField] protected LayerMask _groundLayer;
        [SerializeField] protected float _landingAreaRadius;
        [SerializeField] protected int _landingDamage;

        protected bool _isLanding;

        protected override void Start()
        {
            base.Start();
            _animationController.CallBack += MakeDamage;
        }

        public override void Initialize(Vector3 position)
        {
            position.y += _spawnHight;
            base.Initialize(position);
            _isLanding = true;
            _agent.enabled = false;
            Move();
        }

        protected virtual void FixedUpdate()
        {
            if (_isLanding)
            {
                CheckLanding();
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

        public override float LandingAreaRadius()
        {
            return _landingAreaRadius;
        }

        protected void CheckLanding()
        {
            if (Physics.Raycast(_transform.position, Vector3.down, 2f, _groundLayer))
            {
                Landing();
            }
            else
            {
                Vector3 newPos = _transform.position;
                newPos.y -= _fallSpeed * Time.fixedDeltaTime;
                _transform.position = newPos;
            }
        }

        protected void Landing()
        {
            if (Vector3.Distance(_transform.position, _target.position) <= _landingAreaRadius)
                _player.Parameters.TakeDamage(_landingDamage);
            _isLanding = false;
            _agent.enabled = true;
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
#if UNITY_EDITOR
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            Gizmos.DrawWireSphere(transform.position, _landingAreaRadius);
        }
#endif
    }
}
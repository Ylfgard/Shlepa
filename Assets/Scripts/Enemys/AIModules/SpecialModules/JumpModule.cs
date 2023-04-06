using UnityEngine;
using System.Collections;
using PlayerController;
using Enemys.Bosses;

namespace Enemys.AIModules
{
    public class JumpModule : AIModule
    {
        [Header("Jump parameters")]
        [SerializeField] protected int _damage;
        [SerializeField] protected float _attackDelay;
        [SerializeField] protected int _landingAreaRadius;
        [SerializeField] protected float _jumpTime;
        [SerializeField] protected float _jumpHight;
        [SerializeField] protected Transform _groundCheck;
        [SerializeField] protected LayerMask _ground;

        protected CharacterController _controller;
        protected bool _isGrounded;
        protected float _curVerSpeed;
        protected float _gravity;
        protected float _maxFallSpeed;
        protected Vector3 _dir;
        protected bool _jumpStarted;
        protected float _speed;
        protected bool _isAttacking;

        protected Transform _transform;
        protected Transform _target;
        protected Player _player;
        protected AnimationController _animationController;

        public void Initialize(Enemy enemy, CharacterController controller)
        {
            _transform = enemy.Transform;
            _player = enemy.Player;
            _target = _player.Mover.Transform;
            _controller = controller;
            _animationController = enemy.AnimationController;

            _animationController.CallBack += Jump;
            _curVerSpeed = 0;
            _dir = Vector3.zero;
        }

        protected void Move()
        {
            Vector3 step = _dir * _speed * Time.fixedDeltaTime;

            if (_curVerSpeed > _maxFallSpeed)
                _curVerSpeed += _gravity * Time.fixedDeltaTime;
            else
                _curVerSpeed = _maxFallSpeed;

            step.y = _curVerSpeed * Time.fixedDeltaTime;
            _controller.Move(step);
        }

        public void TryJump()
        {
            if (_jumpStarted == false && _isGrounded == false)
            {
                if (Physics.OverlapSphere(_groundCheck.position, 0.25f, _ground).Length > 0)
                {
                    _isGrounded = true;
                    Landing();
                    _curVerSpeed = 0;
                    _dir = Vector3.zero;
                }
            }

            Move();

            if (_isAttacking == false && _isGrounded)
                Attack();
        }

        protected void Attack()
        {
            _isAttacking = true;
            _animationController.SetTrigger("Attack");
            StartCoroutine(PrepareAttack());
        }

        protected IEnumerator PrepareAttack()
        {
            yield return new WaitForSeconds(_attackDelay);
            _isAttacking = false;
        }

        protected void Jump()
        {
            _dir = ((_target.forward * (_landingAreaRadius - 1)) + _player.Mover.GetFuturePos(_jumpTime)) - _transform.position;
            _speed = _dir.magnitude / _jumpTime;
            float t = _jumpTime / 2;
            _gravity = (-2 * _jumpHight) / Mathf.Pow(t, 2);
            _curVerSpeed = -_gravity * t;
            _maxFallSpeed = -_curVerSpeed * 2;
            _dir.y = 0;
            _dir = _dir.normalized;
            _isGrounded = false;
            StartCoroutine(JumpCooldown());
        }

        protected void Landing()
        {
            if (Vector3.Distance(_target.position, _transform.position) <= _landingAreaRadius)
                _player.Parameters.TakeDamage(_damage);
        }

        protected IEnumerator JumpCooldown()
        {
            _jumpStarted = true;
            yield return new WaitForSeconds(0.1f);
            _jumpStarted = false;
        }

        public void ChangeParameters(BoobaStage stage)
        {
            _attackDelay = stage.AttackDelay;
            _jumpTime = stage.JumpTime;
        }

#if UNITY_EDITOR
        protected void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _landingAreaRadius);
        }
#endif
    }
}
using Enemys.Bosses;
using PlayerController;
using System.Collections;
using UnityEngine;

namespace Enemys.AIModules
{
    public class GroundSpikesModule : AIModule
    {
        [Header("Jump parameters")]
        [SerializeField] protected int _damage;
        [SerializeField] protected float _attackDelay;
        [SerializeField] protected float _jumpTime;
        [SerializeField] protected float _activeTime;
        [SerializeField] protected float _jumpHight;
        [SerializeField] protected Transform _groundCheck;
        [SerializeField] protected LayerMask _ground;

        protected Transform _transform;
        protected Player _player;
        protected CharacterController _controller;
        protected AnimationController _animationController;
        protected Spikes _spikes;

        protected bool _readyToJump;
        protected bool _jumpStarted;
        protected bool _isGrounded;
        protected float _gravity;
        protected float _maxFallSpeed;
        protected float _fallSpeed;
        protected Vector3 _startPos;
        protected bool _startLanding;
        protected bool _firstLanding;
        protected bool _spikesActive;

        public void Initialize(Enemy enemy, CharacterController controller)
        {
            _transform = enemy.Transform;
            _player = enemy.Player;
            _controller = controller;
            
            _spikes = FindObjectOfType<Spikes>();
            if (_spikes == null)
                Debug.LogError("Can't find spikes!");
            _animationController = _spikes.Controller;

            _readyToJump = true;
            _fallSpeed = 0;
            _gravity = -20;
            _maxFallSpeed = -40;
        }

        public void Activate(Vector3 position)
        {
            position += Vector3.up * 5;
            _controller.Move(position);
            _transform.position = position;
            _startPos = position;
            _startLanding = true;
            _firstLanding = true;
        }

        public void Move()
        {
            if (_jumpStarted == false && _isGrounded == false)
            {
                if (Physics.OverlapSphere(_groundCheck.position, 0.25f, _ground).Length > 0)
                {
                    _isGrounded = true;
                    Landing();
                    _fallSpeed = 0;
                }
            }

            Vector3 step = Vector3.up * _fallSpeed * Time.fixedDeltaTime;

            if (_fallSpeed > _maxFallSpeed)
                _fallSpeed += _gravity * Time.fixedDeltaTime;
            else
                _fallSpeed = _maxFallSpeed;

            if (_startLanding)
            {
                _transform.position = _startPos;
                _startLanding = false;
            }
            else
            {
                _controller.Move(step);
            }
        }
        
        public void TryJump()
        {
            if (_readyToJump)
                Jump();
        }

        protected void Landing()
        {
            if (_firstLanding)
            {
                _firstLanding = false;
                return;
            }

            _animationController.SetTrigger("Spikes");
            if (_player.Mover.Grounded)
            {
                _player.Parameters.TakeDamage(_damage);
            }   
            else
            {
                _player.Mover.Landed += MakeDamage;
                _spikesActive = true;
                StartCoroutine(DeactivateSpikes());
            }
            StartCoroutine(PrepareAttack());
        }

        protected void MakeDamage()
        {
            _player.Parameters.TakeDamage(_damage);
            _player.Mover.Landed -= MakeDamage;
            _spikesActive = false;
        }

        protected IEnumerator DeactivateSpikes()
        {
            yield return new WaitForSeconds(_activeTime);
            if (_spikesActive)
            {
                _spikesActive = false;
                _player.Mover.Landed -= MakeDamage;
            }
        }

        protected void Jump()
        {
            float t = _jumpTime / 2;
            _gravity = (-2 * _jumpHight) / Mathf.Pow(t, 2);
            _fallSpeed = -_gravity * t;
            _maxFallSpeed = -_fallSpeed * 2;
            _isGrounded = false;
            _readyToJump = false;
            StartCoroutine(JumpCooldown());
        }

        protected IEnumerator JumpCooldown()
        {
            _jumpStarted = true;
            yield return new WaitForSeconds(_jumpTime / 2);
            _jumpStarted = false;
        }

        protected IEnumerator PrepareAttack()
        {
            yield return new WaitForSeconds(_attackDelay);
            _readyToJump = true;
        }

        public void ChangeParameters(BoobaStage stage)
        {
            _attackDelay = stage.AttackDelay;
            _jumpTime = stage.JumpTime;
        }

        public override void Deactivate(Enemy enemy)
        {
            base.Deactivate(enemy);

        }
    }
}
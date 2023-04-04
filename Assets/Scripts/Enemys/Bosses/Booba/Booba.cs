using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Enemys.Bosses
{
    public class Booba : Boss
    {
        [SerializeField] private BoobaStage[] _stageParameters;

        [Header("Jump parameters")]
        [SerializeField] private int _landingAreaRadius;
        [SerializeField] private float _jumpTime;
        [SerializeField] private float _jumpHight;
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private LayerMask _ground;

        private CharacterController _controller;
        private bool _isGrounded;
        private float _curVerSpeed;
        private float _gravity;
        private float _maxFallSpeed;
        private Vector3 _dir;
        private bool _jumpStarted;
        private Dictionary<int, BoobaStage> _stages;

        protected override void Awake()
        {
            _transform = transform;
            _controller = GetComponent<CharacterController>();
            foreach (WeakPoint weakPoint in _weakPoints)
                weakPoint.Initialize(this);

            foreach (var stage in _stageTriggers)
                stage.Initialize(this);

            _stages = new Dictionary<int, BoobaStage>();
            foreach (var stage in _stageParameters)
            {
                if (_stages.ContainsKey(stage.Index)) Debug.LogError("Wrong stage index! " + stage.Index);
                else _stages.Add(stage.Index, stage);
            }
        }

        protected override void Start()
        {
            base.Start();
            _animationController.CallBack += Jump;
        }

        public override void Initialize(Vector3 position)
        {
            base.Initialize(position);
            _curVerSpeed = 0;
            _dir = Vector3.zero;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

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

        private void Move()
        {
            Vector3 step = _dir * _speed * Time.fixedDeltaTime;

            if (_curVerSpeed > _maxFallSpeed)
                _curVerSpeed += _gravity * Time.fixedDeltaTime;
            else
                _curVerSpeed = _maxFallSpeed;

            step.y = _curVerSpeed * Time.fixedDeltaTime;
            _controller.Move(step);
        }

        public override void ActivateStage(int stageIndex)
        {
            BoobaStage stage;
            if (_stages.TryGetValue(stageIndex, out stage))
            {
                _attackDelay = stage.AttackDelay;
                _jumpTime = stage.JumpTime;
            }
        }

        private void Jump()
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

        private void Landing()
        {
            if (Vector3.Distance(_target.position, _transform.position) <= _landingAreaRadius)
                _player.Parameters.TakeDamage(_damage);
        }

        private IEnumerator JumpCooldown()
        {
            _jumpStarted = true;
            yield return new WaitForSeconds(0.1f);
            _jumpStarted = false;
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
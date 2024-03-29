using System;
using UnityEngine;

namespace PlayerController
{
    public class PlayerMovement : MonoBehaviour
    {
        private const float JumpCooldown = 0.1f;

        [Header("Moving")]
        [SerializeField] private float _speed;

        [Header("Jumping")]
        [SerializeField] private Transform _groundCheck;
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private float _jumpHight;
        [SerializeField] private float _jumpTime;
        [SerializeField] private float _maxFallSpeed;

        private Transform _transform;
        private CharacterController _controller;
        private float _rotateX;
        private float _ySpeed;
        private float _jumpStartSpeed;
        private float _gravity;
        private bool _grounded;
        private bool _readyToJump;
        private Vector3 _curDir;

        public Transform Transform => _transform;
        public bool Grounded => _grounded;

        private void Awake()
        {
            _transform = transform;
            _controller = GetComponent<CharacterController>();
            _readyToJump = true;

            float t = _jumpTime / 2;
            _gravity = (-2 * _jumpHight) / Mathf.Pow(t, 2);
            _jumpStartSpeed = -_gravity * t;
        }

        private void FixedUpdate()
        {
            _grounded = Physics.OverlapSphere(_groundCheck.position, 0.25f, _groundMask).Length > 0;

            if (_ySpeed > _maxFallSpeed)
                _ySpeed += _gravity * Time.fixedDeltaTime;
            else
                _ySpeed = _maxFallSpeed;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Platform")
            {
                transform.parent = other.transform;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Platform")
            {
                transform.parent = null;
            }
        }
        
        public void Move(Vector3 dir)
        {
            dir = _transform.right * dir.x + _transform.forward * dir.z + Vector3.up * dir.y;
            dir = dir * _speed;

            if (dir.y > 0)
            {
                if (_grounded && _readyToJump)
                {
                    _readyToJump = false;
                    Invoke("ReadyJump", JumpCooldown);
                    _ySpeed = _jumpStartSpeed;
                }
            }

            _curDir = new Vector3(dir.x, 0, dir.z).normalized;
            dir.y = _ySpeed * Time.deltaTime;
            _controller.Move(dir);
        }

        public Vector3 GetFuturePos(float timeStep)
        {
            return _transform.position + (_curDir * _speed * timeStep);
        }

        public void SetPosition(Vector3 pos)
        {
            _transform.position = pos;
        }

        public void RotateX(float angle)
        {
            _rotateX += angle;
            _transform.rotation = Quaternion.Euler(0, _rotateX, 0);
        }

        private void ReadyJump()
        {
            _readyToJump = true;
        }
    }
}
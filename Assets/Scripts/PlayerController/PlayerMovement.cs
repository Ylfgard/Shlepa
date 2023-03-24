using UnityEngine;

namespace PlayerController
{
    public class PlayerMovement : MonoBehaviour
    {
        private const float JumpCooldown = 0.1f;

        [Header("Movement")]
        [SerializeField] private float _speed;

        [Header("Jumping")]
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private float _jumpHight;
        [SerializeField] private float _gravity;
        [SerializeField] private float _maxFallSpeed;

        private Transform _transform;
        private CharacterController _controller;
        private float _rotateX;
        private float _ySpeed;
        private bool _grounded;
        private bool _readyToJump;

        private void Awake()
        {
            _transform = transform;
            _controller = GetComponent<CharacterController>();
            _readyToJump = true;
        }

        private void Update()
        {
            _grounded = Physics.Raycast(_transform.position, Vector3.down, 1.25f, _groundMask);

            if (_ySpeed > _maxFallSpeed)
                _ySpeed += _gravity * Time.deltaTime;
            else
                _ySpeed = _maxFallSpeed;
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
                    _ySpeed = _jumpHight;
                }
            }

            dir.y = _ySpeed;
            _controller.Move(dir);
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
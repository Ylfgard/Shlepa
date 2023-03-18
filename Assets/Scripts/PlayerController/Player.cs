using UnityEngine;

namespace PlayerController
{
    public class Player : MonoBehaviour
    {
        private const float JumpCooldown = 0.1f;

        [Header ("Movement")]
        [SerializeField] private Transform _orientation;
        [SerializeField] private float _speed;

        [Header("Jumping")]
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private float _jumpForce;
        

        private static Player _instance;

        private Transform _transform;
        private Rigidbody _rigidbody;
        private float _rotateX;
        private bool _grounded;
        private bool _readyToJump;

        public static Player Instance => _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            else
                _instance = this;

            _transform = transform;
            _rigidbody = GetComponent<Rigidbody>();
            _readyToJump = true;
        }

        private void Update()
        {
            _grounded = Physics.Raycast(_orientation.position, Vector3.down, 1.25f, _groundMask);
        }

        public void Move(Vector3 dir)
        {
            dir = _orientation.right * dir.x + _orientation.forward * dir.z;
            dir = _transform.position + dir * _speed;
            _rigidbody.MovePosition(dir);
        }

        public void RotateX(float angle)
        {
            _rotateX += angle;
            _orientation.rotation = Quaternion.Euler(0, _rotateX, 0);
        }

        public void Jump()
        {
            if (_grounded == false || _readyToJump == false) return;

            _readyToJump = false;
            Invoke("ReadyJump", JumpCooldown);
            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }

        private void ReadyJump()
        {
            _readyToJump = true;
        }
    }
}
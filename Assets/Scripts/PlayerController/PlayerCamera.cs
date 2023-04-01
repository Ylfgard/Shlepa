using UnityEngine;
using UnityEngine.UI;

namespace  PlayerController
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private Image _aim;
        [SerializeField] private Image _sniperAim;
        [SerializeField] private Image _weaponSpR;
        [SerializeField] private Transform _origin;
        [SerializeField] private float _minYAngle, _maxYAngle;
        [SerializeField] private float _tiltHeadAngle;
        [SerializeField] private float _tiltHeadSpeed;

        private Transform _transform;
        private Camera _camera;
        private float _rotationX,_rotationY;
        private float _defualtFieldOfView;
        private float _headTargetAngle;
        private float _headCurAngle;

        public Transform Transform => _transform;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _transform = transform;
            _camera = Camera.main;
            _defualtFieldOfView = _camera.fieldOfView;
            _sniperAim.enabled = false;
            _headTargetAngle = 0;
            _headCurAngle = 0;
        }

        private void Update()
        {
            _transform.position = _origin.position;
        }

        private void FixedUpdate()
        {
            if (Mathf.Abs(_headTargetAngle - _headCurAngle) > 0.1f)
            {
                _headCurAngle += _tiltHeadSpeed * Time.fixedDeltaTime * Mathf.Sign(_headTargetAngle - _headCurAngle);
                _transform.rotation = Quaternion.Euler(_rotationX, _rotationY, _headCurAngle);
            }
            else
            {
                _transform.rotation = Quaternion.Euler(_rotationX, _rotationY, _headTargetAngle);
                _headCurAngle = _headTargetAngle;
            }
        }

        public void Rotate(float angleX, float angleY)
        {
            _rotationY += angleX;
            _rotationX -= angleY;
            _rotationX = Mathf.Clamp(_rotationX, _minYAngle, _maxYAngle);
            _transform.rotation = Quaternion.Euler(_rotationX, _rotationY, _headCurAngle);
        }

        public void TiltHead(float dirX, float dirZ)
        {
            if (dirX == 0 || dirZ != 0)
                _headTargetAngle = 0;
            else if (dirX < 0)
                _headTargetAngle = _tiltHeadAngle;
            else
                _headTargetAngle = -_tiltHeadAngle;
        }

        public void Zoom(float value, bool state)
        {
            if (value <= 1) return;
            if (state)
                _camera.fieldOfView = _defualtFieldOfView / value;
            else
                _camera.fieldOfView = _defualtFieldOfView;

            _sniperAim.enabled = state;
            _aim.enabled = !state;
            _weaponSpR.enabled = !state;
        }
    }
}
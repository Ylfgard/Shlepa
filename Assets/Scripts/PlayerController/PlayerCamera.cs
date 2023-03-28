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

        private Transform _transform;
        private Camera _camera;
        private float _rotationX,_rotationY;
        private float _defualtFieldOfView;

        public Transform Transform => _transform;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _transform = transform;
            _camera = Camera.main;
            _defualtFieldOfView = _camera.fieldOfView;
            _sniperAim.enabled = false;
        }

        private void Update()
        {
            _transform.position = _origin.position;
        }

        public void Rotate(float angleX, float angleY)
        {
            _rotationY += angleX;
            _rotationX -= angleY;
            _rotationX = Mathf.Clamp(_rotationX, _minYAngle, _maxYAngle);
            _transform.rotation = Quaternion.Euler(_rotationX, _rotationY, _transform.rotation.z);
        }

        public void TiltHead(float dirX, float dirZ)
        {
            if (dirX == 0 || dirZ != 0)
                _transform.rotation = Quaternion.Euler(_rotationX, _rotationY, 0);
            else if (dirX < 0)
                _transform.rotation = Quaternion.Euler(_rotationX, _rotationY, _tiltHeadAngle);
            else
                _transform.rotation = Quaternion.Euler(_rotationX, _rotationY, -_tiltHeadAngle);
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
using UnityEngine;

namespace  PlayerController
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private Transform _origin;
        [SerializeField] private float _minYAngle, _maxYAngle;
        [SerializeField] private float _tiltHeadAngle;

        private static PlayerCamera _instance;

        private Transform _transform;
        private float _rotationX,_rotationY;

        public static PlayerCamera Instance => _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            else
                _instance = this;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _transform = transform;
            _rotationY = 0;
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
    }
}
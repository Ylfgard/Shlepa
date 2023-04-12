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

        [Header ("Shake")]
        [SerializeField] private float _startShakeSpeed;
        [SerializeField] private float _shakeTime;

        private Transform _transform;
        private Camera _camera;
        private float _rotationX, _rotationY, _rotationZ;
        private float _defualtFieldOfView;
        private float _headTargetAngleZ;
        private float _headCurAngleZ;

        private float _curShakeSpeed;
        private bool _isShaking;
        private float _headCurAngleX;
        private float _shakeYOffset;
        private float _shakeCurTime;
        private float _shakeDrag;

        public Transform Transform => _transform;

        private void Awake()
        {
            _transform = transform;
            _camera = Camera.main;
            _defualtFieldOfView = _camera.fieldOfView;
            _sniperAim.enabled = false;
            _headTargetAngleZ = 0;
            _headCurAngleZ = 0;
        }

        private void Update()
        {
            if (_isShaking)
            {
                Vector3 pos = _origin.position;
                Vector3 offset = _origin.forward * _headCurAngleX;
                if (offset.magnitude > 0.5f) offset = offset.normalized * 0.5f;
                pos += offset;
                _transform.position = pos;
            }
            else
            {
                _transform.position = _origin.position;
            }
        }

        private void FixedUpdate()
        {
            if (_isShaking)
            {
                _shakeCurTime -= Time.fixedDeltaTime;
                float shakeStep = _curShakeSpeed * Time.fixedDeltaTime;
                _headCurAngleX -= shakeStep;
                if (_shakeCurTime <= 0 || _headCurAngleX >= 0)
                {
                    StopShaking();
                }
                else
                {
                    
                    _curShakeSpeed -= _shakeDrag * Time.fixedDeltaTime;
                }
            }

            float step = _tiltHeadSpeed * Time.fixedDeltaTime * Mathf.Sign(_headTargetAngleZ - _headCurAngleZ);
            if (Mathf.Abs(_headTargetAngleZ - _headCurAngleZ) > Mathf.Abs(step))
                _headCurAngleZ += step;
            else
                _headCurAngleZ = _headTargetAngleZ;
            _rotationZ = _headCurAngleZ;
        }

        public void StartShake(float angle)
        {
            _isShaking = true;
            _shakeYOffset = Random.Range(-0.1f, 0.1f);
            _shakeCurTime = _shakeTime;
            _curShakeSpeed = _startShakeSpeed * angle;
            _shakeDrag = ((4 * _curShakeSpeed * _shakeTime) - (8 * angle)) / Mathf.Pow(_shakeTime, 2);
        }

        private void StopShaking()
        {
            _isShaking = false;
            _rotationX += _headCurAngleX;
            _rotationY += _headCurAngleX * _shakeYOffset;
            _headCurAngleX = 0;
        }

        public void Rotate(float angleX, float angleY)
        {
            _rotationY += angleX;
            _rotationX -= angleY;
            _rotationX = Mathf.Clamp(_rotationX, _minYAngle, _maxYAngle);
            if (_isShaking)
                _transform.rotation = Quaternion.Euler(_rotationX + _headCurAngleX, _rotationY + (_headCurAngleX * _shakeYOffset), _rotationZ);
            else
                _transform.rotation = Quaternion.Euler(_rotationX, _rotationY, _rotationZ); 
        }

        public void TiltHead(float dirX, float dirZ)
        {
            if (dirX > 0)
            {
                if (dirZ == 0)
                    _headTargetAngleZ = _tiltHeadAngle;
                else
                    _headTargetAngleZ = _tiltHeadAngle / 2;
                return;
            }

            if (dirX < 0)
            {
                if (dirZ == 0)
                    _headTargetAngleZ = -_tiltHeadAngle;
                else
                    _headTargetAngleZ = -_tiltHeadAngle / 2;
                return;
            }

            _headTargetAngleZ = 0;
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
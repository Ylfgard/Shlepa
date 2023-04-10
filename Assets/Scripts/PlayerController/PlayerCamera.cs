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
        [SerializeField] private float _minShakeSpeed;
        [SerializeField] private float _shakeStep;
        [SerializeField] private float _shakeRadius;

        private Transform _transform;
        private Camera _camera;
        private float _rotationX,_rotationY;
        private float _defualtFieldOfView;
        private float _headTargetAngle;
        private float _headCurAngle;

        private float _curShakeSpeed;
        private float _shakeDrag;
        private float _curShakeTime;
        private Vector2 _shakeDir;
        private float _shakeDirAngle;
        private bool _isShaking;
        private float _shakeProgress;
        private float _progressDir;

        public Transform Transform => _transform;

        private void Awake()
        {
            _transform = transform;
            _camera = Camera.main;
            _defualtFieldOfView = _camera.fieldOfView;
            _sniperAim.enabled = false;
            _headTargetAngle = 0;
            _headCurAngle = 0;
            _progressDir = 0.5f;
        }

        private void Update()
        {
            if (_isShaking)
            {
                if (_curShakeTime > 0)
                {
                    _curShakeTime -= Time.deltaTime;

                    Vector3 trgPos = _origin.position;
                    trgPos += _origin.right * _shakeDir.x;
                    trgPos += _origin.up * _shakeDir.y;

                    _transform.position = Vector3.Lerp(_origin.position, trgPos, _shakeProgress);
                    _shakeProgress += _progressDir;
                    if (_shakeProgress >= 1)
                    {
                        _progressDir = -0.5f;
                    }
                    else if (_shakeProgress <= 0)
                    {
                        _progressDir = 0.5f;
                        _shakeDir *= -0.9f;
                    }
                }
                else
                {
                    StopShaking();
                }
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
                _curShakeSpeed -= _shakeDrag * Time.fixedDeltaTime;
                if (_curShakeSpeed < _minShakeSpeed)
                {
                    StopShaking();
                    return;
                }
                    float step = _curShakeSpeed * Time.fixedDeltaTime * Mathf.Sign(_shakeDirAngle - _headCurAngle);
                if (Mathf.Abs(_shakeDirAngle - _headCurAngle) > step)
                {
                    _headCurAngle += step;
                    _transform.rotation = Quaternion.Euler(_rotationX, _rotationY, _headCurAngle);
                }
                else
                {
                    _transform.rotation = Quaternion.Euler(_rotationX, _rotationY, _shakeDirAngle);
                    MakeShakeStep();
                }
            }
            else
            {
                float step = _tiltHeadSpeed * Time.fixedDeltaTime * Mathf.Sign(_headTargetAngle - _headCurAngle);
                if (Mathf.Abs(_headTargetAngle - _headCurAngle) > Mathf.Abs(step))
                {
                    _headCurAngle += step;
                    _transform.rotation = Quaternion.Euler(_rotationX, _rotationY, _headCurAngle);
                }
                else
                {
                    _transform.rotation = Quaternion.Euler(_rotationX, _rotationY, _headTargetAngle);
                    _headCurAngle = _headTargetAngle;
                }
            }
        }

        public void StartShake(float angle)
        {
            float radius = _shakeRadius * angle;
            float x = Random.Range(radius, -radius);
            float y = Mathf.Sqrt(Mathf.Pow(radius, 2) - Mathf.Pow(x, 2));
            _shakeDir = new Vector2(x, y);
            _isShaking = true;
            _shakeDirAngle = angle;
            _curShakeTime = _shakeTime;
            _curShakeSpeed = _startShakeSpeed * angle;
            _shakeDrag = _curShakeSpeed / _shakeTime;
        }

        private void MakeShakeStep()
        {
            if (Mathf.Abs(_shakeDirAngle) > _shakeStep && Mathf.Abs(_shakeDirAngle) >= 1)
                _shakeDirAngle = (Mathf.Abs(_shakeDirAngle) - _shakeStep) * -Mathf.Sign(_shakeDirAngle);
            else
                StopShaking();
        }

        private void StopShaking()
        {
            _transform.rotation = Quaternion.Euler(_rotationX, _rotationY, 0);
            _curShakeTime = 0;
            _shakeProgress = 0;
            _shakeDir = Vector2.zero;
            _isShaking = false;
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
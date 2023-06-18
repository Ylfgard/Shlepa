using UnityEngine;

namespace LevelMechanics.ActivatableObjects
{
    public class ClosingDoor : ActivatableObject
    {
        [SerializeField] private Transform _stopPoint;
        [SerializeField] private float _time;

        private Transform _transform;
        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private float _curTime;
        private bool _inMovement;

        private void Start()
        {
            _transform = transform;
            _startPosition = _transform.position;
            _curTime = 0;
            _inMovement = false;
        }

        public override void Activate()
        {
            _targetPosition = _stopPoint.position;
            _inMovement = true;
        }

        public override void Deactivate()
        {
            _targetPosition = _startPosition;
            _inMovement = true;
        }

        private void FixedUpdate()
        {
            if (_inMovement == false) return;

            _curTime += Time.fixedDeltaTime;
            if (_curTime >= _time)
            {
                _curTime = _time;
                _inMovement = false;
            }
            _transform.position = Vector3.Lerp(_transform.position, _targetPosition, _curTime / _time);
        }
    }
}
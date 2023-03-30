using UnityEngine;

namespace Enemys.Cowers
{
    public class Cower : MonoBehaviour
    {
        private Transform _transform;
        private Collider _collider;

        private Vector3 _leftBackwardCorner;
        private Vector3 _rightBackwardCorner;
        private Vector3 _leftForwardCorner;
        private Vector3 _rightForwardCorner;

        private Vector3 _leftSide;
        private Vector3 _rightSide;
        private Vector3 _backSide;
        private Vector3 _frontSide;

        public Transform Transform => _transform;

        private void Awake()
        {
            _transform = transform;
            _collider = GetComponent<Collider>();

            _leftBackwardCorner = _collider.bounds.min;
            _leftBackwardCorner.y = _transform.position.y;
            _rightForwardCorner = _collider.bounds.max;
            _rightForwardCorner.y = _transform.position.y;
            _leftForwardCorner = new Vector3(_leftBackwardCorner.x, _transform.position.y, _rightForwardCorner.z);
            _rightBackwardCorner = new Vector3(_rightForwardCorner.x, _transform.position.y, _leftBackwardCorner.z);

            _leftSide = Vector3.Lerp(_leftBackwardCorner, _leftForwardCorner, 0.5f);
            _rightSide = Vector3.Lerp(_rightBackwardCorner, _rightForwardCorner, 0.5f);
            _backSide = Vector3.Lerp(_leftBackwardCorner, _rightBackwardCorner, 0.5f);
            _frontSide = Vector3.Lerp(_leftForwardCorner, _rightForwardCorner, 0.5f);
        }

        public Vector3 GetCowerPoint(Vector3 position)
        { 
            Vector3 result = _transform.position + (_transform.position - position).normalized * 10;
            return GetSide(result);
        }

        private Vector3 GetSide(Vector3 position)
        {
            Vector3 side = _leftSide;
            float distanceNew = Vector3.Distance(position, _rightSide);
            if (distanceNew < Vector3.Distance(position, side))
                side = _rightSide;
            distanceNew = Vector3.Distance(position, _frontSide);
            if (distanceNew < Vector3.Distance(position, side))
                side = _frontSide;
            distanceNew = Vector3.Distance(position, _backSide);
            if (distanceNew < Vector3.Distance(position, side))
                side = _backSide;

            return side;
        }
    }
}
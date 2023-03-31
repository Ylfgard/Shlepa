using UnityEngine;

namespace Enemys.Cowers
{
    public class Cower : MonoBehaviour
    {
        [SerializeField] private Transform[] _sides;
        
        private Transform _transform;

        public Transform Transform => _transform;

        private void Awake()
        {
            _transform = transform;
        }

        public Vector3 GetCowerPoint(Vector3 position)
        { 
            Vector3 result = _transform.position + (_transform.position - position).normalized * 10;
            return GetSide(result);
        }

        private Vector3 GetSide(Vector3 position)
        {
            Vector3 result = _sides[0].position;
            foreach (Transform side in _sides)
            {
                if (Vector3.Distance(position, side.position) < (Vector3.Distance(position, result)))
                    result = side.position;
            }
            return result;
        }
    }
}
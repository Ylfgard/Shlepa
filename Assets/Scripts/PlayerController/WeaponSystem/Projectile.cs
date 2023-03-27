using UnityEngine;

namespace PlayerController.WeaponSystem
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _startSpeed;
        [SerializeField] private float _gravityImpact;
        [SerializeField] private float _slowImpact;
        [SerializeField] private float _affectedAreaRadius;
        [SerializeField] private LayerMask _canBeCollided;
        [SerializeField] private LayerMask _canBeDamaged;
        [SerializeField] private float _detectRadius;
 
        private Transform _transform;
        private Vector3 _dir;
        private float _curHorSpeed;
        private float _curVerSpeed;

        private void Awake()
        {
            _transform = transform;
        }

        public void Initialize(Vector3 startPoint, Vector3 dir, float lifeTime)
        {
            _transform.position = startPoint;
            _dir = dir;
            _curHorSpeed = _startSpeed * new Vector2(_dir.x, _dir.z).magnitude;
            _curVerSpeed = _startSpeed * _dir.y;

            Invoke("Collision", lifeTime);
        }

        private void FixedUpdate()
        {
            Vector3 step = new Vector3(_dir.x, 0, _dir.z);
            step = step * _curHorSpeed * Time.fixedDeltaTime;
            step += Vector3.up * _curVerSpeed * Time.fixedDeltaTime;
            _transform.position += step;
            
            _curHorSpeed -= _slowImpact * Time.fixedDeltaTime;
            if (Mathf.Abs(_curVerSpeed) > 8)
                _curVerSpeed = 8 * Mathf.Sign(_curVerSpeed);
            else
                _curVerSpeed += _gravityImpact * Time.fixedDeltaTime;

            if (Physics.OverlapSphere(_transform.position, _detectRadius, _canBeCollided).Length > 0)
                Collision();
        }

        private void Collision()
        {
            if (gameObject.activeSelf == false) return;
            var colliders = Physics.OverlapSphere(_transform.position, _affectedAreaRadius, _canBeDamaged);
            foreach (var collider in colliders)
                Debug.Log("Hit " + collider.name);
                
            gameObject.SetActive(false);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _detectRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _affectedAreaRadius);
        }
    }
}
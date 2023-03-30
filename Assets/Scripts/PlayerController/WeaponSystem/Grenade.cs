using UnityEngine;
using Enemys;

namespace PlayerController.WeaponSystem
{
    public class Grenade : Projectile
    {
        [SerializeField] protected float _gravityImpact;

        [Header ("Explosion")]
        [SerializeField] protected float _affectedAreaRadius;

        protected EnemyKeeper _enemyKeeper;

        public void Initialize(EnemyKeeper enemyKeeper, Vector3 startPoint, Vector3 dir,
            int damage, float lifeTime)
        {
            _enemyKeeper = enemyKeeper;
            _transform.position = startPoint;
            _dir = dir;
            _curHorSpeed = _startSpeed * new Vector2(_dir.x, _dir.z).magnitude;
            _curVerSpeed = _startSpeed * _dir.y;
            _damage = damage;

            Invoke("Collision", lifeTime);
        }

        protected override void FixedUpdate()
        {
            Vector3 step = new Vector3(_dir.x, 0, _dir.z);
            step = step * _curHorSpeed * Time.fixedDeltaTime;
            step += Vector3.up * _curVerSpeed * Time.fixedDeltaTime;
            _transform.position += step;

            _curHorSpeed += _speedChanges * Time.fixedDeltaTime;
            if (Mathf.Abs(_curVerSpeed) > 8)
                _curVerSpeed = 8 * Mathf.Sign(_curVerSpeed);
            else
                _curVerSpeed += _gravityImpact * Time.fixedDeltaTime;

            if (Physics.OverlapSphere(_transform.position, _detectRadius, _canBeCollided).Length > 0)
                Collision();
        }

        protected override void Collision()
        {
            if (gameObject.activeSelf == false) return;
            var colliders = Physics.OverlapSphere(_transform.position, _affectedAreaRadius, _canBeDamaged);
            foreach (var collider in colliders)
                _enemyKeeper.MakeDamage(collider.gameObject, _damage, true);

            CancelInvoke();
            gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _affectedAreaRadius);
        }
#endif
    }
}
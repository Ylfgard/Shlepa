using UnityEngine;
using Enemys;

namespace PlayerController.WeaponSystem
{
    public class Grenade : Projectile
    {
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

        protected override void Collision()
        {
            if (gameObject.activeSelf == false) return;
            var colliders = Physics.OverlapSphere(_transform.position, _affectedArea, _canBeDamaged);
            foreach (var collider in colliders)
            {
                RaycastHit hit;
                if (Physics.Raycast(_transform.position, collider.transform.position - _transform.position, out hit, _affectedArea, _canBeCollided))
                    if (Physics.OverlapSphere(hit.point, 0.01f, _canBeDamaged).Length > 0)
                        _enemyKeeper.MakeDamage(collider.gameObject, _damage, true);
            }

            CancelInvoke();
            gameObject.SetActive(false);
        }
    }
}
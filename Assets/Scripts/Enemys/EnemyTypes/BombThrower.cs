using System.Collections;
using UnityEngine;

namespace Enemys
{
    public class BombThrower : DistanceKeepEnemy
    {
        [Header ("Bomb Parameters")]
        [SerializeField] protected float _bombFlyTime;
        [SerializeField] protected float _bombFlyHight;

        protected float _startHSpeed;
        protected float _startVSpeed;
        protected float _gravityImpact;

        protected override void FastShot()
        {
            for (int i = 0; i < _bulletsPerShot; i++)
            {
                var bullet = _bullets.GetObjectFromPool();
                Vector3 dir = CalculateBulletDir(i);
                bullet.Initialize(_shotPoint.position, dir, _damage, _bulletLifeTime, 
                    _startHSpeed, _startVSpeed, _gravityImpact);
            }
            _agent.isStopped = false;
        }

        protected override IEnumerator Shot()
        {
            Vector3 dirToTarget = _target.position - _shotPoint.position;
            for (int i = 0; i < _bulletsPerShot; i++)
            {
                var bullet = _bullets.GetObjectFromPool();
                Vector3 dir = CalculateBulletDir(i);
                bullet.Initialize(_shotPoint.position, dir, _damage, _bulletLifeTime,
                    _startHSpeed, _startVSpeed, _gravityImpact);
                yield return new WaitForSeconds(_bulletDelay);
            }
            _agent.isStopped = false;
        }

        protected override Vector3 CalculateBulletDir(int number)
        {
            Vector3 dir = _target.position - _shotPoint.position;

            _startHSpeed = dir.magnitude / _bombFlyTime;
            float t = _bombFlyTime / 2;
            _gravityImpact = (-2 * _bombFlyHight) / Mathf.Pow(t, 2);
            _startVSpeed = -_gravityImpact * t;
            dir.y = 0;
            return dir.normalized;
        }
    }
}

using UnityEngine;
using System.Collections;

namespace Enemys.AIModules
{
    public class KineticAttackModule : RangeAttackModule
    {
        [Header("Projectile Parameters")]
        [SerializeField] protected float _flyTime;
        [SerializeField] protected float _flyHight;

        protected float _startHSpeed;
        protected float _startVSpeed;
        protected float _gravityImpact;

        protected override void Start()
        {
            base.Start();
            _bulletLifeTime = _flyTime;
        }

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
            AttackFinished?.Invoke();
        }

        protected override IEnumerator Shot()
        {
            for (int i = 0; i < _bulletsPerShot; i++)
            {
                var bullet = _bullets.GetObjectFromPool();
                Vector3 dir = CalculateBulletDir(i);
                bullet.Initialize(_shotPoint.position, dir, _damage, _bulletLifeTime,
                    _startHSpeed, _startVSpeed, _gravityImpact);
                yield return new WaitForSeconds(_bulletDelay);
            }
            _agent.isStopped = false;
            AttackFinished?.Invoke();
        }

        protected override Vector3 CalculateBulletDir(int number)
        {
            Vector3 dir = _target.position - _shotPoint.position;
            if (_dispersion != 0)
            {
                float y = dir.y;
                dir.y = 0;
                float distance = dir.magnitude;
                Vector3 normal = Vector3.right;
                Vector3.OrthoNormalize(ref dir, ref normal);
                float x = _bulletsPerShot / 2;
                if (_randomDir)
                    x = Random.Range(-_dispersion, _dispersion);
                else
                    x = _dispersion * ((number - x) / x);

                dir = dir * _attackDistance + normal * x;
                dir = dir.normalized * distance;
                dir.y = y;
            }
            _startHSpeed = dir.magnitude / _flyTime;
            float t = _flyTime / 2;
            _gravityImpact = (-2 * _flyHight) / Mathf.Pow(t, 2);
            _startVSpeed = -_gravityImpact * t;
            dir.y = 0;
            return dir.normalized;
        }
    }
}
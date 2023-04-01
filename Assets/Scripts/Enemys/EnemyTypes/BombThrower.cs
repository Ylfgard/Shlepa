using UnityEngine;

namespace Enemys
{
    public class BombThrower : DistanceKeepEnemy
    {
        protected float _startSpeed;
        protected float _gravityImpact;

        protected override void Start()
        {
            base.Start();
            _startSpeed = _bullets.Value.StartSpeed;
            _gravityImpact = -_bullets.Value.GravityImpact;
        }

        protected override Vector3 CalculateBulletDir(int number)
        {
            Vector3 dir = _target.position - _shotPoint.position;
            float y = dir.y;
            dir.y = 0;
            float S = dir.magnitude;
            if (_dispersion != 0)
            {
                Vector3 normal = Vector3.right;
                Vector3.OrthoNormalize(ref dir, ref normal);
                float x = _bulletsPerShot / 2;
                if (_randomDir)
                    x = Random.Range(-_dispersion, _dispersion);
                else
                    x = _dispersion * ((number - x) / x);

                dir = dir * _attackDistance + normal * x;
                dir = dir.normalized * S;
            }

            float sqrt = Mathf.Pow(_startSpeed, 4) - _gravityImpact * (_gravityImpact * Mathf.Pow(S, 2) + 2 * Mathf.Pow(_startSpeed, 2) * y);
            float angleTan;
            if (sqrt < 0) 
                angleTan = Mathf.Tan(45);
            else 
                angleTan = Mathf.Tan((Mathf.Pow(_startSpeed, 2) - Mathf.Sqrt(sqrt)) / (_gravityImpact * S));
            
            dir.y = S * angleTan;
            return dir.normalized;
        }
    }
}

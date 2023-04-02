using UnityEngine;

namespace Enemys
{
    public class Sniper : RangeEnemy
    {
        [Header("Aim")]
        [SerializeField] protected Transform _aimPoint;
        [SerializeField] protected LineRenderer _aimLine;
        [SerializeField] protected Material _aimMaterial;
        [SerializeField] protected Material _shotMaterial;
        [SerializeField] protected float _aimSpeed;

        protected override void Awake()
        {
            base.Awake();
            _aimLine.SetPosition(0, _shotPoint.position);
        }

        public override void Initialize(Vector3 position)
        {
            base.Initialize(position);
            _aimPoint.position = _target.position;
        }

        protected override void FixedUpdate()
        {
            if (_isLanding)
            {
                CheckLanding();
                return;
            }

            if (_isAttacking == false)
            {
                Aim();
                Attack();
            }
        }

        protected override void Attack()
        {
            RaycastHit hit;
            if (Physics.Raycast(_shotPoint.position, _aimPoint.position - _shotPoint.position, out hit, _attackDistance, _canBeCollided))
            {
                if (Physics.OverlapSphere(hit.point, 0.01f, _canBeDamaged).Length > 0)
                {
                    _isAttacking = true;
                    _animationController.SetTrigger("Attack");
                    StartCoroutine(PrepareAttack());
                    _agent.isStopped = true;
                    _aimLine.material = _shotMaterial;
                }
            }
        }
        
        protected override Vector3 CalculateBulletDir(int number)
        {
            Vector3 dir = _aimPoint.position - _shotPoint.position;
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
            return dir.normalized;
        }

        protected void Aim()
        {
            Vector3 dir = _target.position - _aimPoint.position;
            dir = dir.normalized * _aimSpeed * Time.fixedDeltaTime;
            _aimPoint.position += dir;
            RaycastHit hit;
            if (Physics.Raycast(_shotPoint.position, _aimPoint.position - _shotPoint.position, out hit, _attackDistance, _canBeCollided))
                _aimLine.SetPosition(1, hit.point);
            else
                _aimLine.SetPosition(1, _shotPoint.position + (_aimPoint.position - _shotPoint.position).normalized * _attackDistance);
            _aimLine.material = _aimMaterial;
        }
    }
}
using UnityEngine;
using System.Collections;

namespace Enemys.AIModules
{
    public class SniperAttackModule : RangeAttackModule
    {
        [Header("Aim")]
        [SerializeField] protected Transform _aimPoint;
        [SerializeField] protected LineRenderer _aimLine;
        [SerializeField] protected Material _aimMaterial;
        [SerializeField] protected Material _shotMaterial;
        [SerializeField] protected float _aimSpeed;

        protected override void Start()
        {
            base.Start();
            _aimPoint.position = _shotPoint.position;
        }

        protected override IEnumerator ActivateAttack()
        {
            yield return new WaitForSeconds(_delayBeforeActivating);
            base.ActivateAttack();
            _aimLine.enabled = false;
        }

        protected override bool Attack()
        {
            if (_aimLine.enabled == false)
                _aimLine.enabled = true;
            Aim();
            RaycastHit hit;
            Vector3 dir = _aimPoint.position - _shotPoint.position;
            if (Physics.Raycast(_shotPoint.position, dir, out hit, dir.magnitude, _canBeCollided))
            {
                if (hit.collider.gameObject.tag == TagsKeeper.Player)
                {
                    _isAttacking = true;
                    _animationController.SetTrigger("Attack");
                    StartCoroutine(PrepareAttack());
                    _agent.isStopped = true;
                    _aimLine.material = _shotMaterial;
                    StartCoroutine(ActivateAttack());
                    return true;
                }
            }
            return false;
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
            _aimLine.SetPosition(0, _shotPoint.position);
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

        public override void Deactivate(Enemy enemy)
        {
            base.Deactivate(enemy);
            _aimPoint.position = _shotPoint.position;
        }
    }
}
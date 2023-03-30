using UnityEngine;

namespace Enemys
{
    public class Trollface : MeleeEnemy
    {
        [Header ("Explosion")]
        [SerializeField] protected GameObject _explosionVFX;
        [SerializeField] protected float _explosionRadius;

        protected override void Attack()
        {
            _animationController.SetTrigger("Attack");
            _isAttacking = true;
        }

        protected override void MakeDamage()
        {
            Destroy(Instantiate(_explosionVFX, _transform.position, Quaternion.identity), 2);
            Death();
        }

        public override void Death()
        {
            if (Vector3.Distance(_transform.position, _target.position) <= _explosionRadius)
                _player.Parameters.TakeDamage(_damage);

            base.Death();
        }

#if UNITY_EDITOR
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _explosionRadius);
        }
#endif
    }
}
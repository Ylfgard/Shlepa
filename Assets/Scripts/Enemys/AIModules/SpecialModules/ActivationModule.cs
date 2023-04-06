using UnityEngine;

namespace Enemys.AIModules
{
    public class ActivationModule : AIModule
    {
        [Header("Activation Parameters")]
        [SerializeField] protected float _activationDistance;

        protected bool _isActive;

        protected Transform _transform;
        protected Transform _target;

        public void Initialize(Enemy enemy)
        {
            _transform = enemy.Transform;
            _target = enemy.Player.Mover.Transform;
            enemy.TakedDamage += ForcedActivation;
        }

        public void StartActivator()
        {
            _isActive = false;
        }

        public bool CheckActivation()
        {
            if (_isActive == false && Vector2.Distance(new Vector2(_target.position.x, _target.position.z),
                new Vector2(_transform.position.x, _transform.position.z)) <= _activationDistance)
                _isActive = true;
            return _isActive;
        }

        protected void ForcedActivation(int healthPercent)
        {
            _isActive = true;
        }

#if UNITY_EDITOR
        protected void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, _activationDistance);
        }
#endif
    }
}
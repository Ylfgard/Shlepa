using UnityEngine;

namespace Enemys
{
    public abstract class WeakPoint : MonoBehaviour
    {
        [SerializeField] private bool _canBeSplashDamaged;

        protected Enemy _enemy;

        public bool CanBeSplashDamaged => _canBeSplashDamaged;
        public Enemy Enemy => _enemy;

        public virtual void Initialize(Enemy enemy)
        {
            _enemy = enemy;
        }

        public abstract void TakeDamage(int value);
    }
}
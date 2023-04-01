using UnityEngine;
using PlayerController;

namespace Enemys.Projectiles
{
    public class Bullet : Projectile
    {
        [SerializeField] protected bool _canBeHit;
        [SerializeField] protected int _maxHealth;

        protected Player _player;
        protected int _curHealth;

        public LayerMask CanBeCollided => _canBeCollided;
        public LayerMask CanBeDamaged => _canBeDamaged;
        public float StartSpeed => _startSpeed;
        public float GravityImpact => _gravityImpact;

        protected void Start()
        {
            _player = Player.Instance;
            if (_canBeHit)
                EnemyKeeper.Instance.AddBullet(this);
        }

        public override void Initialize(Vector3 startPoint, Vector3 dir, int damage, float lifeTime)
        {
            base.Initialize(startPoint, dir, damage, lifeTime);
            _curHealth = _maxHealth;
        }

        protected override void Collision()
        {
            if (gameObject.activeSelf == false) return;
            if (Physics.OverlapSphere(_transform.position, _affectedArea, _canBeDamaged).Length > 0)
                _player.Parameters.TakeDamage(_damage);

            CancelInvoke();
            gameObject.SetActive(false);
        }

        protected void Destroing()
        {
            if (gameObject.activeSelf == false) return;
            if (Physics.OverlapSphere(_transform.position, _affectedArea / 2, _canBeDamaged).Length > 0)
                _player.Parameters.TakeDamage(_damage);

            CancelInvoke();
            gameObject.SetActive(false);
        }

        public virtual void TakeDamage(int value)
        {
            _curHealth -= value;
            if (_curHealth <= 0)
                Destroing();
        }
    }
}
using UnityEngine;
using System.Collections.Generic;

namespace Enemys.Projectiles
{
    public class ProjectilePoolsKeeper : MonoBehaviour
    {
        [SerializeField] private GameObject _bullet;
        [SerializeField] private GameObject _cannonball;
        [SerializeField] private GameObject _bomb;
        [SerializeField] private GameObject _explosiveBullet;
        [SerializeField] private GameObject _bossCannonball;

        private ObjectPool<Bullet> _bullets;
        private ObjectPool<Bullet> _cannonballs;
        private ObjectPool<Bullet> _bombs;
        private ObjectPool<Bullet> _explosiveBullets;
        private ObjectPool<Bullet> _bossCannonballs;

        public ObjectPool<Bullet> Bullets => _bullets;
        public ObjectPool<Bullet> Cannonballs => _cannonballs;
        public ObjectPool<Bullet> Bombs => _bombs;
        public ObjectPool<Bullet> ExplosiveBullets => _explosiveBullets;
        public ObjectPool<Bullet> BossCannonballs => _bossCannonballs;

        // Singleton
        private static ProjectilePoolsKeeper _instance;
        public static ProjectilePoolsKeeper Instance => _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            else
                _instance = this;

            _bullets = new ObjectPool<Bullet>(_bullet);
            _cannonballs = new ObjectPool<Bullet>(_cannonball);
            _bombs = new ObjectPool<Bullet>(_bomb);
            _explosiveBullets = new ObjectPool<Bullet>(_explosiveBullet);
            _bossCannonballs = new ObjectPool<Bullet>(_bossCannonball);
        }
    }

    public enum ProjectileType
    {
        Bullet,
        Cannonball,
        Bomb,
        ExplosiveBullet,
        BossCannonball
    }
}
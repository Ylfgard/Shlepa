using UnityEngine;
using System.Collections.Generic;

namespace Enemys.Projectiles
{
    public class ProjectilePoolsKeeper : MonoBehaviour
    {
        [SerializeField] private GameObject _bullet;
        [SerializeField] private GameObject _canonball;
        [SerializeField] private GameObject _bomb;
        [SerializeField] private GameObject _explosiveBullet;

        private ObjectPool<Bullet> _bullets;
        private ObjectPool<Bullet> _canonballs;
        private ObjectPool<Bullet> _bombs;
        private ObjectPool<Bullet> _explosiveBullets;

        public ObjectPool<Bullet> Bullets => _bullets;
        public ObjectPool<Bullet> Canonballs => _canonballs;
        public ObjectPool<Bullet> Bombs => _bombs;
        public ObjectPool<Bullet> ExplosiveBullets => _explosiveBullets;

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
            _canonballs = new ObjectPool<Bullet>(_canonball);
            _bombs = new ObjectPool<Bullet>(_bomb);
            _explosiveBullets = new ObjectPool<Bullet>(_explosiveBullet);
        }
    }
}
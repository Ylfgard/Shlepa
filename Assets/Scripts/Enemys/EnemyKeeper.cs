using UnityEngine;
using System.Collections.Generic;

namespace Enemys
{
    public class EnemyKeeper : MonoBehaviour
    {
        private Dictionary<GameObject, Enemy> _enemys;
        private Dictionary<GameObject, WeakPoint> _weakPoints;

        // Singleton
        private static EnemyKeeper _instance;
        public static EnemyKeeper Instance => _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            else
                _instance = this;

            _enemys = new Dictionary<GameObject, Enemy>();
            _weakPoints = new Dictionary<GameObject, WeakPoint>();
        }

        public void AddEnemy(Enemy enemy)
        {
            if (_enemys.ContainsValue(enemy)) return;

            _enemys.Add(enemy.gameObject, enemy);
            foreach (WeakPoint point in enemy.WeakPoints)
                _weakPoints.Add(point.gameObject, point);
        }

        public void MakeDamage(GameObject body, int value, bool isSplash)
        {
            Enemy enemy;
            if (_enemys.TryGetValue(body, out enemy))
            {
                enemy.TakeDamage(value);
                return;
            }

            WeakPoint weakPoint;
            if (_weakPoints.TryGetValue(body, out weakPoint))
            {
                if (isSplash)
                {
                    if (weakPoint.CanBeSplashDamaged)
                        weakPoint.TakeDamage(value);
                }
                else
                {
                    weakPoint.TakeDamage(value);
                }
                return;
            }

            Debug.LogError("Unknown enemy! " + body);
        }
    }
}
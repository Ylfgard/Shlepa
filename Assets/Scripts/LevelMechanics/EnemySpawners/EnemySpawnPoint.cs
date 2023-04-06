using UnityEngine;
using TMPro;
using Enemys;
using System.Collections;
using System;

namespace LevelMechanics.EnemySpawners
{
    public class EnemySpawnPoint : MonoBehaviour
    {
        [SerializeField] private Transform _spawnArea;
        [SerializeField] private TextMeshPro _timerText;
        [SerializeField] private float _spawnDelay;

        private float _curTime;
        private Transform _transform;

        private void Awake()
        {
            _spawnArea.gameObject.SetActive(false);
            _timerText.enabled = false;
            _curTime = 0;
            _transform = transform;
        }

        private void FixedUpdate()
        {
            if (_curTime > 0)
            {
                _curTime -= Time.fixedDeltaTime;
                _timerText.text = Mathf.RoundToInt(_curTime).ToString();
            }
        }

        public void ActivateSpawner(ObjectPool<Enemy> enemys, Action<Enemy> InvokeDeathEvent, float spawnAreaRadius)
        {
            _spawnArea.gameObject.SetActive(true);
            _timerText.enabled = true;
            _curTime = _spawnDelay;
            _spawnArea.localScale = Vector3.one * (spawnAreaRadius * 2);
            StartCoroutine(DelayedSpawn(enemys, InvokeDeathEvent));
        }

        public IEnumerator DelayedSpawn(ObjectPool<Enemy> enemys, Action<Enemy> InvokeDeathEvent)
        {
            yield return new WaitForSeconds(_spawnDelay);
            Enemy enemy = enemys.GetObjectFromPool();
            enemy.SendDeath += InvokeDeathEvent;
            enemy.Initialize(_transform.position);
            _spawnArea.gameObject.SetActive(false);
            _timerText.enabled = false;
        }
    }
}
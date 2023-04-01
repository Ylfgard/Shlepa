using UnityEngine;
using Enemys;
using System.Collections.Generic;
using System;

namespace LevelMechanics
{
    public class EnemySpawner : MonoBehaviour
    {
        public Action SendEnemyDeath;

        [SerializeField] private GameObject _enemy;
        [SerializeField] private StageSpawnerData[] _stagesData;

        private Dictionary<int, StageSpawnerData> _stages;

        private ObjectPool<Enemy> _enemys;

        private void Awake()
        {
            _enemys = new ObjectPool<Enemy>(_enemy);
            _stages = new Dictionary<int, StageSpawnerData>();
            foreach (StageSpawnerData stage in _stagesData)
            {
                stage.Initialize(_enemys);
                stage.SendEnemyDeath += DecreaseEnemyCount;
                if (_stages.ContainsKey(stage.Index)) Debug.LogError("Wrong stage index: " + stage.Index + ", spawner: " + gameObject.name);
                else _stages.Add(stage.Index, stage);
            }
        }

        private void Start()
        {
            StagesKeeper.Instance.AddSpawner(this);
        }

        public int ActivateStage(int index)
        {
            int enemyCount = 0;
            StageSpawnerData stage;
            if (_stages.TryGetValue(index, out stage))
                enemyCount = stage.Activate();
            return enemyCount;
        }

        private void DecreaseEnemyCount()
        {
            SendEnemyDeath?.Invoke();
        }
    }

    [Serializable]
    public class StageSpawnerData
    {
        public Action SendEnemyDeath;
        
        [SerializeField] private int _index;
        [SerializeField] private Transform[] _spawnPoints;
        
        private ObjectPool<Enemy> _enemys;

        public int Index => _index;

        public void Initialize(ObjectPool<Enemy> enemys)
        {
            _enemys = enemys;
        }

        public int Activate()
        {
            foreach(Transform point in _spawnPoints)
            {
                var enemy = _enemys.GetObjectFromPool();
                enemy.Initialize(point.position);
                enemy.SendDeath += InvokeDeathEvent;
            }
            return _spawnPoints.Length;
        }

        private void InvokeDeathEvent(Enemy enemy)
        {
            SendEnemyDeath?.Invoke();
            enemy.SendDeath -= InvokeDeathEvent;
        }
    }
}
using UnityEngine;
using Enemys;
using System.Collections.Generic;
using System;
using Enemys.Bosses;

namespace LevelMechanics.EnemySpawners
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
            if (_enemy.TryGetComponent<Boss>(out Boss boss))
                _enemys.PreSpawn(1);
            else
                _enemys.PreSpawn(5);
            StagesKeeper.Instance.AddEnemySpawner(this);
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
        [SerializeField] private EnemySpawnPoint[] _spawnPoints;
        
        private ObjectPool<Enemy> _enemys;
        private float _spawnAreaRadius;

        public int Index => _index;

        public void Initialize(ObjectPool<Enemy> enemys)
        {
            _enemys = enemys;
            ;
            if (_enemys.Value.TryGetComponent(out LandingEnemy enemy))
                _spawnAreaRadius = enemy.LandingAreaRadius;
            else
                _spawnAreaRadius = 0;
        }

        public int Activate()
        {
            foreach(EnemySpawnPoint point in _spawnPoints)
                point.ActivateSpawner(_enemys, InvokeDeathEvent, _spawnAreaRadius);
            return _spawnPoints.Length;
        }

        private void InvokeDeathEvent(Enemy enemy)
        {
            SendEnemyDeath?.Invoke();
            enemy.SendDeath -= InvokeDeathEvent;
        }
    }
}
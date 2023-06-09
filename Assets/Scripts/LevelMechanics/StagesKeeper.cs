using UnityEngine;
using System.Collections.Generic;
using System;
using LevelMechanics.ActivatableObjects;
using LevelMechanics.EnemySpawners;
using LevelMechanics.Pickups;

namespace LevelMechanics
{
    public class StagesKeeper : MonoBehaviour
    {
        public Action<int> SendClearPercent;

        [SerializeField] private StageData[] _stagesData;

        private List<EnemySpawner> _enemySpawners;
        private List<PickupsSpawner> _pickupsSpawners;
        private Dictionary<int, StageData> _stages;
        private float _totalEnemyCount;
        private float _curEnemyCount;
        private bool _isWaveStage;
        private int _curStageIndex;

        // Singleton
        private static StagesKeeper _instance;
        public static StagesKeeper Instance => _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            else
                _instance = this;

            _enemySpawners = new List<EnemySpawner>();
            _pickupsSpawners = new List<PickupsSpawner>();
            _stages = new Dictionary<int, StageData>();
            foreach (StageData stage in _stagesData)
            {
                if (_stages.ContainsKey(stage.Index)) Debug.LogError("Wrong stage index: " + stage.Index);
                else _stages.Add(stage.Index, stage);
            }
        }

        public void AddEnemySpawner(EnemySpawner spawner)
        {
            _enemySpawners.Add(spawner);
            spawner.SendEnemyDeath += DecreaseEnemyCount;
        }

        public void AddPickupsSpawner(PickupsSpawner spawner)
        {
            _pickupsSpawners.Add(spawner);
        }    

        public void ActivateStage(int index, bool isWave)
        {
            if (isWave == false)
            {
                _totalEnemyCount = 0;
                _curEnemyCount = 0;
                _isWaveStage = false;
            }
            else
            {
                if (_isWaveStage == false)
                {
                    _totalEnemyCount = 0;
                    _curEnemyCount = 0;
                }
                _isWaveStage = true;
            }

            EndStage();
            _curStageIndex = index;

            int newEnemys = 0;
            foreach (EnemySpawner spawner in _enemySpawners)
                newEnemys = spawner.ActivateStage(index);
            _totalEnemyCount += newEnemys;
            _curEnemyCount += newEnemys;

            foreach (PickupsSpawner spawner in _pickupsSpawners)
                spawner.ActivateStage(index);

            StageData stage;
            if (_stages.TryGetValue(index, out stage))
                stage.Activate();
        }

        public void EndStage()
        {
            StageData stage;
            if (_stages.TryGetValue(_curStageIndex, out stage))
                stage.Deactivate();
        }

        private void DecreaseEnemyCount()
        {
            _curEnemyCount--;
            int percent =Mathf.CeilToInt((1f - (_curEnemyCount / _totalEnemyCount)) * 100f);
            SendClearPercent?.Invoke(percent);
        }
    }
    
    [Serializable]
    public class StageData
    {
        [SerializeField] private int _index;
        [SerializeField] private ActivatableObject[] _activateObjects;
        [SerializeField] private ActivatableObject[] _deactivateObjects;

        public int Index => _index;

        public void Activate()
        {
            foreach (ActivatableObject obj in _activateObjects)
                obj.Activate();
        }

        public void Deactivate()
        {
            foreach (ActivatableObject obj in _deactivateObjects)
                obj.Deactivate();
        }
    }
}


using UnityEngine;
using System;
using System.Collections.Generic;

namespace LevelMechanics.Pickups
{
    public class PickupsSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _item;
        [SerializeField] private StageSpawnerData[] _stagesData;

        private ObjectPool<PickupItems> _items;
        private Dictionary<int, StageSpawnerData> _stages;

        private void Awake()
        {
            _items = new ObjectPool<PickupItems>(_item);
            _stages = new Dictionary<int, StageSpawnerData>();
            foreach (var stage in _stagesData)
            {
                stage.Intialize(_items);
                if (_stages.ContainsKey(stage.Index)) Debug.LogError("Wrong stage index! " + stage.Index);
                _stages.Add(stage.Index, stage);
            }
        }

        private void Start()
        {
            StagesKeeper.Instance.AddPickupsSpawner(this);
        }

        public void ActivateStage(int index)
        {
            StageSpawnerData data;
            if (_stages.TryGetValue(index, out data))
                data.Spawn();
        }
    }

    [Serializable]
    public class StageSpawnerData
    {
        [SerializeField] private int _index;
        [SerializeField] private Transform[] _points;

        private ObjectPool<PickupItems> _items;

        public int Index => _index;

        public void Intialize(ObjectPool<PickupItems> items)
        {
            _items = items;
        }

        public void Spawn()
        {
            foreach(var point in _points)
            {
                var item = _items.GetObjectFromPool();
                item.Initialize(point.position);
            }
        }
    }
}
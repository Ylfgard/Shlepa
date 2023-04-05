using UnityEngine;
using System.Collections.Generic;

namespace Enemys.Bosses
{
    public class TrollBoss : BossCower
    {
        [Header("Boss Stages")]
        [SerializeField] private TrollBossStage[] _stageParameters;

        protected Dictionary<int, TrollBossStage> _stages;

        protected override void Awake()
        {
            base.Awake();
            _stages = new Dictionary<int, TrollBossStage>();
            foreach (var stage in _stageParameters)
            {
                if (_stages.ContainsKey(stage.Index)) Debug.LogError("Wrong stage index! " + stage.Index);
                else _stages.Add(stage.Index, stage);
            }
        }

        public override void ActivateStage(int stageIndex)
        {
            TrollBossStage stage;
            if (_stages.TryGetValue(stageIndex, out stage))
            {
                _attackDelay = stage.AttackDelay;
                _dispersion = stage.Dispersion;
                _bulletsPerShot = stage.BulletsPerShot;
                _attackDistance = stage.AttackDistance;
                _distanceFromTarget = stage.DistanceFromTarget;
            }
        }
    }
}
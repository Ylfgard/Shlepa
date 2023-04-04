using UnityEngine;
using System;

namespace Enemys.Bosses
{
    public abstract class Boss : Enemy
    {
        public Action<int> TakedDamage;

        [Header("Stages")]
        [SerializeField] protected StageTrigger[] _stageTriggers;

        protected bool _isActive;

        protected override void Awake()
        {
            base.Awake();
            foreach (var stage in _stageTriggers)
                stage.Initialize(this);
        }

        public override void Initialize(Vector3 position)
        {
            base.Initialize(position);
            _isActive = false;
        }

        protected virtual void FixedUpdate()
        {
            if (_isActive == false && Vector3.Distance(_target.position, _transform.position) <= _attackDistance)
                _isActive = true;
        }

        public abstract void ActivateStage(int stageIndex);

        public override float LandingAreaRadius()
        {
            return 0;
        }

        public override void TakeDamage(int value)
        {
            base.TakeDamage(value);
            TakedDamage?.Invoke(GetHealthPercent());
        }

        public int GetHealthPercent()
        {
            float percent = ((float)_curHealth / _maxHealth) * 100;
            return Mathf.RoundToInt(percent);
        }
    }
}
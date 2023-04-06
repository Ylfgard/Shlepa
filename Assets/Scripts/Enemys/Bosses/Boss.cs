using UnityEngine;
using Enemys.AIModules;

namespace Enemys.Bosses
{
    public abstract class Boss : Enemy
    {
        [Header("Modules")]
        [SerializeField] protected ActivationModule _activator;

        protected override void Awake()
        {
            base.Awake();
            _activator.Initialize(this);
            SendDeath += _activator.Deactivate;
        }

        public override void Initialize(Vector3 position)
        {
            base.Initialize(position);
            _activator.StartActivator();
        }

        public abstract void ActivateStage(BossStage stage);
    }
}
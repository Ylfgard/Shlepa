using UnityEngine;
using Enemys.AIModules;

namespace Enemys.Bosses
{
    public abstract class Boss : Enemy
    {
        [Header("Modules")]
        [SerializeField] protected ActivationModule _activator;

        protected override void Start()
        {
            base.Start();
            _activator.Initialize(this);
            SendDeath += _activator.Deactivate;
        }

        public override void Initialize(Vector3 position)
        {
            base.Initialize(position);
            _activator.StartActivator();
        }

        protected virtual void FixedUpdate()
        {
            if (_activator.CheckActivation() == false) return;
        }

        public abstract void ActivateStage(BossStage stage);
    }
}
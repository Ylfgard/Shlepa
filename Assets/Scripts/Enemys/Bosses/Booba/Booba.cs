using UnityEngine;
using Enemys.AIModules;

namespace Enemys.Bosses
{
    public class Booba : Boss
    {
        [SerializeField] protected JumpModule _jumper;

        [Header ("Stages")]
        [SerializeField] private BoobaStage[] _stages;

        protected override void Awake()
        {
            base.Awake();
            var controller = GetComponent<CharacterController>();
            _jumper.Initialize(this, controller);
            foreach (BoobaStage stage in _stages)
                stage.Initialize(this);
        }

        protected virtual void FixedUpdate()
        {
            _jumper.Move();
            
            if (_activator.CheckActivation() == false) return;

            _jumper.TryJump();
        }

        public override void ActivateStage(BossStage stage)
        {
            _jumper.ChangeParameters(stage as BoobaStage);
        }
    }
}
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
            foreach (BoobaStage stage in _stages)
                stage.Initialize(this);
        }

        protected override void Start()
        {
            base.Start();
            var controller = GetComponent<CharacterController>();
            _jumper.Initialize(this, controller);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            _jumper.TryJump();
        }

        public override void ActivateStage(BossStage stage)
        {
            _jumper.ChangeParameters(stage as BoobaStage);
        }
    }
}
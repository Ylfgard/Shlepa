using UnityEngine;
using Enemys.AIModules;
using PlayerController;

namespace Enemys
{
    public abstract class LandingEnemy : Enemy
    {
        [Header("Modules")]
        [SerializeField] protected LandingModule _lander;

        public float LandingAreaRadius => _lander.LandingAreaRadius;

        protected override void Awake()
        {
            base.Awake();
            _lander.Initialize(this);
            SendDeath += _lander.Deactivate;
        }

        public override void Initialize(Vector3 position)
        {
            base.Initialize(position);
            _lander.StartLanding();
        }
    }
}
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

        protected override void Start()
        {
            if (_lander.IsInitialized == false)
            {
                base.Start();
                _lander.Initialize(this);
                SendDeath += _lander.Deactivate;
            }
        }

        public override void Initialize(Vector3 position)
        {
            base.Initialize(position);
            if (_lander.IsInitialized == false)
            {
                _player = Player.Instance;
                EnemyKeeper.Instance.AddEnemy(this);
                _lander.Initialize(this);
                SendDeath += _lander.Deactivate;
            }
            _lander.StartLanding();
        }
    }
}
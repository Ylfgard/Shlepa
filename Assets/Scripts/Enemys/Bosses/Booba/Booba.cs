using UnityEngine;
using Enemys.AIModules;
using System.Collections;

namespace Enemys.Bosses
{
    public class Booba : Boss
    {
        [SerializeField] protected GroundSpikesModule _spikes;

        [Header ("Stages")]
        [SerializeField] private BoobaStage[] _stages;

        protected override void Awake()
        {
            base.Awake();
            var controller = GetComponent<CharacterController>();
            _spikes.Initialize(this, controller);
            foreach (BoobaStage stage in _stages)
                stage.Initialize(this);
        }

        public override void Initialize(Vector3 position)
        {
            _transform.position = position;
            _curHealth = _maxHealth;
            _isAlive = true;
            _spikes.Activate(position);
        }

        protected virtual void FixedUpdate()
        {
            if (_isAlive == false) return;

            _spikes.Move();

            if (_activator.CheckActivation() == false) return;

            _spikes.TryJump();
        }

        public override void ActivateStage(BossStage stage)
        {
            _spikes.ChangeParameters(stage as BoobaStage);
        }

        protected override IEnumerator DeathDelay()
        {
            _animationController.SetTrigger("Death");
            _isAlive = false;
            yield return new WaitForSeconds(_deathDelay);
            SendDeath?.Invoke(this);
            gameObject.SetActive(false);
        }
    }
}
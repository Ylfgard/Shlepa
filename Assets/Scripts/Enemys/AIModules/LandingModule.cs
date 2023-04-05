using UnityEngine;
using PlayerController;
using UnityEngine.AI;

namespace Enemys.AIModules
{
    public class LandingModule : MonoBehaviour
    {
        protected const float _spawnHight = 10;
        protected const float _fallSpeed = 20;

        [Header("Spawn Parameters")]
        [SerializeField] protected LayerMask _groundLayer;
        [SerializeField] protected float _landingAreaRadius;
        [SerializeField] protected int _landingDamage;
        
        protected Transform _transform;
        protected Player _player;
        protected Transform _target;
        protected NavMeshAgent _agent;
        protected bool _isLanding;

        public void Initialize(Transform myTransform, Player player, NavMeshAgent agent)
        {
            _transform = myTransform;
            _player = player;
            _target = player.Mover.Transform;
            _agent = agent;
        }

        public void StartLanding()
        {
            _isLanding = false;
            _agent.enabled = false;
        }

        public bool TryLanded()
        {
            if (_isLanding)
                CheckLanding();
            return _isLanding;
        }

        protected void CheckLanding()
        {
            if (Physics.Raycast(_transform.position, Vector3.down, 2f, _groundLayer))
            {
                Landing();
            }
            else
            {
                Vector3 newPos = _transform.position;
                newPos.y -= _fallSpeed * Time.fixedDeltaTime;
                _transform.position = newPos;
            }
        }

        protected void Landing()
        {
            if (Vector3.Distance(_transform.position, _target.position) <= _landingAreaRadius)
                _player.Parameters.TakeDamage(_landingDamage);
            _isLanding = false;
            _agent.enabled = true;
        }
    }
}
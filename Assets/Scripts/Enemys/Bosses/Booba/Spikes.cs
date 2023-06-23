using UnityEngine;
using PlayerController;

namespace Enemys.Bosses
{
    public class Spikes : MonoBehaviour
    {
        [SerializeField] private AnimationController _controller;

        private bool _spikesActive;
        private int _damage;
        private PlayerParameters _parameters;

        public void AttackSpikes()
        {
            _spikesActive = true;
            _controller.SetTrigger("Spikes");
        }

        public void DeactivateSpikes()
        {
            _spikesActive = false;
        }

        public void Initialize(PlayerParameters parameters, int damage)
        {
            _parameters = parameters;
            _damage = damage;
            _spikesActive = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_spikesActive)
            {
                if (other.CompareTag(TagsKeeper.Player))
                {
                    MakeDamage();
                }
            }
        }

        private void MakeDamage()
        {
            _parameters.TakeDamage(_damage);
            _spikesActive = false;
        }
    }
}
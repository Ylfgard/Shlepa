using UnityEngine;
using PlayerController;

namespace LevelMechanics.Pickups
{
    public abstract class PickupItems : MonoBehaviour
    {
        protected Player _player;
        protected Transform _transform;

        private void Awake()
        {
            _player = Player.Instance;
            _transform = transform;
        }

        public void Initialize(Vector3 position)
        {
            _transform.position = position;
        }

        protected void OnTriggerEnter(Collider other)
        {
            if (other.tag == TagsKeeper.Player)
                Pickup();
        }

        protected abstract void Pickup();
    }
}
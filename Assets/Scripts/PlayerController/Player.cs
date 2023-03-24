using UnityEngine;
using PlayerController.WeaponSystem;

namespace PlayerController
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private PlayerMovement _mover;
        [SerializeField] private PlayerCamera _camera;
        [SerializeField] private WeaponKeeper _weaponKeeper;

        public PlayerMovement Mover => _mover;
        public PlayerCamera Camera => _camera;

        // Singleton
        private static Player _instance;
        public static Player Instance => _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            else
                _instance = this;
        }

        public void Shot()
        {
            var weapon = _weaponKeeper.CurWeapon;
            weapon.Shot(_camera.Transform);
        }
    }
}
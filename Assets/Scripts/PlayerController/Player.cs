using UnityEngine;
using PlayerController.WeaponSystem;

namespace PlayerController
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private PlayerMovement _mover;
        [SerializeField] private PlayerCamera _camera;
        [SerializeField] private WeaponKeeper _weaponKeeper;
        [SerializeField] private PlayerParameters _parameters;

        public PlayerMovement Mover => _mover;
        public PlayerCamera Camera => _camera;
        public PlayerParameters Parameters => _parameters;

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
            _weaponKeeper.CurWeapon.Shot(_camera.Transform);
        }

        public void ChangeWeapon(int slot)
        {
            _weaponKeeper.ChangeWeapon(slot);
        }

        public void ReloadWeapon()
        {
            _weaponKeeper.CurWeapon.Reload();
        }

        public void Aim(bool state)
        {
            if (_weaponKeeper.CurWeapon.AimValue == 1) return;
            else _camera.Zoom(_weaponKeeper.CurWeapon.AimValue, state);
        }
    }
}
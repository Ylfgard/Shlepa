using UnityEngine;
using PlayerController.WeaponSystem;
using LevelMechanics.SaveSystem;

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
        public WeaponKeeper WeaponKeeper => _weaponKeeper;

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

        public void Move(Vector3 dir)
        {
            _mover.Move(dir);
            if (dir.x == 0 && dir.z == 0)
                _weaponKeeper.SetWalkAnim(false);
            else
                _weaponKeeper.SetWalkAnim(true);
        }

        public void Shot()
        {
            float angle = _weaponKeeper.MakeShot(_camera.Transform);
            if (angle > 0) _camera.StartShake(angle);
        }

        public void ChangeWeapon(int slot)
        {
            _weaponKeeper.TryChangeWeapon(slot);
        }

        public void ScrollWeapon(bool next)
        {
            _weaponKeeper.ScrollWeapon(next);
        }

        public void ReloadWeapon()
        {
            _weaponKeeper.Reload();
        }

        public void Aim(bool state)
        {
            if (_weaponKeeper.GetWeaponAimValue() == 1) return;
            else _camera.Zoom(_weaponKeeper.GetWeaponAimValue(), state);
        }

        public void Load(SaveData save)
        {
            _parameters.SetParameters(save.Health, save.Armor);
            _weaponKeeper.SetAmmos(save.Ammos);
        }

        public void Load(SaveData save, int checkpointIndex)
        {
            _parameters.SetParameters(save.Health, save.Armor);
            _weaponKeeper.SetAmmos(save.Ammos);
            _mover.SetPosition(ChekpointKeeper.Instance.GetCheckpointPosition(checkpointIndex));
        }
    }
}
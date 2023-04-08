using UnityEngine;
using UI;

namespace PlayerController
{
    public class InputHandler : MonoBehaviour
    {
        private const string Cancel = "Cancel";
        private const string HorizAxis = "Horizontal";
        private const string VertAxis = "Vertical";
        private const string JumpAxis = "Jump";
        private const string MouseX = "Mouse X";
        private const string MouseY = "Mouse Y";
        private const string Shot = "Shot";
        private const string Slot1 = "Slot1";
        private const string Slot2 = "Slot2";
        private const string Slot3 = "Slot3";
        private const string Slot4 = "Slot4";
        private const string Slot5 = "Slot5";
        private const string Slot6 = "Slot6";
        private const string Reload = "Reload";
        private const string Aim = "Aim";

        [SerializeField] private float sensX;
        [SerializeField] private float sensY;

        private Player _player;
        private bool _isLocked;
        private Menu _menu;

        // Singleton
        private static InputHandler _instance;
        public static InputHandler Instance => _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            else
                _instance = this;
        }

        private void Start()
        {
            _menu = Menu.Instance;
            _player = Player.Instance;
        }

        private void Update()
        {
            if (_isLocked) return;

            if (Input.GetButtonDown(Cancel))
                _menu.ChangeMenuActive();

            Vector3 dir = Vector3.zero;

            dir.x = Input.GetAxisRaw(HorizAxis);
            dir.z = Input.GetAxisRaw(VertAxis);
            dir = dir.normalized * Time.deltaTime;

            dir.y = Input.GetAxis(JumpAxis);

            _player.Mover.Move(dir);

            float mouseX = Input.GetAxis(MouseX) * Time.deltaTime * sensX;
            _player.Mover.RotateX(mouseX);

            float mouseY = Input.GetAxis(MouseY) * Time.deltaTime * sensY;
            _player.Camera.Rotate(mouseX, mouseY);
            _player.Camera.TiltHead(dir.x, dir.z);

            if (Input.GetButton(Shot))
                _player.Shot();

            if (Input.GetButtonDown(Reload))
                _player.ReloadWeapon();

            if (Input.GetButtonDown(Aim))
                _player.Aim(true);

            if (Input.GetButtonUp(Aim))
                _player.Aim(false);

            if (Input.GetButtonDown(Slot1))
                _player.ChangeWeapon(1);
            if (Input.GetButtonDown(Slot2))
                _player.ChangeWeapon(2);
            if (Input.GetButtonDown(Slot3))
                _player.ChangeWeapon(3);
            if (Input.GetButtonDown(Slot4))
                _player.ChangeWeapon(4);
            if (Input.GetButtonDown(Slot5))
                _player.ChangeWeapon(5);
            if (Input.GetButtonDown(Slot6))
                _player.ChangeWeapon(6);
        }

        public void SetInputLockState(bool state)
        {
            _isLocked = state;
        }
    }
}
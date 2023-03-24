using UnityEngine;

namespace PlayerController
{
    public class InputHandler : MonoBehaviour
    {
        private const string HorizAxis = "Horizontal";
        private const string VertAxis = "Vertical";
        private const string JumpAxis = "Jump";
        private const string MouseX = "Mouse X";
        private const string MouseY = "Mouse Y";
        private const string Shot = "Shot";

        [SerializeField] private float sensX;
        [SerializeField] private float sensY;

        private Player _player;

        private void Start()
        {
            _player = Player.Instance;
        }

        private void Update()
        {
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
        }
    }
}
using UnityEngine;
using Global;

namespace PlayerController
{
    public class InputHandler : MonoBehaviour
    {
        [SerializeField] private float sensX;
        [SerializeField] private float sensY;

        private void Update()
        {
            Vector3 dir = Vector3.zero;

            dir.x = Input.GetAxisRaw(GlobalConstants.HorizAxis);
            dir.z = Input.GetAxisRaw(GlobalConstants.VertAxis);

            dir = dir.normalized * Time.deltaTime;
            Player.Instance.Move(dir);

            float mouseX = Input.GetAxis(GlobalConstants.MouseX) * Time.deltaTime * sensX;
            Player.Instance.RotateX(mouseX);

            float mouseY = Input.GetAxis(GlobalConstants.MouseY) * Time.deltaTime * sensY;
            PlayerCamera.Instance.Rotate(mouseX, mouseY);
            PlayerCamera.Instance.TiltHead(dir.x, dir.z);

            if (Input.GetAxis(GlobalConstants.JumpAxis) != 0)
                Player.Instance.Jump();
        }
    }
}
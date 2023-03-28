using UnityEngine;
using PlayerController;

public class SpriteRotator : MonoBehaviour
{
    private Transform _transform;
    private Transform _target;

    private void Start()
    {
        _transform = transform;
        _target = Player.Instance.Mover.Transform;
    }

    private void FixedUpdate()
    {
        Quaternion rotate = Quaternion.LookRotation(_target.forward, Vector3.up);
        _transform.rotation = rotate;
    }
}

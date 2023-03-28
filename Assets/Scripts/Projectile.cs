using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header ("Balance")]
    [SerializeField] protected int _damage;
    [SerializeField] protected float _startSpeed;
    [SerializeField] protected float _gravityImpact;
    [SerializeField] protected float _slowImpact;

    [Header ("Parameters")]
    [SerializeField] protected float _affectedAreaRadius;
    [SerializeField] protected float _detectRadius;
    [SerializeField] protected LayerMask _canBeCollided;
    [SerializeField] protected LayerMask _canBeDamaged;

    protected Transform _transform;
    protected Vector3 _dir;
    protected float _curHorSpeed;
    protected float _curVerSpeed;

    protected void Awake()
    {
        _transform = transform;
    }

    public void Initialize(Vector3 startPoint, Vector3 dir, float lifeTime)
    {
        _transform.position = startPoint;
        _dir = dir;
        _curHorSpeed = _startSpeed * new Vector2(_dir.x, _dir.z).magnitude;
        _curVerSpeed = _startSpeed * _dir.y;

        Invoke("Collision", lifeTime);
    }

    protected void FixedUpdate()
    {
        Vector3 step = new Vector3(_dir.x, 0, _dir.z);
        step = step * _curHorSpeed * Time.fixedDeltaTime;
        step += Vector3.up * _curVerSpeed * Time.fixedDeltaTime;
        _transform.position += step;
            
        _curHorSpeed -= _slowImpact * Time.fixedDeltaTime;
        if (Mathf.Abs(_curVerSpeed) > 8)
            _curVerSpeed = 8 * Mathf.Sign(_curVerSpeed);
        else
            _curVerSpeed += _gravityImpact * Time.fixedDeltaTime;

        if (Physics.OverlapSphere(_transform.position, _detectRadius, _canBeCollided).Length > 0)
            Collision();
    }

    protected virtual void Collision()
    {
        if (gameObject.activeSelf == false) return;
        var colliders = Physics.OverlapSphere(_transform.position, _affectedAreaRadius, _canBeDamaged);
        foreach (var collider in colliders)
            Debug.Log("Hit " + collider.name);

        CancelInvoke();
        gameObject.SetActive(false);
    }

#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectRadius);
    }
#endif
}
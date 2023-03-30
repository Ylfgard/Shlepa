using UnityEngine;
using Enemys;

public abstract class Projectile : MonoBehaviour
{
    [Header ("Parameters")]
    [SerializeField] protected float _detectRadius;
    [SerializeField] protected LayerMask _canBeCollided;
    [SerializeField] protected LayerMask _canBeDamaged;
    [SerializeField] protected float _startSpeed;
    [SerializeField] protected float _speedChanges;

    protected Transform _transform;
    protected Vector3 _dir;
    protected float _curHorSpeed;
    protected float _curVerSpeed;
    protected int _damage;
    
    public LayerMask CanBeCollided => _canBeCollided;
    public LayerMask CanBeDamaged => _canBeDamaged;

    protected void Awake()
    {
        _transform = transform;
    }

    public virtual void Initialize(Vector3 startPoint, Vector3 dir, int damage, float lifeTime)
    {
        _transform.position = startPoint;
        _dir = dir;
        _curHorSpeed = _startSpeed * new Vector2(_dir.x, _dir.z).magnitude;
        _curVerSpeed = _startSpeed * _dir.y;
        _damage = damage;

        Invoke("Collision", lifeTime);
    }

    protected virtual void FixedUpdate()
    {
        Vector3 step = _dir;
        step = step * _curHorSpeed * Time.fixedDeltaTime;
        step += Vector3.up * _curVerSpeed * Time.fixedDeltaTime;
        _transform.position += step;
            
        _curHorSpeed += _speedChanges * Time.fixedDeltaTime;

        if (Physics.OverlapSphere(_transform.position, _detectRadius, _canBeCollided).Length > 0)
            Collision();
    }

    protected abstract void Collision();

#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectRadius);
    }
#endif
}
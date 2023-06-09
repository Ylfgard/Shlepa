using UnityEngine;
using Enemys.Cowers;
using UnityEngine.AI;

namespace Enemys.AIModules
{
    public class TakeCowerModule : MoveModule
    {
        [Header("Cower")]
        [SerializeField] protected float _distanceFromTarget;
        [SerializeField] protected float _changeCowerDistance;

        protected CowerKeeper _cowerKeeper;
        protected Cower _curCower;
        protected bool _moveToCover;
        protected bool _moveToAlternativeCower;
        protected bool _inCower;

        protected void Start()
        {
            _cowerKeeper = CowerKeeper.Instance;
        }

        public override void Initialize(Enemy enemy)
        {
            base.Initialize(enemy);
            enemy.TakedDamage += LeaveCower;
        }

        public override void Move()
        {
            if (_agent.stoppingDistance != 0) _agent.stoppingDistance = 0;
            if (Vector2.Distance(new Vector2(_agent.destination.x, _agent.destination.z),
                new Vector2(_transform.position.x, _transform.position.z)) <= 0.5f) _inCower = true;
            TakeCover();
        }

        protected void TakeCover()
        {
            if (Vector2.Distance(new Vector2(_target.position.x, _target.position.z),
                new Vector2(_transform.position.x, _transform.position.z)) < _changeCowerDistance &&
                _inCower && _moveToAlternativeCower == false) _moveToCover = false;

            if (_moveToCover) return;
            _inCower = false;
            _moveToCover = true;
            Vector3 cowerOffset = (_transform.position - _target.position).normalized * _distanceFromTarget;
            Cower cower = _cowerKeeper.GetNearestShelter(_target.position + cowerOffset);
            if (_curCower != cower)
            {
                NavMeshHit destination;
                NavMesh.SamplePosition(cower.GetCowerPoint(_target.position), out destination, 100, NavMesh.AllAreas);
                _agent.SetDestination(destination.position);
                _curCower = cower;
            }
            else
            {
                cower = _cowerKeeper.GetNearestShelter(_target.position - cowerOffset);
                NavMeshHit destination;
                NavMesh.SamplePosition(cower.GetCowerPoint(_target.position), out destination, 100, NavMesh.AllAreas);
                _agent.SetDestination(destination.position);
                _moveToAlternativeCower = true;
            }

            CheckMove();
        }

        protected void LeaveCower(int healthPercent)
        {
            if (_inCower == false) return;
            _moveToCover = false;
            _moveToAlternativeCower = false;
        }

        public override void MoveToTarget()
        {
            _inCower = false;
            _moveToCover = false;
            _moveToAlternativeCower = false;
            base.MoveToTarget();
        }

#if UNITY_EDITOR
        protected void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _changeCowerDistance);
        }
#endif
    }
}
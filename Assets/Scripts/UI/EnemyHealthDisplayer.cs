using UnityEngine;
using Enemys;
using PlayerController;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class EnemyHealthDisplayer : MonoBehaviour
    {
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private TextMeshProUGUI _enemyName;
        [SerializeField] private TextMeshProUGUI _healthText;
        [SerializeField] private LayerMask _enemyLayer;

        private EnemyKeeper _enemyKeeper;
        private Transform _cameraDir;
        private Enemy _curEnemy;

        private void Awake()
        {
            HideEnemy(_curEnemy);
        }

        private void Start()
        {
            _enemyKeeper = EnemyKeeper.Instance;
            _cameraDir = Player.Instance.Camera.Transform;
        }

        private void FixedUpdate()
        {
            if (Physics.Raycast(_cameraDir.position, _cameraDir.forward, out RaycastHit hit, Mathf.Infinity, _enemyLayer))
            {
                if (_enemyKeeper.TryGetEnemy(hit.collider.gameObject, out Enemy enemy))
                {
                    if (_curEnemy == enemy) return;

                    if (_curEnemy != null)
                    {
                        _curEnemy.TakedDamage -= ChangeHealthBar;
                        _curEnemy.SendDeath -= HideEnemy;
                    }
                    else
                    {
                        _healthSlider.gameObject.SetActive(true);
                    }

                    _curEnemy = enemy;
                    _curEnemy.TakedDamage += ChangeHealthBar;
                    _curEnemy.SendDeath += HideEnemy;

                    ChangeHealthBar(_curEnemy.GetHealthPercent());
                    _enemyName.text = _curEnemy.Name;
                }
            }
        }

        private void ChangeHealthBar(int percent)
        {
            if (_curEnemy == null)
            {
                HideEnemy(_curEnemy);
            }
            else
            {
                _healthSlider.value = _curEnemy.GetHealthPercent();
                _healthText.text = _healthSlider.value.ToString() + "%";
            }
        }

        private void HideEnemy(Enemy enemy)
        {
            if (_curEnemy == null)
            {
                _healthSlider.gameObject.SetActive(false);
                return;
            }

            if (enemy == _curEnemy)
            {
                _curEnemy.TakedDamage -= ChangeHealthBar;
                _curEnemy.SendDeath -= HideEnemy;
                _curEnemy = null;
                _healthSlider.gameObject.SetActive(false);
            }
        }
    }
}
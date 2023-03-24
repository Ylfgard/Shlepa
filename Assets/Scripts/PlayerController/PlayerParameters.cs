using UnityEngine;
using TMPro;

namespace PlayerController
{
    public class PlayerParameters : MonoBehaviour
    {
        [SerializeField] private int _maxHealth;
        [SerializeField] private int _maxArmor;
        [SerializeField] private TextMeshProUGUI _healthText;
        [SerializeField] private TextMeshProUGUI _armorText;
        private int _curHealth;
        private int _curArmor;

        private void Awake()
        {
            _healthText.text = _maxHealth.ToString();
            _curHealth = _maxHealth;
            _curArmor = 0;
            _armorText.text = _curArmor.ToString();
        }

        public void TakeDamage(int damage)
        {
            if (_curHealth + _curArmor > damage)
            {
                if (_curArmor >= damage)
                {
                    _curArmor -= damage;
                }
                else
                {
                    _curHealth -= damage - _curArmor;
                    _curArmor = 0;
                }
            }
            else
            {
                _curHealth = 0;
                _curArmor = 0;
                Debug.Log("Dead");
            }

            _healthText.text = _curHealth.ToString();
            _armorText.text = _curArmor.ToString();
        }
    }
}
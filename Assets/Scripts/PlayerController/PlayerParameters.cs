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

        public int CurHealth => _curHealth;
        public int CurArmor => _curArmor;

        private void Awake()
        {
            _healthText.text = _maxHealth.ToString();
            _curHealth = _maxHealth;
            _curArmor = 0;
            _armorText.text = _curArmor.ToString();
        }

        public void SetParameters(int health, int armor)
        {
            _curHealth = health;
            _healthText.text = _curHealth.ToString();
            _curArmor = armor;
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

        public bool AddHealt(int value)
        {
            if (_curHealth == _maxHealth) return false;
            _curHealth += value;
            if (_curHealth >= _maxHealth) _curHealth = _maxHealth;
            _healthText.text = _curHealth.ToString();
            return true;
        }

        public bool AddArmor(int value)
        {
            if (_curArmor == _maxArmor) return false;
            _curArmor += value;
            if (_curArmor >= _maxArmor) _curArmor = _maxArmor;
            _armorText.text = _curArmor.ToString();
            return true;
        }
    }
}
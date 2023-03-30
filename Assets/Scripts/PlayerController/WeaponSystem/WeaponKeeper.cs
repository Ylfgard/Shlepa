using UnityEngine;
using TMPro;
using System;
using Enemys;

namespace PlayerController.WeaponSystem
{
    public class WeaponKeeper : MonoBehaviour
    {
        private const int WeaponsCount = 6;

        [SerializeField] private GameObject _hitMarker;
        [SerializeField] private Animator _animator;
        [SerializeField] private TextMeshProUGUI _clipText;
        [SerializeField] private TextMeshProUGUI _ammoText;
        [SerializeField] private WeaponSO[] _availableWeapons;
        [SerializeField] private LayerMask _canBeCollided;
        [SerializeField] private LayerMask _canBeDamaged;

        private EnemyKeeper _enemyKeeper;
        private Weapon[] _weapons;
        private int _curWIndx;
        private Action _updateCall;

        public Weapon CurWeapon => _weapons[_curWIndx];

        private void Start()
        {
            _enemyKeeper = EnemyKeeper.Instance;

            _weapons = new Weapon[WeaponsCount];
            foreach (var weapon in _availableWeapons)
            {
                if (_weapons[weapon.SlotIndex - 1] != null)
                {
                    Debug.LogError("Error! Slot " + weapon.SlotIndex + " is taken!");
                    continue;
                }

                if (weapon.Projectile != null)
                {
                    _weapons[weapon.SlotIndex - 1] = new WeaponProjectile(_canBeCollided, _canBeDamaged, _animator, _enemyKeeper,
                        _hitMarker, _clipText, _ammoText, weapon);
                    continue;
                }

                if (weapon.DispersionSpeed > 0)
                {
                    var weaponAuto = new WeaponAuto(_canBeCollided, _canBeDamaged, _animator, _enemyKeeper,
                        _hitMarker, _clipText, _ammoText, weapon);
                    _updateCall += weaponAuto.UpdateDisperion;
                    _weapons[weapon.SlotIndex - 1] = weaponAuto;
                    continue;
                }

                _weapons[weapon.SlotIndex - 1] = new Weapon(_canBeCollided, _canBeDamaged, _animator, _enemyKeeper,
                    _hitMarker, _clipText, _ammoText, weapon);
            }

            _curWIndx = -1;
            ChangeWeapon(_availableWeapons[0].SlotIndex);
        }

        private void FixedUpdate()
        {
            _updateCall?.Invoke();
        }

        public async void ChangeWeapon(int slotNumber)
        {
            if (slotNumber <= 0 || slotNumber > WeaponsCount) return;
            if (_curWIndx == slotNumber - 1 || _weapons[slotNumber - 1] == null) return;

            if (_curWIndx != -1) await _weapons[_curWIndx].PutAway();
            _curWIndx = slotNumber - 1;
            await _weapons[_curWIndx].TakeInHand();
        }

        public void ReloadEnded()
        {
            CurWeapon.EndReloading();
        }
    }
}
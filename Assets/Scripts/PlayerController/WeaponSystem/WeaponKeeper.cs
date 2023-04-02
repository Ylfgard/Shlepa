using UnityEngine;
using TMPro;
using System;
using Enemys;
using System.Collections;

namespace PlayerController.WeaponSystem
{
    public class WeaponKeeper : MonoBehaviour
    {
        private const int WeaponsCount = 6;

        [SerializeField] private GameObject _hitMarker;
        [SerializeField] private Animator _animator;
        [SerializeField] private TextMeshProUGUI _clipText;
        [SerializeField] private TextMeshProUGUI _ammoText;
        [SerializeField] private WeaponData[] _availableWeapons;
        [SerializeField] private LayerMask _canBeCollided;
        [SerializeField] private LayerMask _canBeDamaged;

        private EnemyKeeper _enemyKeeper;
        private Weapon[] _weapons;
        private int _curWIndx;
        private Action _updateCall;
        private bool _weaponReady;

        private void Awake()
        {
            _enemyKeeper = EnemyKeeper.Instance;

            _weapons = new Weapon[WeaponsCount];
            foreach (var availableWeapon in _availableWeapons)
            {
                WeaponSO weapon = availableWeapon.Weapon;
                if (_weapons[weapon.SlotIndex - 1] != null)
                {
                    Debug.LogError("Error! Slot " + weapon.SlotIndex + " is taken!");
                    continue;
                }

                if (weapon.Projectile != null)
                {
                    _weapons[weapon.SlotIndex - 1] = new WeaponProjectile(_canBeCollided, _canBeDamaged, _animator, _enemyKeeper,
                        _hitMarker, _clipText, _ammoText, availableWeapon.StartAmmo, weapon);
                    continue;
                }

                if (weapon.DispersionSpeed > 0)
                {
                    var weaponAuto = new WeaponAuto(_canBeCollided, _canBeDamaged, _animator, _enemyKeeper,
                        _hitMarker, _clipText, _ammoText, availableWeapon.StartAmmo, weapon);
                    _updateCall += weaponAuto.UpdateDisperion;
                    _weapons[weapon.SlotIndex - 1] = weaponAuto;
                    continue;
                }

                _weapons[weapon.SlotIndex - 1] = new Weapon(_canBeCollided, _canBeDamaged, _animator, _enemyKeeper,
                    _hitMarker, _clipText, _ammoText, availableWeapon.StartAmmo, weapon);
            }

            _curWIndx = -1;
            _weaponReady = true;
            StartCoroutine(ChangeWeapon(_availableWeapons[0].Weapon.SlotIndex));
        }

        private void FixedUpdate()
        {
            _updateCall?.Invoke();
        }

        public void MakeShot(Transform dir)
        {
            if (_weaponReady == false) return;
            _weapons[_curWIndx].Shot(dir);
        }

        public void Reload()
        {
            if (_weaponReady == false) return;
            _weapons[_curWIndx].Reload();
        }

        public float GetWeaponAimValue()
        {
            return _weapons[_curWIndx].AimValue;
        }

        public void TryChangeWeapon(int slotNumber)
        {
            if (_weaponReady == false || slotNumber <= 0 || slotNumber > WeaponsCount) return;
            if (_curWIndx == slotNumber - 1 || _weapons[slotNumber - 1] == null) return;
            StartCoroutine(ChangeWeapon(slotNumber));
        }

        private IEnumerator ChangeWeapon(int slotNumber)
        {
            _weaponReady = false;
            if (_curWIndx != -1)
            {
                yield return new WaitForSeconds(0.3f);
                _weapons[_curWIndx].PutAway();
            }

            _curWIndx = slotNumber - 1;
            yield return new WaitForSeconds(0.3f);
            _weapons[_curWIndx].TakeInHand();
            _weaponReady = true;
        }

        public void AddAmmo(int weaponSlot, int value)
        {
            if (weaponSlot <= 0 || weaponSlot >= WeaponsCount) return;
            if (_weapons[weaponSlot] == null) return;

            _weapons[weaponSlot].AddAmmo(value);
        }

        public void ReloadEnded()
        {
            _weapons[_curWIndx].EndReloading();
        }
    }

    [Serializable]
    public class WeaponData
    {
        [SerializeField] private WeaponSO _weapon;
        [SerializeField] private int _startAmmo;

        public WeaponSO Weapon => _weapon;
        public int StartAmmo => _startAmmo;
    }
}
using UnityEngine;
using TMPro;
using System;
using Enemys;
using System.Collections;
using VFX;

namespace PlayerController.WeaponSystem
{
    public class WeaponKeeper : MonoBehaviour
    {
        private const string WalkStateName = "Walk";
        private const int WeaponsCount = 6;

        [SerializeField] private float _changeWeaponTime;
        [SerializeField] private GameObject _hitGroundPrefab;
        [SerializeField] private GameObject _hitEnemyPrefab;
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
        private ObjectPool<HitMarker> _hitGround;
        private ObjectPool<HitMarker> _hitEnemy;

        private void Start()
        {
            _enemyKeeper = EnemyKeeper.Instance;
            _hitGround = new ObjectPool<HitMarker>(_hitGroundPrefab);
            _hitEnemy = new ObjectPool<HitMarker>(_hitEnemyPrefab);
            _hitGround.PreSpawn(30);
            _hitEnemy.PreSpawn(30);

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
                        _hitGround, _hitEnemy, _clipText, _ammoText, availableWeapon.StartAmmo, weapon);
                    continue;
                }

                if (weapon.DispersionSpeed > 0)
                {
                    var weaponAuto = new WeaponAuto(_canBeCollided, _canBeDamaged, _animator, _enemyKeeper,
                        _hitGround, _hitEnemy, _clipText, _ammoText, availableWeapon.StartAmmo, weapon);
                    _updateCall += weaponAuto.UpdateDisperion;
                    _weapons[weapon.SlotIndex - 1] = weaponAuto;
                    continue;
                }

                _weapons[weapon.SlotIndex - 1] = new Weapon(_canBeCollided, _canBeDamaged, _animator, _enemyKeeper,
                    _hitGround, _hitEnemy, _clipText, _ammoText, availableWeapon.StartAmmo, weapon);
            }

            _curWIndx = -1;
            _weaponReady = true;
            StartCoroutine(ChangeWeapon(_availableWeapons[0].Weapon.SlotIndex));
        }

        private void FixedUpdate()
        {
            _updateCall?.Invoke();
        }

        public int[] GetAmmos()
        {
            int[] ammos = new int[_weapons.Length - 1];
            for (int i = 1; i <= ammos.Length; i++)
                ammos[i - 1] = _weapons[i].Ammos + _weapons[i].BulletsInClip;

            return ammos;
        }

        public void SetAmmos(int[] ammos)
        {
            for (int i = 1; i <= ammos.Length; i++)
            {
                if (i == _curWIndx) _weapons[i].SetAmmos(ammos[i - 1], true);
                else _weapons[i].SetAmmos(ammos[i - 1], false);
            }
        }

        public float MakeShot(Transform dir)
        {
            if (_weaponReady == false) return 0;
            else return _weapons[_curWIndx].Shot(dir);
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

        public void ScrollWeapon(bool next)
        {
            if (next)
            {
                if (_curWIndx < _weapons.Length - 1)
                    TryChangeWeapon(_curWIndx + 2);
                else
                    TryChangeWeapon(1);
            }
            else
            {
                if (_curWIndx > 0)
                    TryChangeWeapon(_curWIndx);
                else
                    TryChangeWeapon(_weapons.Length);
            }
        }

        private IEnumerator ChangeWeapon(int slotNumber)
        {
            _weaponReady = false;
            if (_curWIndx != -1)
            {
                yield return new WaitForSeconds(_changeWeaponTime);
                _weapons[_curWIndx].PutAway();
            }

            _curWIndx = slotNumber - 1;
            yield return new WaitForSeconds(_changeWeaponTime);
            _weapons[_curWIndx].TakeInHand();
            _weaponReady = true;
        }

        public void AddAmmo(int weaponSlot, int value)
        {
            if (weaponSlot <= 0 || weaponSlot >= WeaponsCount) return;
            if (_weapons[weaponSlot] == null) return;

            if (weaponSlot == _curWIndx) _weapons[weaponSlot].AddAmmos(value, true);
            else _weapons[weaponSlot].AddAmmos(value, false);
        }

        public void SetWalkAnim(bool state)
        {
            if (_animator.GetBool(WalkStateName) != state)
                _animator.SetBool(WalkStateName, state);
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
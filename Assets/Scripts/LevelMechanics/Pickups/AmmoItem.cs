using UnityEngine;

namespace LevelMechanics.Pickups
{
    public class AmmoItem : PickupItems
    {
        [SerializeField] protected AmmoType _type;
        [SerializeField] protected int _amount;

        protected override void Pickup()
        {
            int slot = 0;
            switch (_type)
            {
                case AmmoType.Revolver:
                    slot = 1;
                    break;
                case AmmoType.Shotgun:
                    slot = 2;
                    break;
                case AmmoType.AK47:
                    slot = 3;
                    break;
                case AmmoType.GrenadeLauncher:
                    slot = 4;
                    break;
                case AmmoType.Rifle:
                    slot = 5;
                    break;
                default:
                    Debug.LogError("Wrong ammo type!" + _type);
                    break;
            }
            if (slot == 0) return;
            _player.WeaponKeeper.AddAmmo(slot, _amount);
            gameObject.SetActive(false);
        }
    }

    public enum AmmoType
    { 
        Revolver,
        Shotgun,
        AK47,
        GrenadeLauncher,
        Rifle
    }
}
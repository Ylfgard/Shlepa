using UnityEngine;

namespace LevelMechanics.Pickups
{
    public class MedicineItem : PickupItems
    {
        [SerializeField] protected MedicineType _type;
        [SerializeField] protected int _amount;

        protected override void Pickup()
        {
            switch(_type)
            {
                case MedicineType.Health:
                    if (_player.Parameters.AddHealt(_amount))
                        gameObject.SetActive(false);
                    break;
                case MedicineType.Armor:
                    if (_player.Parameters.AddArmor(_amount))
                        gameObject.SetActive(false);
                    break;
                default:
                    Debug.LogError("Wrong medicine type! " + _type);
                    break;
            }
        }
    }

    public enum MedicineType
    {
        Health,
        Armor
    }
}
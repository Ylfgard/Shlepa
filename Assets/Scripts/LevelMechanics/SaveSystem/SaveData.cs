using System;

namespace LevelMechanics.SaveSystem
{
    [Serializable]
    public class SaveData
    {
        protected int _health;
        protected int _armor;
        protected int[] _ammos;

        public int Health => _health;
        public int Armor => _armor;
        public int[] Ammos => _ammos;

        public SaveData(int health, int armor, int[] ammos)
        {
            _health = health;
            _armor = armor;
            _ammos = ammos;
        }
    }
}
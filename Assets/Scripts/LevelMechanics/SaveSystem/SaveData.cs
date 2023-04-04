using System;

namespace LevelMechanics.SaveSystem
{
    [Serializable]
    public class SaveData
    {
        private int _checkpointIndex;
        private int _health;
        private int _armor;
        private int[] _ammos;

        public int CheckpointIndex => _checkpointIndex;
        public int Health => _health;
        public int Armor => _armor;
        public int[] Ammos => _ammos;

        public SaveData(int checkpointIndex, int health, int armor, int[] ammos)
        {
            _checkpointIndex = checkpointIndex;
            _health = health;
            _armor = armor;
            _ammos = ammos;
        }
    }
}
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using PlayerController;
using System.Runtime.Serialization.Formatters.Binary;

namespace LevelMechanics.SaveSystem
{
    public static class LevelSaver
    {
        public static void Save()
        {
            PlayerPrefs.SetString(PlayerPrefsKeeper.SceneKey, SceneManager.GetActiveScene().name);
            PlayerPrefs.SetInt(PlayerPrefsKeeper.CheckpointKey, -1);

            Player player = Player.Instance;

            SaveData save = new SaveData(player.Parameters.CurHealth,
                player.Parameters.CurArmor, player.WeaponKeeper.GetAmmos());

            BinaryFormatter formatter = new BinaryFormatter();

            string path = Application.dataPath + "/Saves/level.save";
            FileStream stream = new FileStream(path, FileMode.Create);
            formatter.Serialize(stream, save);
            stream.Close();
        }

        public static void Save(int checkpointIndex)
        {
            PlayerPrefs.SetString(PlayerPrefsKeeper.SceneKey, SceneManager.GetActiveScene().name);
            PlayerPrefs.SetInt(PlayerPrefsKeeper.CheckpointKey, checkpointIndex);

            Player player = Player.Instance;

            SaveData save = new SaveData(player.Parameters.CurHealth,
                player.Parameters.CurArmor, player.WeaponKeeper.GetAmmos());

            BinaryFormatter formatter = new BinaryFormatter();
            
            string path = Application.dataPath + "/Saves/checkpoint.save";
            FileStream stream = new FileStream(path, FileMode.Create);
            formatter.Serialize(stream, save);
            stream.Close();
        }
    }
}
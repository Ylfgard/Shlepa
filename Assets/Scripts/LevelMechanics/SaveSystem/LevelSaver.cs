using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using PlayerController;
using System.Runtime.Serialization.Formatters.Binary;

namespace LevelMechanics.SaveSystem
{
    public static class LevelSaver
    {
        public static void SaveLevel(int checkpointIndex)
        {
            PlayerPrefs.SetString(PlayerPrefsKeeper.SceneKey, SceneManager.GetActiveScene().name);
            Player player = Player.Instance;

            SaveData save = new SaveData(checkpointIndex, player.Parameters.CurHealth,
                player.Parameters.CurArmor, player.WeaponKeeper.GetAmmos());

            BinaryFormatter formatter = new BinaryFormatter();

            string path = Application.persistentDataPath + "/save.data";
            FileStream stream = new FileStream(path, FileMode.Create);
            formatter.Serialize(stream, save);
            stream.Close();
        }
    }
}
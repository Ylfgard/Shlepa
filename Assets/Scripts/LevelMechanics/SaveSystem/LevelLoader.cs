using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using PlayerController;
using UnityEngine.SceneManagement;

namespace LevelMechanics.SaveSystem
{ 
    public class LevelLoader : MonoBehaviour
    {
        private void Start()
        {
            LoadLevel();
        }

        private void LoadLevel()
        {
            BinaryFormatter formatter = new BinaryFormatter();

            string path = Application.dataPath + "/Saves/save.data";
            FileStream stream = new FileStream(path, FileMode.Open);
            SaveData save = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            Player.Instance.Load(save);
        }

        public void LoadScene()
        {
            string scene = PlayerPrefs.GetString(PlayerPrefsKeeper.SceneKey);
            SceneManager.LoadScene(scene);
        }
    }
}
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using PlayerController;
using UnityEngine.SceneManagement;

namespace LevelMechanics.SaveSystem
{ 
    public class LevelLoader : MonoBehaviour
    {
        private const string FirstLevelName = "Level_1";

        [SerializeField] private bool _dontPreLoad;

        private int _checkpointIndex;

        // Singleton
        private static LevelLoader _instance;
        public static LevelLoader Instance => _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            else
                _instance = this;
        }

        private void Start()
        {
            _checkpointIndex = PlayerPrefs.GetInt(PlayerPrefsKeeper.CheckpointKey);
            if (_dontPreLoad)
            {
                if (_checkpointIndex == -1)
                    LevelSaver.Save();
                else
                    LoadLevel();
                return;
            }
            else
            {
                LoadLevel();
                if (_checkpointIndex == -1)
                    LevelSaver.Save();
            }
        }

        private void LoadLevel()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path;
            if (_checkpointIndex == -1)
                path = Application.dataPath + "/Saves/level.save";
            else
                path = Application.dataPath + "/Saves/checkpoint.save";
            FileStream stream = new FileStream(path, FileMode.Open);
            SaveData save = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            if (_checkpointIndex == -1)
                Player.Instance.Load(save);
            else
                Player.Instance.Load(save, _checkpointIndex);
        }

        public void LoadScene(bool loadCheckpoint)
        {
            string scene = PlayerPrefs.GetString(PlayerPrefsKeeper.SceneKey);
            if (loadCheckpoint == false) PlayerPrefs.SetInt(PlayerPrefsKeeper.CheckpointKey, -1);
            SceneManager.LoadScene(scene);
        }

        public void LoadFisrtLevel()
        {
            string scene = FirstLevelName;
            PlayerPrefs.SetInt(PlayerPrefsKeeper.CheckpointKey, -1);
            SceneManager.LoadScene(scene);
        }
    }
}
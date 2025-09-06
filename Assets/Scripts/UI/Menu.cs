using UnityEngine;
using LevelMechanics.SaveSystem;

namespace UI
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] private GameObject _body;

        // Singleton
        private static Menu _instance;
        public static Menu Instance => _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            else
                _instance = this;
        }

        private void Start()
        {
            GameTimeChanger.PlayTime();
            _body.SetActive(false);
        }

        public void ChangeMenuActive()
        {
            if (_body.activeSelf)
            {
                GameTimeChanger.PlayTime();
                _body.SetActive(false);
            }
            else
            {
                GameTimeChanger.StopTime();
                _body.SetActive(true);
            }
        }

        public void OpenSettings()
        {
            Debug.Log("Open settings");
        }

        public void LoadCheckPoint()
        {
            LevelLoader.Instance.LoadScene(true);
        }

        public void ReloadLevel()
        {
            LevelLoader.Instance.LoadScene(false);
        }

        public void StartNewGame()
        {
            LevelLoader.Instance.LoadFisrtLevel();
        }

        public void OpenMainMenu()
        {
            Debug.Log("Open main menu");
            Application.Quit();
        }
    }
}
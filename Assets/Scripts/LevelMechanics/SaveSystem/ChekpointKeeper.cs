using UnityEngine;

namespace LevelMechanics.SaveSystem
{
    public class ChekpointKeeper : MonoBehaviour
    {
        [SerializeField] private Checkpoint[] _checkpoints;

        // Singleton
        private static ChekpointKeeper _instance;
        public static ChekpointKeeper Instance => _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            else
                _instance = this;

            for (int i = 0; i < _checkpoints.Length; i++)
                _checkpoints[i].Initialize(i);
        }

        public Vector3 GetCheckpointPosition(int index)
        {
            Vector3 position = _checkpoints[index].GetPosition();
            return position;
        }
    }
}
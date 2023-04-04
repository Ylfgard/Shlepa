using UnityEngine;
using LevelMechanics.ActivatableObjects;

namespace LevelMechanics.SaveSystem
{
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField] private ActivatableObject[] _activatableObjects;

        private int _checkpointIndex;

        public void Initialize(int checkpointIndex)
        {
            _checkpointIndex = checkpointIndex;
        }

        public Vector3 GetPosition()
        {
            foreach (ActivatableObject obj in _activatableObjects)
                obj.Activate();
            gameObject.SetActive(false);
            return transform.position;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == TagsKeeper.Player)
            {
                LevelSaver.SaveLevel(_checkpointIndex);
                gameObject.SetActive(false);
            }
        }
    }
}
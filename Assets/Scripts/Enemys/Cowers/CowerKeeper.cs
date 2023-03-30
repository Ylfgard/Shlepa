using UnityEngine;

namespace Enemys.Cowers
{
    public class CowerKeeper : MonoBehaviour
    {
        private Cower[] _shelters;

        // Singleton
        private static CowerKeeper _instance;
        public static CowerKeeper Instance => _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
                Destroy(gameObject);
            else
                _instance = this;

            _shelters = FindObjectsOfType<Cower>();
        }

        public Cower GetNearestShelter(Vector3 position)
        {
            Cower result = _shelters[0];
            for (int i = 1; i < _shelters.Length; i++)
            {
                if (Vector3.Distance(_shelters[i].Transform.position, position) < 
                    Vector3.Distance(result.Transform.position, position))
                    result = _shelters[i];
            }

            return result;
        }
    }
}
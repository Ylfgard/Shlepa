using UnityEngine;
using System.Collections;

namespace VFX
{
    public class HitMarker : MonoBehaviour
    {
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        public void Initialize(Vector3 position, Quaternion quaternion, float showTime)
        {
            _transform.position = position;
            _transform.rotation = quaternion;
            StartCoroutine(DelayedDisable(showTime));
        }

        private IEnumerator DelayedDisable(float delay)
        {
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
        }
    }
}
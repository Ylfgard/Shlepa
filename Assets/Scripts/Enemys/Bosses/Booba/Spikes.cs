using UnityEngine;

namespace Enemys.Bosses
{
    public class Spikes : MonoBehaviour
    {
        [SerializeField] private AnimationController _controller;

        public AnimationController Controller => _controller;
    }
}
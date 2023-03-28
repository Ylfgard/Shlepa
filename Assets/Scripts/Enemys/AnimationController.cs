using UnityEngine;
using System;

namespace Enemys
{ 
    public class AnimationController : MonoBehaviour
    {
        public Action CallBack;
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void SetTrigger(string trigger)
        {
            _animator.SetTrigger(trigger);
        }

        public void ActivateEvent()
        {
            CallBack?.Invoke();
        }    
    }
}
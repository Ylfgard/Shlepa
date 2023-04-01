using UnityEngine;

namespace LevelMechanics.ActivatableObjects
{
    public abstract class ActivatableObject : MonoBehaviour
    {
        public abstract void Activate();

        public abstract void Deactivate();
    }
}
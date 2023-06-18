using UnityEngine;

namespace LevelMechanics.ActivatableObjects
{
    public class InvisibleWall : ActivatableObject
    {
        public override void Activate()
        {
            gameObject.SetActive(true);
        }

        public override void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}
using UnityEngine;

namespace Enemys.AIModules
{
    public abstract class AIModule : MonoBehaviour
    {
        public virtual void Deactivate(Enemy enemy)
        {
            StopAllCoroutines();
        }
    }
}
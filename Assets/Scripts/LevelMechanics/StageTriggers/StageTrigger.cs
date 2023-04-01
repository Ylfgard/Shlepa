using UnityEngine;

namespace LevelMechanics
{
    public abstract class StageTrigger : MonoBehaviour
    {
        protected StagesKeeper _stagesKeeper;

        protected void Start()
        {
            _stagesKeeper = StagesKeeper.Instance;
        }

        protected abstract void ActivateStage();
    }
}
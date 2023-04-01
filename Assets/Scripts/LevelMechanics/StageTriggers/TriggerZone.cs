using UnityEngine;

namespace LevelMechanics.StageTriggers
{
    public class TriggerZone : StageTrigger
    {
        [SerializeField] protected int _stageIndex;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == TagsKeeper.Player)
                ActivateStage();
        }

        protected override void ActivateStage()
        {
            _stagesKeeper.ActivateStage(_stageIndex, false);
            gameObject.SetActive(false);
        }
    }
}
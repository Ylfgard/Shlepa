using UnityEngine;
using System;
using System.Collections;

namespace LevelMechanics.StageTriggers
{
    public class WaveSpawner : StageTrigger
    {
        [SerializeField] protected WaveData[] _wavesData;

        protected Collider _triggerZone;
        protected int _curWave;
        protected int _clearPercent;

        private void Awake()
        {
            _triggerZone = GetComponent<Collider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == TagsKeeper.Player)
            {
                _triggerZone.enabled = false;
                _curWave = 0;
                ActivateStage();
            }
        }

        protected override void ActivateStage()
        {
            if (_curWave >= _wavesData.Length)
            {
                _stagesKeeper.EndStage();
                gameObject.SetActive(false);
            }
            else
            {
                if (_wavesData[_curWave].TimeDelay == 0)
                {
                    _clearPercent = _wavesData[_curWave].EnemyClearPercent;
                    _stagesKeeper.SendClearPercent += ClearPercent;
                    _stagesKeeper.ActivateStage(_wavesData[_curWave].Index, true);
                }
                else
                {
                    StartCoroutine(DelayedWaveActivation(_wavesData[_curWave].TimeDelay));
                    _stagesKeeper.ActivateStage(_wavesData[_curWave].Index, true);
                }
                _curWave++;
            }
        }

        protected void ClearPercent(int percent)
        {
            if (percent >= _clearPercent)
            {
                _stagesKeeper.SendClearPercent -= ClearPercent;
                ActivateStage();
            }
        }

        protected IEnumerator DelayedWaveActivation(int time)
        {
            yield return new WaitForSeconds(time);
            ActivateStage();
        }
    }

    [Serializable]
    public class WaveData
    {
        [SerializeField] private int _index;
        [SerializeField] private int _timeDelay;
        [SerializeField] private int _enemyClearPercent;

        public int Index => _index;
        public int TimeDelay => _timeDelay;
        public int EnemyClearPercent => _enemyClearPercent;
    }
}
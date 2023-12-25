using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAnim : MonoBehaviour
{
    [ContextMenuItem ("Get current pos", "GetCurrentPosition")]
    [SerializeField] private Vector3 _endPos;

    [SerializeField] private bool _cycleAnim;
    [SerializeField] private int _lenghtTime;
    [SerializeField, Range(0,1)] private float _offsetTime;

    private Vector3 _startPos;

    private float _currTime;
    private float _percentTime;

    private void Start()
    {
        _currTime = Mathf.Lerp(0, _lenghtTime, _offsetTime);
        _startPos = transform.position;
    }

    private void Update()
    {
        if (_cycleAnim)
        {
            if (_currTime < _lenghtTime * 2)
            {
                _currTime += Time.deltaTime;
                _percentTime = _currTime / _lenghtTime;

                if(_currTime > _lenghtTime)
                {
                    _percentTime = 1-((_currTime / _lenghtTime)-1);
                }
            }
            else
            {
                _currTime = 0;
            }
        }
        else
        {
            if (_currTime < _lenghtTime)
            {
                _currTime += Time.deltaTime;
            }
            else
            {
                _currTime = 0;
            }
            _percentTime = _currTime / _lenghtTime;
        }
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(_startPos, _endPos, _percentTime);
    }

    //Only for inspector uses
    private void GetCurrentPosition()
    {
        _endPos = transform.position;
    }
}

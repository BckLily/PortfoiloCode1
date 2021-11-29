using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkEffectCtrl : MonoBehaviour
{
    private float removeTime;
    private float _time;

    void Start()
    {
        removeTime = 1.5f;
    }

    private void OnEnable()
    {
        _time = 0f;
    }

    void Update()
    {
        if (_time >= removeTime)
        {
            _time = 0f;
            PlayerEffectCtrl.ReturnSparkEffect(this);
        }
        else
        {
            _time += Time.deltaTime;
        }
    }
}

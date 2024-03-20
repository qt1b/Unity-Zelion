using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AbstractBar : MonoBehaviour
{

    // abstract classe des bars, a modifier pour modifier tout les comportementes des bars

    public Slider slider;
    public Slider damageBar;
    public float speed = 0.0125f;
    public float maxValue = 100f;
    public float curValue;


    // Start is called before the first frame update
    void Start()
    {
    curValue = maxValue;   
    }

    // Update is called once per frame
    public void Update()
    {
        if(curValue < 0){
            //
            curValue = 0;
        }
        if(slider.value != curValue){
            slider.value = curValue;
        }
        
        if(slider.value != damageBar.value){
            damageBar.value = Mathf.Lerp(damageBar.value, curValue, speed);
        }

    }

    public void TakeDamages(float damages){
        curValue -= damages;
    }
}

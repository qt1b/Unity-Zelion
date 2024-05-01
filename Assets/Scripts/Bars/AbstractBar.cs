using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AbstractBar : MonoBehaviour, IHealth
{

    // abstract classe des bars, a modifier pour modifier tout les comportementes des bars

    public Slider slider;
    public Slider damageBar;
    public float speed = 0.0125f;
    public uint maxValue = 100;
    protected uint curValue;


    // Start is called before the first frame update
    void Start()
    {
        curValue = maxValue;
    }

    // Update is called once per frame
    public void Update()
    {
        /* if(curValue < 0) { // not needed anymore
            curValue = 0;
        } */
        slider.value = curValue;
        
        if(Math.Abs(slider.value - damageBar.value) > 0.01){
            damageBar.value = Mathf.Lerp(damageBar.value, curValue, speed);
        }

    }

    public uint GetValue() {
        return curValue;
    }

    // not to be used with health ! in this case, it should be a game over
    public bool CanTakeDamages(uint damages) => damages <= curValue;

    // same
    public bool TryTakeDamages(uint damages) {
        if (damages <= curValue) {
            TakeDamages(damages);
            return true;
        }
        else return false;
    }

    public void TakeDamages(uint damages){
        // value should be checked to be positive, or else use a uint
        if (damages >= curValue)
            curValue = 0;
        else curValue -= damages;
        print($"curvalue={curValue}");
    }

    public void Heal(uint heal) {
        if (heal + curValue >= maxValue)
            curValue = maxValue;
        else curValue += heal;
    }
}

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
    private bool hasChanged;
    protected uint maxValue;
    protected uint curValue;

    // may be what causes unity to crash ??? but why ??
    /* public AbstractBar(uint maxvalue) {
        maxValue = maxValue;
    } */

    public void Start() {
        maxValue = (uint)slider.maxValue;
        curValue = maxValue;
        hasChanged = true;
    }

    // Update is called once per frame
    public void Update()
    {
        /* if(curValue < 0) { // not needed anymore
            curValue = 0;
        } */
        if (hasChanged) {
            slider.value = curValue;
            hasChanged = false;
        }
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
        hasChanged = true;
        if (damages >= curValue)
            curValue = 0;
        else curValue -= damages;
    }

    public void Heal(uint heal) {
        hasChanged = true;
        if (heal + curValue >= maxValue)
            curValue = maxValue;
        else curValue += heal;
    }

    public void ChangeMaxValue(uint newMax) {
        if (newMax < maxValue) {
            curValue = Math.Clamp(curValue, 0, newMax);
            hasChanged = true;
        }
        maxValue = newMax;
        slider.maxValue = maxValue;
        damageBar.maxValue = maxValue;
    }
}

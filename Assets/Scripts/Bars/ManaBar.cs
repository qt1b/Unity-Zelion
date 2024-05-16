using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaBar : AbstractBar {
    
    private bool _gain;

    new void Start() {
        base.Start();
        // maxValue = 40;
        _gain = true;
    }

    new void Update() 
    {
        base.Update();
        if(_gain && !IsMax){
            StartCoroutine(Gain());
        }
    }
    
    IEnumerator Gain(){
        base.Heal(1);
        _gain = false;
        yield return new WaitForSeconds(1f);
        _gain = true;
    }
}

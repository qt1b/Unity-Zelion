using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaBar : AbstractBar {
    
    private bool gain;

    new void Start() {
        base.Start();
        // maxValue = 40;
        gain = true;
    }

    new void Update() 
    {
        base.Update();
        if(gain && !IsMax){
            StartCoroutine(Gain());
        }
    }
    
    IEnumerator Gain(){
        base.Heal(1);
        gain = false;
        yield return new WaitForSeconds(1f);
        gain = true;
    }
}

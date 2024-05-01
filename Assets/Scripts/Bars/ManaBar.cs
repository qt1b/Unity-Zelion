using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaBar : AbstractBar {
    
    private bool gain;

    private void Start() {
        maxValue = 40;
        gain = true;
    }

    new void Update() 
    {
        base.Update();
        if(gain && curValue < maxValue){
            StartCoroutine(Gain());
        }
    }
    
    IEnumerator Gain(){
        curValue += 1;
        gain = false;
        yield return new WaitForSeconds(1f);
        gain = true;
    }
}

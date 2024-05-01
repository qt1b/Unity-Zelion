using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaBar : AbstractBar {
    private bool gain = true;
    void Start() {
        maxValue = 40;
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
        yield return new WaitForSeconds(0.5f);
        gain = true;
    }
}

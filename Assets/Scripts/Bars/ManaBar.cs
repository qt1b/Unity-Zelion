using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaBar : AbstractBar {
    
    ManaBar() : base(40) { }
    private bool gain;
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

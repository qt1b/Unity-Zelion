using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaBar : AbstractBar {
    private bool gain = true;
    new void Start() {
        base.Start();
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
        yield return new WaitForSeconds(0.5f);
        gain = true;
    }
}

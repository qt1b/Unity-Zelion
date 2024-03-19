using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaBar : AbstractBar
{
    bool damagedash = false;
    bool gain = true;

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        if(Player.isDashing && !damagedash){
            StartCoroutine(Dashing());
        }
        if(gain && curValue < maxValue){
            StartCoroutine(Gain());
        }
    }

    IEnumerator Dashing(){
        TakeDamages(10);
        damagedash = true;
        yield return new WaitForSeconds(1f);
        damagedash = false;
    }
    IEnumerator Gain(){
        curValue += 1;
        gain = false;
        yield return new WaitForSeconds(0.5f);
        gain = true;
    }


}

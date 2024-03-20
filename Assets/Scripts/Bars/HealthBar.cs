using System;
using System.Collections;
using System.Collections.Generic;
// using UnityEditor.PackageManager; // can't compile with it (for now)
using UnityEngine;

public class HealthBar : AbstractBar
{

    // Update is called once per frame
    new void Update() 
    {
        base.Update(); // utilise l'update de la abstract class

        // appuyer sur m en qwerty pour prendre des dégats (demo a modifier plus tard pour que ce soit avec les attaques ennemies)
        if(Input.GetKeyDown(KeyCode.M)){
            TakeDamages(10);
        }
    }
}

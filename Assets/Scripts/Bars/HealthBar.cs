using System;
using UnityEngine;

namespace Bars {
    public class HealthBar : AbstractBar {
        private new void Start() {
            base.Start();
            RestoreDelay = 0f;
        }
        // to delete
        new void Update() {
            base.Update(); // utilise l'update de la abstract class
            // appuyer sur m en qwerty pour prendre des dégats (demo a modifier plus tard pour que ce soit avec les attaques ennemies)
            if (Input.GetKeyDown(KeyCode.M)) {
                TakeDamages(2);
            }
        }
    }
}
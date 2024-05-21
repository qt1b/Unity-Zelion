using System;
using UnityEngine;

namespace Bars {
    public class HealthBar : AbstractBar {
        private new void Start() {
            base.Start();
        }

        // Update is called once per frame
        new void Update() {
            base.Update(); // utilise l'update de la abstract class

            // appuyer sur m en qwerty pour prendre des dégats (demo a modifier plus tard pour que ce soit avec les attaques ennemies)
            if (Input.GetKeyDown(KeyCode.M)) {
                TakeDamages(2);
            }
        }

        // custom takeDamages to call the gameover
        public new void TakeDamages(uint damages) {
            if (!base.TryTakeDamages(damages)) {
                // Application.Quit(); // WARNING : To remove !!!
                throw new NotImplementedException("GameOver Condition Reached");
            }
        }
    }
}
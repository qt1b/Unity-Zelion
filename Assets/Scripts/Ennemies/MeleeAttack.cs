using System.Collections;
using UnityEngine;

namespace Ennemies {
    public class MeleeAttack : MonoBehaviour {
        public IEnumerator Attack()
        {
            print("Attacked");
            yield break;
        }
    }
}

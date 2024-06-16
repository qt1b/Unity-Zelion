using System.Collections.Generic;
using Photon.PhotonUnityNetworking.Code;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Ennemies {
    public class CollectibleDrop : MonoBehaviour {

        private static string[] _collectibles = new[] {
            "Heart", "ManaOrb",
            "StaminaOrb"
        };

        // for every 16hp of every enemy, we get an orb
        // could be a little more smooth if elements were to go progressively to their place
        public static void Activate(uint value,Vector3 position) {
            uint nbr = (value/16) +1; // kind of a 'roof + 1'
            for (int i = 0; i < nbr; i++) {
                Vector3 pos;
                do {
                    pos = position + Random.Range(0.5f, 2.5f) *
                        new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized;
                } while (Physics.OverlapSphere(pos, 0.6f).Length != 0);
                PhotonNetwork.Instantiate("Prefabs/Collectibles/"+_collectibles[Random.Range(0,3)] ,
                    pos,
                    Quaternion.identity);
            }
        }
        /*
         public static List<GameObject> SpawnList(uint value,Vector3 position) {
            // uint nbr = value / 40;
            uint nbr = value;
            List<GameObject> res = new List<GameObject>();
            for (int i = 0; i < nbr; i++) {
                GameObject go = Collectibles[Random.Range(0, 3)];
                go.transform.position = position + Random.Range(0.3f, 1f) *
                    new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized;
                res.Add(go);
            }
            return res;
        } */
    }
}

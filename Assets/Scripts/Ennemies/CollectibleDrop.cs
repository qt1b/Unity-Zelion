using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Ennemies {
    public class CollectibleDrop : MonoBehaviour {
        public static GameObject[] Collectibles;
        void Start() {
            Collectibles = Resources.LoadAll<GameObject>("Prefabs/Collectibles");
        }

        // for every 40hp of every ennemy, we get an orb
        // could be a little more smooth if elements were to go progressively to their place
        public static void Activate(uint value,Vector3 position) {
            // uint nbr = value / 40; // what we will use
            uint nbr = value;
            for (int i = 0; i < nbr; i++) {
                var instantiated = Instantiate(Collectibles[Random.Range(0, 3)] ,
                    position + Random.Range(0.3f,1f)*new Vector3(Random.Range(-1f,1f), Random.Range(-1f,1f), 0).normalized,
                    Quaternion.identity);
                instantiated.GetComponent<NetworkObject>().Spawn();
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

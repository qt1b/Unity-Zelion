using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjRenderLayer : MonoBehaviour
{
    public GameObject player;
    public Transform target;

    // Update is called once per frame
    void Update()
    {
        if (target.position.y >= transform.position.y) {
            this.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("OnTop");
        }
        else {
            this.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("Bellow");
        }
    }
}

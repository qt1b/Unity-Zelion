using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();   
    }

    public void Smash() {
        anim.SetBool("cut",true);
        StartCoroutine(delCollision());
    }

    IEnumerator delCollision() {
        yield return new WaitForSeconds(.3f);
        this.gameObject.SetActive(false);
    }
}

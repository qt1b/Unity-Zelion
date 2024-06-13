using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator Transition;

    public float transitime = 1f;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("o"))
        {
            LoadNextLvl(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void LoadNextLvl(int index)
    {
        StartCoroutine(LoadLevel(index));
    }

    IEnumerator LoadLevel(int index)
    {
        //anim
        Transition.SetTrigger("Start");
        //attends
        yield return new WaitForSeconds(transitime);
        //load
        SceneManager.LoadScene(index);
    }
}

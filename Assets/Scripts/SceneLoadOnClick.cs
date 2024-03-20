using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SceneLoadOnClick : MonoBehaviour, IPointerClickHandler
{
    public string sceneToLoad;

    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene(sceneToLoad,LoadSceneMode.Single);
    }
}

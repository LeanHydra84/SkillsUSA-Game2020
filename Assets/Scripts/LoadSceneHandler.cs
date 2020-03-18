using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LoadSceneHandler : MonoBehaviour
{

    [SerializeField]
    private Slider slider;

    void Start()
    {
        StartCoroutine(AsyncSceneLoad());
    }

    IEnumerator AsyncSceneLoad()
    {
        yield return new WaitForSeconds(0.2f);
        PlayerState.Reset();
        AsyncOperation sceneLoader = SceneManager.LoadSceneAsync("Alright");
        Debug.Log("Created Async Operation");
        while(sceneLoader.progress < 1)
        {
            Debug.Log(sceneLoader.progress);
            slider.value = sceneLoader.progress;
            yield return new WaitForEndOfFrame();
        }

    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LoadSceneHandler : MonoBehaviour
{
    public static int Scene { get; set; }
    [SerializeField] private Slider slider;

    private void Start()
    {
        StartCoroutine(AsyncSceneLoad());
    }

    IEnumerator AsyncSceneLoad()
    {
        yield return new WaitForSeconds(0.5f);
        PlayerState.Reset();
        Debug.Log(Scene);
        AsyncOperation sceneLoader = SceneManager.LoadSceneAsync(Scene);
        Debug.Log("Created Async Operation");
        while(sceneLoader.progress < 1)
        {
            Debug.Log(sceneLoader.progress);
            slider.value = sceneLoader.progress;
            yield return new WaitForEndOfFrame();
        }


    }

}

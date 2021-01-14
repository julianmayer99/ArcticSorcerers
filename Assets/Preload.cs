using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Preload : MonoBehaviour
{
    private void Start()
    {
        // TODO: Check is Version is valid
        StartCoroutine(LoadFirstFrameAndScene());
    }

    IEnumerator LoadFirstFrameAndScene()
    {
        yield return new WaitForEndOfFrame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}

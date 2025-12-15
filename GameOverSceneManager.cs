using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverSceneManager : MonoBehaviour
{
    // fade out screen on player ui
    public FadeScreen fadeScreen;

    [Header("Buttons")]
    public Button replayButton;
    public Button mainMenuButton;

    void Start()
    {
        // hook events with their events when clicked
        replayButton.onClick.AddListener(ReloadGame);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    void ReloadGame()
    {
        GoToScene(SceneManager.GetActiveScene().buildIndex);
    }

    void GoToMainMenu()
    {
        GoToScene(0);
    }

    public void GoToScene(int sceneIndex)
    {
        StartCoroutine(GoToSceneRoutine(sceneIndex));
    }

    IEnumerator GoToSceneRoutine(int sceneIndex)
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(fadeScreen.fadeDuration);

        //Launch the new scene
        SceneManager.LoadScene(sceneIndex);
    }
}

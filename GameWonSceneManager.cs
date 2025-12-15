using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameWonSceneManager : MonoBehaviour
{
    // fade out screen on player ui
    public FadeScreen fadeScreen;

    [Header("Buttons")]
    public Button nextLevelButton;
    public Button replayButton;
    public Button mainMenuButton;

    void Start()
    {
        // hook events with their events when clicked
        nextLevelButton.onClick.AddListener(NextLevel);
        replayButton.onClick.AddListener(ReloadGame);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    void NextLevel()
    {
        // load the next level, or if there isn't one, throw a warning (there should be a win screen)
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            GoToScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("############      WARNING: Trying to load a scene that is not on the build!!     ############");
        }
    }

    void ReloadGame()
    {
        // replay this scene
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

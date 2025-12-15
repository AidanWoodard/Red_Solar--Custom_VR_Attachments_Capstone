using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    // This script's purpose is just to set the UI above the player HUD
    // for feedback during combat. It takes values from 'GameManager.cs',
    // NOT from 'PlayerController.cs'.

    // ref to script
    private GameManager gameManager;

    // UI's
    private GameObject powerUpUI;
    private Slider powerUpSlider;

    void Start()
    {
        gameManager = GameObject.Find("GAME MANAGER").GetComponent<GameManager>();
        powerUpUI = GameObject.Find("Power Up Bar");
        powerUpSlider = powerUpUI.GetComponent<Slider>();

        // set values of slider
        powerUpSlider.maxValue = gameManager.powerThreshold;
        powerUpSlider.value = 0;
    }

    void Update()
    {
        if (gameManager.playerScript.handsConnected && gameManager.isGunCharged)
        {
            // give an error to the player
            // nevermind i ran out of time
        }

        // set health UI
        powerUpSlider.value = gameManager.powerLevel;
    }
}

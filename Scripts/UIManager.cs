using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    GameObject[] tireSpawners; //all tire spawners in level. will use length
    GameObject menu, hud;
    GameManager gameManager; //script
    PlayerControls playerControls; //script

    TextMeshProUGUI speedText; 
    TextMeshProUGUI gearText;
    TextMeshProUGUI fuelText;
    TextMeshProUGUI distanceText;
    TextMeshProUGUI tiresText;
    TextMeshProUGUI warningText;
    TextMeshProUGUI levelText;
    TextMeshProUGUI scoreText;

    void Start()
    {
        menu = GameObject.Find("Menu");
        hud = GameObject.Find("HUD");

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerControls = GameObject.Find("player").GetComponent<PlayerControls>();

        speedText = GameObject.Find("speed_txt").GetComponent<TextMeshProUGUI>();
        gearText = GameObject.Find("gear_txt").GetComponent<TextMeshProUGUI>();
        fuelText = GameObject.Find("fuel_txt").GetComponent<TextMeshProUGUI>();
        distanceText = GameObject.Find("distance_txt").GetComponent<TextMeshProUGUI>();
        tiresText = GameObject.Find("tires_txt").GetComponent<TextMeshProUGUI>();
        warningText = GameObject.Find("warning_txt").GetComponent<TextMeshProUGUI>();
        levelText = GameObject.Find("level_txt").GetComponent<TextMeshProUGUI>();
        scoreText = GameObject.Find("score_txt").GetComponent<TextMeshProUGUI>();

        tireSpawners = GameObject.FindGameObjectsWithTag("Tire Spawner");

        menu.SetActive(false);
        hud.SetActive(true);
        StartCoroutine(Countdown());
    }
    
    void Update()
    {
        speedText.text = (Mathf.Round(playerControls.speed * 2) + " KM/H"); //doubling for more realistic speed
        gearText.text = ("GEAR: " + playerControls.gear);              
        tiresText.text = ("TIRES" + playerControls.tiresCollected + "/" + tireSpawners.Length);
        
        if(!gameManager.levelPassed && !gameManager.isGameOver) //when game is over stop fuel draining on HUD, before menu pop up
            fuelText.text = ("FUEL" + Mathf.Ceil(playerControls.fuel));

        if (playerControls.fuel <= 0) // even out of fuel, HUD shows fuel is 1 beacuse of mathf.ceil, so we fix it
            fuelText.text = ("FUEL0");

        if (!gameManager.levelPassed) //don't show negative distance on HUD
        distanceText.text = ("FINISH" + (gameManager.distance / 500).ToString("0.0") + " KM"); //shows like X,X KM and doubled for more realistic distance

        if (gameManager.levelPassed)
        {
            scoreText.text = ("Tires Collected: " + playerControls.tiresCollected + "/" + tireSpawners.Length);
            menu.SetActive(true);
            hud.SetActive(false);            
        }
    }

    public void NextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            SceneManager.LoadScene(1);
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnMenu()
    {
        Destroy(GameObject.Find("Music Player"));
        SceneManager.LoadScene(0);
    }

    IEnumerator Countdown()
    {
        levelText.enabled = true;

        warningText.text = "3";
        warningText.enabled = true;
        yield return new WaitForSeconds(1f);
        warningText.text = "2";
        yield return new WaitForSeconds(1f);
        warningText.text = "1";
        yield return new WaitForSeconds(1f);
        warningText.text = "GO!";
        yield return new WaitForSeconds(1f);
        warningText.enabled = false;

        levelText.enabled = false;
    }

    public IEnumerator CrashWarning()
    {
        warningText.text = "CRASHED!";
        warningText.enabled = true;
        yield return new WaitForSeconds(2f);
        warningText.enabled = false;
    }

    public IEnumerator FuelWarning()
    {
        warningText.text = "OUT OF FUEL!";
        warningText.enabled = true;
        yield return new WaitForSeconds(2f);
        warningText.enabled = false;
    }

    public IEnumerator OilWarning()
    {
        warningText.text = "TIRES ARE OILED!";
        warningText.enabled = true;
        yield return new WaitForSeconds(2f);
        warningText.enabled = false;
    }

    public IEnumerator FinishWarning()
    {
        warningText.text = "FINISHED!";
        warningText.enabled = true;
        yield return new WaitForSeconds(2f);
        warningText.enabled = false;
    }    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    GameObject player, finish;
    PlayerControls playerControls;
    UIManager uiManager;

    [HideInInspector] public float distance; //level finisher     
    [HideInInspector] public bool isGameOver; //used for stop the spamming of coroutines in update to decrease players speed slowly when game overs
    [HideInInspector] public bool levelPassed; //For UIManager. It will show different menu

    public bool gameStarted; //using for countdown and player controls

    void Start()
    {
        playerControls = GameObject.Find("player").GetComponent<PlayerControls>();
        uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        finish = GameObject.Find("finish");
        player = GameObject.Find("player");
        StartCoroutine(StartGame());
    }

    void Update()
    {
        distance = finish.transform.position.z - player.transform.position.z;

        if(distance <= 0 && !isGameOver && !levelPassed) //used "else if" and booleans beacuse i don't want multiple gameover screens. just in case if they happen synchronous
        {
            Debug.Log("Congrats! You have finished the level.");
            levelPassed = true;
            StartCoroutine(uiManager.FinishWarning());
            StartCoroutine(playerControls.Stop());
        }
        else if (playerControls.fuel <= 0 && !isGameOver && !levelPassed) 
        {
            isGameOver = true;
            Debug.Log("Out of Fuel!");
            StartCoroutine(uiManager.FuelWarning());
            StartCoroutine(playerControls.Stop());
            StartCoroutine(RestartLevel());
        }
        else if (playerControls.crashed && !isGameOver && !levelPassed)
        {
            isGameOver = true;
            StartCoroutine(uiManager.CrashWarning());
            StartCoroutine(playerControls.Stop());
            StartCoroutine(RestartLevel());
        }
    }

    private IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator StartGame()
    {
        yield return new WaitForSeconds(3f);
        gameStarted = true;
    }
}

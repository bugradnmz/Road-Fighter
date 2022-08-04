using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject menu, levelSelect;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Play()
    {
        menu.SetActive(false);
        levelSelect.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Back()
    {        
        levelSelect.SetActive(false);
        menu.SetActive(true);
    }

    public void Level1()
    {
        SceneManager.LoadScene(1);
    }

    public void Level2()
    {
        SceneManager.LoadScene(2);
    }

    public void Level3()
    {
        SceneManager.LoadScene(3);
    }

    public void Level4()
    {
        SceneManager.LoadScene(4);
    }

    public void Level5()
    {
        SceneManager.LoadScene(5);
    }
}

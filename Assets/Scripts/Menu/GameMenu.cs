using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class GameMenu : MonoBehaviour
{
    public bool opened;
    public enum MenuMode { Main, Pause }
    public MenuMode menuMode;
    public GameObject settingsMenu;

    [Header("General")]
    public Transform UI;
    public GameObject newGameButton;
    public GameObject loadGameButton;
    [Space]
    public GameObject saveGameButton;
    public GameObject backToMainMenuButton;

    [Header("Main Menu")]
    public GameObject mainBackground;

    [Header("Pause Menu")]
    public GameObject pauseBackground;

    private void Update()
    {
        if(menuMode == MenuMode.Main)
        {
            UI.transform.localPosition = new Vector3(-626.894897f, -361.315399f, 27.9708462f);
        }
        else if (menuMode == MenuMode.Pause)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                opened = !opened;
            }
            if (opened)
                UI.transform.localPosition = new Vector3(-626.894897f, -361.315399f, 27.9708462f);
            else
            {
                UI.transform.localPosition = new Vector3(-10000, 0, 0);
            }
        }
    }
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void NewGame()
    {
        menuMode = MenuMode.Pause;

        if (mainBackground != null)
        {
            mainBackground.gameObject.SetActive(false);
        }
        if (pauseBackground != null)
        {
            pauseBackground.gameObject.SetActive(true);
        }

        newGameButton.gameObject.SetActive(false);
        loadGameButton.gameObject.SetActive(false);

        saveGameButton.gameObject.SetActive(true);
        backToMainMenuButton.gameObject.SetActive(true);
        SceneManager.LoadScene(1);
    }

    public void LoadGame()
    {
        // CHECK THAT DIRECTORY & FILE HAVE BEEN CREATED
        if (!Directory.Exists(Application.dataPath + "/saves"))
        {
            return;
        }

        if (!File.Exists(Application.dataPath + "/saves/GameSave.save"))
        {
            return;
        }

        SaveHandler.loaded = true;
        menuMode = MenuMode.Pause;

        if (mainBackground != null)
        {
            mainBackground.gameObject.SetActive(false);
        }
        if(pauseBackground != null)
        {
            pauseBackground.gameObject.SetActive(true);
        }
        mainBackground.SetActive(false);
        newGameButton.gameObject.SetActive(false);
        loadGameButton.gameObject.SetActive(false);

        saveGameButton.gameObject.SetActive(true);
        backToMainMenuButton.gameObject.SetActive(true);
        SceneManager.LoadScene(1);
    }

    public void BackToMain()
    {
        menuMode = MenuMode.Main;
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }
    public void SaveGame()
    {
        FindAnyObjectByType<SaveHandler>().Save();
    }
    public void Settings()
    {
        settingsMenu.SetActive(true);
    }

    public void Exit()
    {
        Application.Quit();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {
    /* This class handles the main menu user interaction
     * A notable exception is the interaction of the options menu which is handled by 
     * the options controller both in the main menu and in-game
     */
    
    [SerializeField]
    GameObject optionsMenu, creditsMenu, mainMenu, loadingMenu;

    [SerializeField]
    Slider slider;

    [SerializeField]
    Text loadingPercentageText;

    float startTime;

    private void Start() {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        loadingMenu.SetActive(false);
    }

    public void StartGame()
    {
        //SceneManager.LoadScene("GreenlightDemo");
        //LoadLevel();
        loadingMenu.SetActive(true);
        startTime = Time.time;
        StartCoroutine(LoadAsynchronously());
    }

    /*public void LoadLevel()
    {
        
    }*/

    IEnumerator LoadAsynchronously()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("GreenlightDemo");
        
        while (operation.isDone == false)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            //0-100
            //float p = operation.progress * 100f;//Time.time - startTime;
            //int pRounded = Mathf.RoundToInt(p);
            //string perc = pRounded.ToString();

            slider.value = progress;
            progress = Mathf.RoundToInt(100f * progress);
            loadingPercentageText.text = progress + "%";
            Debug.Log(progress);

            yield return null;
        }

    }

    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
    }

    public void HideMainMenu()
    {
        mainMenu.SetActive(false);
    }

    public void ShowOptions()
    {
        optionsMenu.SetActive(true);
        HideMainMenu();
    }

    public void HideOptions() {
        optionsMenu.SetActive(false);
        ShowMainMenu();
    }

    public void ShowCredits()
    {
        creditsMenu.SetActive(true);
        HideMainMenu();
    }

    public void HideCredits()
    {
        creditsMenu.SetActive(false);
        ShowMainMenu();
    }
    public void QuitGame()
    {
        Debug.Log("The application has been quit");
        Application.Quit(); //simply quits the game
    }
}

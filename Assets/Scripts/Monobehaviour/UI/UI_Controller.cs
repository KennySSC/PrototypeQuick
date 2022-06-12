using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using TMPro;

public class UI_Controller : MonoBehaviour
{
    #region Serializable Variables

    [Header ("Main Settings")]


    [Tooltip("Allows the menu main screen to behave like main menu or pause menu. Off= pause; On = Main")]
    [SerializeField] bool pauseOrMain;

    [Tooltip("Main menu play button")]
    [SerializeField] GameObject Btn_Play;

    [Tooltip("Pause menu continue button")]
    [SerializeField] GameObject Btn_Continue;

    [Tooltip("Main Menu exit button")]
    [SerializeField] GameObject Btn_Exit;

    [Tooltip("Pause menu return to menu button")]
    [SerializeField] GameObject Btn_MainMenu;

    [Tooltip("Object that has a scroll rect. It will be used if the scene has a Objective Manager")]
    [SerializeField] GameObject Pnl_Objectives;

    [Tooltip("The scroll rect content object. Used to obtain objectives")]
    [SerializeField] GameObject Objectives_Container;

    [Tooltip("If it's pause menu, you must attach player movement to it. Else leave it empty")]
    [SerializeField] PlayerMovement player;


    [Space]


    [Header("UI Objects")]


    [Tooltip("The main window of the menu")]
    [SerializeField] GameObject startingPanel;

    [Tooltip("All panel menus")]
    [SerializeField] List<GameObject> allPanels = new List<GameObject>();

    [Tooltip("All sub panel buttons. It refers to buttons that behave similar to default options menu, that move between sub panels")]
    [SerializeField] List<GameObject> allButtons = new List<GameObject>();

    [Tooltip("Option panel with sub panels. If you change the default panel and use normal ones, you can leave it empty")]
    [SerializeField] GameObject optionPnl;

    [Tooltip("Sound sub panel. . If you change the default options panel and use normal ones, you can leave it empty")]
    [SerializeField] GameObject soundPnl;

    [Tooltip("Sound sub panel. . If you change the default options panel and use normal ones, you can leave it empty")]
    [SerializeField] GameObject graphicsPnl;

    [Tooltip("Sound sub panel. . If you change the default options panel and use normal ones, you can leave it empty")]
    [SerializeField] GameObject bindingsPnl;


    [Space]


    [Header("Loading Panel settings")]

    [Tooltip("Loading screen panel")]
    [SerializeField] GameObject Pnl_Loading;

    [Tooltip("Slider that works as progress bar of the loading. It should have a min value of 0 and max of 1")]
    [SerializeField] Slider loadingSlider;

    [Tooltip("TMP text that shows the percentaje. If you don't need it, leave it empty")]
    [SerializeField] TMP_Text loadingPercentage_Tmp;

    [Tooltip("If you aren't using TMPro, text that shows the percentaje. If you don't need it, leave it empty")]
    [SerializeField] Text loadingPercentage_txt;


    [Space]


    [Header("Options settings")]


    [Tooltip("Quality drop down that is used to change graphics")]
    [SerializeField] TMP_Dropdown qualityDropDown;

    [Tooltip("Resolution drop down that is used to change resolution")]
    [SerializeField] TMP_Dropdown resolutionsDropDown;

    [Tooltip("Full screen toogle")]
    [SerializeField] Toggle tgl_Fullscreen;


    [Space]


    [Header("Sound")]

    [Tooltip("It performs when the cursor touches a UI object if you call the function. If you don't need it, leave it empty")]
    [SerializeField] GameObject hoverSound;

    [Tooltip("It performs when the player click a button if you call the function. If you don't need it, leave it empty")]
    [SerializeField] GameObject continueSound;

    [Tooltip("It performs when the player click a button if you call the function. If you don't need it, leave it empty")]
    [SerializeField] GameObject cancelSound;

    #endregion

    #region Private Variables

    private Resolution[] resolutions;
    private bool isFullScreen = true;
    private bool activateSound = false;

    private GameObject tempActivate;
    private GameObject tempBtn;
    private List<GameObject> desactivatePanels = new List<GameObject>();
    private SaveVolume[] volumes;
    private ObjectiveManager objectiveMan;
    #endregion

    #region Main Functions
    void Start()
    {
        Time.timeScale = 1;
        volumes = FindObjectsOfType<SaveVolume>();
        objectiveMan = FindObjectOfType<ObjectiveManager>();
        foreach (SaveVolume vl in volumes)
        {
            vl.Load();
        }
        //Desactivates all panels and shows the starting one. The exception is sound sub panel to show it when you enter to options menu
        foreach (GameObject gm in allPanels)
        {
            if (!desactivatePanels.Contains(gm))
            {
                desactivatePanels.Add(gm);
            }
            if (gm.activeSelf)
            {
                if(gm!= soundPnl)
                {
                    gm.SetActive(false);
                }
            }
        }
        if (!startingPanel.activeSelf)
        {
            startingPanel.SetActive(true);
        }
        //Change between pause and main menu behaviour
        if (pauseOrMain)
        {
            if (!Btn_Play.activeSelf)
            {
                Btn_Play.SetActive(true);
            }
            if (Btn_Continue.activeSelf)
            {
                Btn_Continue.SetActive(false);
            }
            if (!Btn_Exit.activeSelf)
            {
                Btn_Exit.SetActive(true);
            }
            if (Btn_MainMenu.activeSelf)
            {
                Btn_MainMenu.SetActive(false);
            }
        } else if (!pauseOrMain)
        {
            if (Btn_Play.activeSelf)
            {
                Btn_Play.SetActive(false);
            }
            if (!Btn_Continue.activeSelf)
            {
                Btn_Continue.SetActive(true);
            }
            if (Btn_Exit.activeSelf)
            {
                Btn_Exit.SetActive(false);
            }
            if (!Btn_MainMenu.activeSelf)
            {
                Btn_MainMenu.SetActive(true);
            }
        }
        // Set the saved quality
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Quality"));
        qualityDropDown.value = PlayerPrefs.GetInt("Quality");
        qualityDropDown.RefreshShownValue();

        resolutions = Screen.resolutions;
        resolutionsDropDown.ClearOptions();
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRate + "Hz";
            if (options.Contains(option))
            {

            }
            else
            {
                options.Add(option);
            }
        }
        resolutionsDropDown.AddOptions(options);
        resolutionsDropDown.value = PlayerPrefs.GetInt("ScreenResIndex");
        resolutionsDropDown.RefreshShownValue();
        SetResolution(PlayerPrefs.GetInt("ScreenResIndex"));

        if (PlayerPrefs.GetInt("FullScreen") == 0)
        {
            isFullScreen = false;
        }
        else
        {
            isFullScreen = true;
        }
        tgl_Fullscreen.isOn = isFullScreen;
        SetFullScreen(isFullScreen);

        tempBtn = allButtons[0];
        tempBtn.GetComponent<Button>().interactable = false;
        if(Pnl_Objectives.activeSelf)
        {
            Pnl_Objectives.SetActive(false);
        }

    }

    public void SpawnHoverSound()
    {
        if (hoverSound != null)
        {
            Instantiate(hoverSound, transform.position, transform.rotation);
        }
    }

    public void SpawnContinueSound()
    {
        if (continueSound != null)
        {
            Instantiate(continueSound, transform.position, transform.rotation);
        }
    }
    public void SpawnCancelSound()
    {
        if (cancelSound != null)
        {
            Instantiate(cancelSound, transform.position, transform.rotation);
        }
    }
    //If you don't want a empty sub options menu, call this function
    public void ActivateSoundPnl(bool activate)
    {
        activateSound = activate;
        if (activateSound)
        {
            tempBtn = allButtons[0];
            tempBtn.GetComponent<Button>().interactable = false;
        }
    }
    //Activates and hides panel instantly
    public void ActivatePanel_NoAnim(int activatePnl_Index)
    {
        desactivatePanels.RemoveAt(activatePnl_Index);
        if (activatePnl_Index == allPanels.IndexOf(bindingsPnl) || activatePnl_Index == allPanels.IndexOf(soundPnl) || activatePnl_Index == allPanels.IndexOf(graphicsPnl) || activatePnl_Index == allPanels.IndexOf(optionPnl))
        {
            if(activatePnl_Index == allPanels.IndexOf(optionPnl))
            {
                if (desactivatePanels.Contains(soundPnl))
                {
                    desactivatePanels.Remove(soundPnl);
                }
            }
            desactivatePanels.Remove(optionPnl);
        }
        foreach (GameObject pnl in desactivatePanels)
        {
            if (pnl.activeSelf)
            {
                pnl.SetActive(false);
            }
        }
        desactivatePanels.Clear();
        foreach (GameObject gm in allPanels)
        {
            if (!desactivatePanels.Contains(gm))
            {
                desactivatePanels.Add(gm);
            }
        }
        tempActivate = allPanels[activatePnl_Index];
        if (!tempActivate.activeSelf)
        {
            tempActivate.SetActive(true);
        }
    }
    //Activates and hides panel with animation
    public void ActivatePanel_Animated(int activatePnl_Index)
    {
        desactivatePanels.RemoveAt(activatePnl_Index);
        if (activatePnl_Index == allPanels.IndexOf(bindingsPnl) || activatePnl_Index == allPanels.IndexOf(soundPnl) || activatePnl_Index == allPanels.IndexOf(graphicsPnl) || activatePnl_Index == allPanels.IndexOf(optionPnl))
        {
            if (activatePnl_Index == allPanels.IndexOf(optionPnl))
            {
                if (desactivatePanels.Contains(soundPnl))
                {
                    desactivatePanels.Remove(soundPnl);
                }
            }
            desactivatePanels.Remove(optionPnl);
        }
        foreach (GameObject pnl in desactivatePanels)
        {
            if (pnl.activeSelf)
            {
                StartCoroutine(HidePanelAnim(pnl));
            }
        }
        desactivatePanels.Clear();
        foreach (GameObject gm in allPanels)
        {
            if (!desactivatePanels.Contains(gm))
            {
                desactivatePanels.Add(gm);
            }
        }
        tempActivate = allPanels[activatePnl_Index];
        if(tempActivate == startingPanel)
        {
            if (!soundPnl.activeSelf)
            {
                soundPnl.SetActive(true);
                foreach(GameObject gm in allButtons)
                {
                    gm.GetComponent<Button>().interactable = true;
                }
                allButtons[0].GetComponent<Button>().interactable = false;
            }
        }
        StartCoroutine(ShowPanelAnim(tempActivate));
    }
    //Desactivate a button of allButtons list by index. Used if a panel has sub panels
    public void DesactivateSelfButton(int index_Btn)
    {
        foreach(GameObject gm in allButtons)
        {
            gm.GetComponent<Button>().interactable = true;
        }
        allButtons[index_Btn].GetComponent<Button>().interactable = false;
        tempBtn = allButtons[index_Btn];
    }
    //Unpauses the game if pause behaviour is set
    public void ContinueGame()
    {
        if (!soundPnl.activeSelf)
        {
            soundPnl.SetActive(true);
        }
        player.OnChangePause();
    }
    //Changes quality by index
    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt("Quality", index);
    }
    //Changes resolution by index
    public void SetResolution(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.SetInt("ScreenResIndex", index);
    }
    //Changes full screen by toogle
    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        if (isFullScreen)
        {
            PlayerPrefs.SetInt("FullScreen",1);
        }
        else
        {
            PlayerPrefs.SetInt("FullScreen", 0);
        }

    }
    public GameObject GetContentObject()
    {
        return Objectives_Container;
    }
    public void ActivateObjectivesPanel()
    {
        if (!Pnl_Objectives.activeSelf)
        {
            Pnl_Objectives.SetActive(true);
        }
    }
    //Exits application
    public void ExitGame()
    {
        Application.Quit(0);
    }
    //Load a scene
    public void OnLoadScene(int sceneIndex)
    {
        StartCoroutine(SceneLoadCoroutine(sceneIndex));
    }

    #endregion

    #region Coroutines
    //Hides all panels with animation
    IEnumerator HidePanelAnim(GameObject desactivatePnl)
    {
        float elapsed = 0f;
        desactivatePnl.GetComponent<Animator>().Play("Hide");
        foreach(GameObject gm in allButtons)
        {
            gm.GetComponent<Button>().interactable = false;
        }
        while (elapsed < 0.5f)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        tempBtn.GetComponent<Button>().interactable = false;
        desactivatePnl.SetActive(false);
        if (activateSound)
        {
            if (!soundPnl.activeSelf)
            {
                soundPnl.SetActive(true);
                tempBtn = allButtons[0];
                foreach (GameObject gm in allButtons)
                {
                    gm.GetComponent<Button>().interactable = true;
                }
            }
            tempBtn.GetComponent<Button>().interactable = false;
            activateSound = false;
        }

    }
    //Show a panel with animation
    IEnumerator ShowPanelAnim(GameObject activatePnl)
    {
        float elapsed = 0f;
        foreach (GameObject gm in allButtons)
        {
            gm.GetComponent<Button>().interactable = false;
        }
        while (elapsed < 0.6f)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        if (!activatePnl.activeSelf)
        {
            Debug.Log("Im activating");
            activatePnl.SetActive(true);
        }
        foreach (GameObject gm in allButtons)
        {
            gm.GetComponent<Button>().interactable = true;
        }
        tempBtn.GetComponent<Button>().interactable = false;
        activatePnl.GetComponent<Animator>().Play("Show");
    }
    //Shows loading screen and the progress of it
    IEnumerator SceneLoadCoroutine(int sceneIndex)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex);
        if (!Pnl_Loading.activeSelf)
        {
            Pnl_Loading.SetActive(true);
        }
        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            loadingSlider.value = progress;
            if(loadingPercentage_Tmp!= null)
            {
                loadingPercentage_Tmp.text = (progress * 100).ToString("00") + "%";
            }else if( loadingPercentage_txt!= null)
            {
                loadingPercentage_txt.text = (progress * 100).ToString("00") + "%";
            }

            yield return null;
        }
    }

    #endregion 
}

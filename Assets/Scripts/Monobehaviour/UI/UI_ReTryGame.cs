using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UI_ReTryGame : MonoBehaviour
{
    [SerializeField] Slider loadingSlider;
    [SerializeField] TMP_Text loadingPercentage_Tmp;
    [SerializeField] Text loadingPercentage_txt;
    [SerializeField] GameObject Pnl_Loading;
    [SerializeField] GameObject Pnl_Main;

    [SerializeField] GameObject hoverSound;
    [SerializeField] GameObject clickSound;

    private int currentSceneIndex;

    private void Awake()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        Debug.Log("La escena es: " + currentSceneIndex);
    }
    private void Start()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
    IEnumerator SceneLoadCoroutine(int sceneIndex)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex);
        if (!Pnl_Loading.activeSelf)
        {
            Pnl_Loading.SetActive(true);
        }
        if (Pnl_Main.activeSelf)
        {
            Pnl_Main.SetActive(false);
        }
        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / 0.9f);
            loadingSlider.value = progress;
            if (loadingPercentage_Tmp != null)
            {
                loadingPercentage_Tmp.text = (progress * 100).ToString("00") + "%";
            }
            else if (loadingPercentage_txt != null)
            {
                loadingPercentage_txt.text = (progress * 100).ToString("00") + "%";
            }

            yield return null;
        }
    }
    public void RetryGame()
    {
        StartCoroutine(SceneLoadCoroutine(currentSceneIndex));
    }
    public void ReturnMenu()
    {
        StartCoroutine(SceneLoadCoroutine(0));
    }
    public void SpawnHoverSound()
    {
        if(hoverSound!= null)
        {
            Instantiate(hoverSound, transform.position, transform.rotation);
        }
    }
    public void SpawnClickSound()
    {
        if(clickSound!= null)
        {
            Instantiate(clickSound, transform.position, transform.rotation);
        }
    }
}

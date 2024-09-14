using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasScript : MonoBehaviour
{
    public GameObject backgrounds;
    public GameObject main;
    public GameObject setting;
    public GameObject levels;
    // Start is called before the first frame update
    void Start()
    {
        // sfxmix.SetFloat("volume", (PlayerPrefs.GetFloat("SFX")*80)-60f);
        // sfx.value = PlayerPrefs.GetFloat("SFX");
        // musicmix.SetFloat("volume", (PlayerPrefs.GetFloat("Music")*80)-60f);
        // music.value = PlayerPrefs.GetFloat("Music");

        // Screen.fullScreen = PlayerPrefs.GetInt("Fullscreen") == 1;
        // fullscreen.isOn = PlayerPrefs.GetInt("Fullscreen") == 1;

        // resolutions = Screen.resolutions;
        // res.ClearOptions();
        // List<string> options = new List<string>();
        // int currentresindex = 0;
        // for (int i = 0; i < resolutions.Length; i++)
        // {
        //     string option = resolutions[i].width + " x " + resolutions[i].height;
        //     options.Add(option);
        //     if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
        //     {
        //         currentresindex = i;
        //     }
        // }
        // res.AddOptions(options);
        // res.value = currentresindex;
        // res.RefreshShownValue();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void play()
    {
        StartCoroutine(scroll(721, levels));
    }
    public void settings()
    {
        StartCoroutine(scroll(-721, setting));
    }
    public void close()
    {
        StartCoroutine(scroll(0, main));
    }
    public void quit()
    {
        Application.Quit();
    }
    public IEnumerator scroll(int y, GameObject current)
    {
        main.SetActive(false);
        setting.SetActive(false);
        levels.SetActive(false);

        for(int i = 0; i < 20; i++)
        {
            backgrounds.GetComponent<RectTransform>().localPosition = Vector3.Lerp(backgrounds.GetComponent<RectTransform>().localPosition, new Vector3(0, y, 0), .25f);
            yield return new WaitForSeconds(.05f);
        }
        current.SetActive(true);
    }
    // public void sfxadjust()
    // {
    //     PlayerPrefs.SetFloat("SFX", sfx.value);
    //     sfx.gameObject.transform.Find("NumberText").GetComponent<TextMeshProUGUI>().text = (int)(PlayerPrefs.GetFloat("SFX")*100)+"%";
    //     sfxmix.SetFloat("volume", (PlayerPrefs.GetFloat("SFX")*80)-60f);
    // }
    // public void musicadjust()
    // {
    //     PlayerPrefs.SetFloat("Music", music.value);
    //     music.gameObject.transform.Find("NumberText").GetComponent<TextMeshProUGUI>().text = (int)(PlayerPrefs.GetFloat("Music")*100)+"%";
    //     musicmix.SetFloat("volume", (float)(PlayerPrefs.GetFloat("Music")*80)-60f);
    // }
    // public void fullscreentoggle()
    // {
    //     Screen.fullScreen = !Screen.fullScreen;
    //     if (fullscreen.isOn)
    //     {
    //         PlayerPrefs.SetInt("Fullscreen", 1);
    //     }
    //     else
    //     {
    //         PlayerPrefs.SetInt("Fullscreen", 0);
    //     }
    // }
    // public void changeres()
    // {
    //     Resolution setting_res = resolutions[res.value];
    //     Screen.SetResolution(setting_res.width, setting_res.height, Screen.fullScreen);
    // }
}

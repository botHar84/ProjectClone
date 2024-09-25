using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class CanvasScript : MonoBehaviour
{
    public bool menu;
    public bool paused;
    public GameObject backgrounds;
    public GameObject main;
    public GameObject setting;
    public GameObject levels;
    public AudioMixer sfxmix;
    public AudioMixer musicmix;
    public Slider sfx;
    public Slider music;
    public Toggle fullscreen;
    public Toggle playcutscene;
    public TMP_Dropdown res;
    public Resolution[] resolutions;
    public Sprite pause;
    public Sprite play;
    public Transform levelcollection;
    public bool moving;
    void Start()
    {
        var cutscene = PlayerPrefs.GetInt("PlayCutscene", 1);
        sfxmix.SetFloat("volume", (PlayerPrefs.GetFloat("SFX")*80)-60f);
        sfx.value = PlayerPrefs.GetFloat("SFX");
        musicmix.SetFloat("volume", (PlayerPrefs.GetFloat("Music")*80)-60f);
        music.value = PlayerPrefs.GetFloat("Music");

        Screen.fullScreen = PlayerPrefs.GetInt("Fullscreen") == 1;
        fullscreen.isOn = PlayerPrefs.GetInt("Fullscreen") == 1;

        playcutscene.isOn = PlayerPrefs.GetInt("PlayCutscene") == 1;

        resolutions = Screen.resolutions;
        res.ClearOptions();
        List<string> options = new List<string>();
        int currentresindex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentresindex = i;
            }
        }
        res.AddOptions(options);
        res.value = currentresindex;
        res.RefreshShownValue();
        if (!menu && PlayerPrefs.GetInt("CurrentLevel") == 1)
        {
            StartCoroutine("help", "HelpMessage");
        }
        if (menu)
        {
            foreach (Transform g in levelcollection)
            {
                g.gameObject.GetComponent<Image>().color = new Color((float)110/255, (float)110/255, (float)110/255, 1);
            }
            if (PlayerPrefs.GetInt("HighestLevel") == 10)
            {
                for (int i = 1; i <= PlayerPrefs.GetInt("HighestLevel"); i++)
                {
                    levelcollection.Find("Level"+i).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                }
            }
            else
            {
                for (int i = 1; i <= PlayerPrefs.GetInt("HighestLevel")+1; i++)
                {
                    levelcollection.Find("Level"+i).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !menu)
        {
            pausefunc();
        }
        if (Input.GetKeyDown(KeyCode.Escape) && menu)
        {
            if (!moving)
            {
                StartCoroutine(scroll(0, main));
            }
        }
    }
    public void pausefunc()
    {
        // pause button in this case
        paused = !paused;
        setting.SetActive(paused);
        if (paused)
        {
            backgrounds.GetComponent<Image>().sprite = play;
            backgrounds.GetComponent<RectTransform>().localPosition += new Vector3(1, 3);
            Time.timeScale = 0;
        }
        else
        {
            backgrounds.GetComponent<Image>().sprite = pause;
            backgrounds.GetComponent<RectTransform>().localPosition -= new Vector3(1, 3);
            Time.timeScale = 1;
        }
    }
    public void playfunc()
    {
        StartCoroutine(scroll(10, levels));
    }
    public void settings()
    {
        StartCoroutine(scroll(-10, setting));
    }
    public void close()
    {
        StartCoroutine(scroll(0, main));
    }
    public void quit()
    {
        Application.Quit();
    }
    public void levelselect(int level)
    {
        if (PlayerPrefs.GetInt("HighestLevel")+1 >= level)
        {
            PlayerPrefs.SetInt("CurrentLevel", level);
            if (PlayerPrefs.GetInt("CurrentLevel") == 1 && PlayerPrefs.GetInt("PlayCutscene") == 1)
            {
                SceneManager.LoadScene("cutscene");
            }
            else
            {
                SceneManager.LoadScene("SampleScene");
            }
        }
    }
    public void mainmenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
    public IEnumerator scroll(float y, GameObject current)
    {
        moving = true;
        main.SetActive(false);
        setting.SetActive(false);
        levels.SetActive(false);

        for(int i = 0; i < 20; i++)
        {
            backgrounds.transform.position = Vector3.Lerp(backgrounds.transform.position, new Vector3(0, y, 0), .25f);
            yield return new WaitForSeconds(.02f);
        }
        current.SetActive(true);
        moving = false;
    }
    public void sfxadjust()
    {
        PlayerPrefs.SetFloat("SFX", sfx.value);
        sfx.gameObject.transform.Find("NumberText").GetComponent<TextMeshProUGUI>().text = (int)(PlayerPrefs.GetFloat("SFX")*100)+"%";
        sfxmix.SetFloat("volume", (PlayerPrefs.GetFloat("SFX")*80)-60f);
    }
    public void musicadjust()
    {
        PlayerPrefs.SetFloat("Music", music.value);
        music.gameObject.transform.Find("NumberText").GetComponent<TextMeshProUGUI>().text = (int)(PlayerPrefs.GetFloat("Music")*100)+"%";
        musicmix.SetFloat("volume", (float)(PlayerPrefs.GetFloat("Music")*80)-60f);
    }
    public void fullscreentoggle()
    {
        Screen.fullScreen = !Screen.fullScreen;
        if (fullscreen.isOn)
        {
            PlayerPrefs.SetInt("Fullscreen", 0);
        }
        else
        {
            PlayerPrefs.SetInt("Fullscreen", 1);
        }
    }
    public void cutscenetoggle()
    {
        if (playcutscene.isOn)
        {
            PlayerPrefs.SetInt("PlayCutscene", 1);
        }
        else
        {
            PlayerPrefs.SetInt("PlayCutscene", 0);
        }
    }
    public void changeres()
    {
        Resolution setting_res = resolutions[res.value];
        Screen.SetResolution(setting_res.width, setting_res.height, Screen.fullScreen);
    }
    public void extHelp(string message)
    {
        StartCoroutine("help", message);
    }
    public IEnumerator help(string message)
    {
        yield return new WaitForSeconds(2);
        if (message == "HelpMessage")
        {
            yield return new WaitForSeconds(10);
            message = "Press [ESC] to see helpful keybinds.";
        }
        if (PlayerPrefs.GetInt("CurrentLevel") == 1 || (PlayerPrefs.GetInt("CurrentLevel") != 1 && message != "Press [ESC] to see helpful keybinds."))
        {
            main.GetComponent<TextMeshProUGUI>().text = message;
            for (int i = 0; i <= 255; i++)
            {
                main.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, (float)i/255);
                yield return new WaitForSeconds(.006f);
            }
            yield return new WaitForSeconds(3);
            for (int i = 255; i >= 0; i--)
            {
                main.GetComponent<TextMeshProUGUI>().color = new Color(1, 1, 1, (float)i/255);
                yield return new WaitForSeconds(.006f);
            }
        }
    }
}

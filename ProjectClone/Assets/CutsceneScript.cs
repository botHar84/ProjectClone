using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutsceneScript : MonoBehaviour
{
    public PlayableDirector director;
    void OnEnable()
    {
        director.stopped += OnPlayableDirectorStopped;
    }
    void OnPlayableDirectorStopped(PlayableDirector dir)
    {
        if (director == dir)
        {
            if (SceneManager.GetActiveScene().name == "Cutscene 3")
            {
                SceneManager.LoadScene("MainMenu");
            }
            else
            {
                SceneManager.LoadScene("SampleScene");
            }
        }
    }
    public void skip()
    {
        if (SceneManager.GetActiveScene().name == "Cutscene 3")
        {
            SceneManager.LoadScene("MainMenu");
        }
        else
        {
            SceneManager.LoadScene("SampleScene");
        }
    }
    void OnDisable()
    {
        director.stopped -= OnPlayableDirectorStopped;
    }
}

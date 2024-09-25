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
            SceneManager.LoadScene("SampleScene");
        }
    }
    public void skip()
    {
        SceneManager.LoadScene("SampleScene");
    }
    void OnDisable()
    {
        director.stopped -= OnPlayableDirectorStopped;
    }
}

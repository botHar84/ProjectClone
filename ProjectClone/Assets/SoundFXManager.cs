using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager instance;
    [SerializeField] private AudioSource soundFXObject; 

    // Dictionary to keep track of active sound effects and their sources
    private Dictionary<AudioClip, AudioSource> activeSoundFX = new Dictionary<AudioClip, AudioSource>();

    private void Awake(){
        if (instance == null){
            instance = this;
        }
    }

    // Function to play a sound effect
    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume){
        // Spawn in game object
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        // Assign the audioClip
        audioSource.clip = audioClip;
        // Assign volume
        audioSource.volume = volume;
        // Play sound 
        audioSource.Play();
        // Get length of sound FX clip
        float clipLength = audioSource.clip.length;
        // Store reference of the playing audio source
        activeSoundFX[audioClip] = audioSource;
        // Destroy the clip after it is done playing
        Destroy(audioSource.gameObject, clipLength);
    }

    // Function to stop a sound effect
    public void StopSoundFXClip(AudioClip audioClip){
        // Check if the audio clip is currently playing
        if (activeSoundFX.ContainsKey(audioClip)){
            // Stop the sound
            activeSoundFX[audioClip].Stop();
            // Destroy the audio source object
            Destroy(activeSoundFX[audioClip].gameObject);
            // Remove the clip from the dictionary
            activeSoundFX.Remove(audioClip);
        }
    }

    public void PlayRandomSoundFXClip(AudioClip[] audioClip, Transform spawnTransform, float volume){
        //random index
        int rand = Random.Range(0, audioClip.Length);
        //spawn in gameobject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);
        //assign the audioClip
        audioSource.clip = audioClip[rand];
        //assign volume
        audioSource.volume = volume;
        //play sound 
        audioSource.Play();
        //get length of sound fx clip
        float clipLength = audioSource.clip.length;
        //destroy the clip after it is done playing
        Destroy(audioSource.gameObject, clipLength);
    }

    // Add this method to your SoundFXManager class
    public bool IsPlaying(AudioClip audioClip){
        // Check if the audio clip is currently playing
        if (activeSoundFX.ContainsKey(audioClip))
        {
            return activeSoundFX[audioClip].isPlaying;
        }
        return false;
}

}

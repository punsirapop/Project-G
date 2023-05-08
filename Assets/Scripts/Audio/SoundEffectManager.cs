using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundEffectManager : MonoBehaviour
{
    [System.Serializable]
    public struct SoundEffect
    {
        public string Name;
        public AudioClip Clip;
        public string SceneName; // Scene name associated with the sound effect
    }

    public SoundEffect[] SoundEffects; // Array of sound effects
    private List<AudioSource> _AudioSources; // Array of AudioSources for each sound effect

    private static SoundEffectManager _Instance;
    private string _CurrentSceneName;

    void Awake()
    {
        _CurrentSceneName = SceneManager.GetActiveScene().name;
        
        if (_Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _Instance = this;
        DontDestroyOnLoad(gameObject);

        _AudioSources = new List<AudioSource>();

        // Create an AudioSource for each sound effect
        for (int i = 0; i < SoundEffects.Length; i++)
        {
            if(SoundEffects[i].SceneName == null || SoundEffects[i].SceneName == _CurrentSceneName)
            {
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.loop = false;
                _AudioSources.Add(audioSource);
            }
        }
    }

    public static SoundEffectManager Instance
    {
        get { return _Instance; }
    }

    public void PlaySoundEffect(string name)
    {
        SoundEffect soundEffect = GetSoundEffectByName(name);
        if (soundEffect.Clip != null)
        {
            // Find an available AudioSource for the sound effect
            AudioSource audioSource = GetAvailableAudioSource(soundEffect);

            // Play the sound effect using the AudioSource
            audioSource.PlayOneShot(soundEffect.Clip);
        }
    }

    private SoundEffect GetSoundEffectByName(string name)
    {
        foreach (SoundEffect soundEffect in SoundEffects)
        {
            if (soundEffect.Name == name)
            {
                return soundEffect;
            }
        }

        return new SoundEffect();
    }

    private AudioSource GetAvailableAudioSource(SoundEffect soundEffect)
    {
        // Find an available AudioSource for the sound effect
        foreach (AudioSource audioSource in _AudioSources)
        {
            if (!audioSource.isPlaying || audioSource.clip == null)
            {
                audioSource.clip = soundEffect.Clip;
                return audioSource;
            }
        }

        // No available AudioSource found, create a new one
        AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
        newAudioSource.playOnAwake = false;
        newAudioSource.clip = soundEffect.Clip;
        newAudioSource.loop = false;
        _AudioSources.Add(newAudioSource); // Add the new AudioSource to the list
        return newAudioSource;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    [System.Serializable]
    public struct MusicClip
    {
        public string SceneName;
        public AudioClip Clip;
        public float Volume;
    }

    public MusicClip[] MusicClips; // An array of music clips with scene names
    private AudioSource _AudioSource;
    private string _CurrentSceneName;

    void Awake()
    {
        // Check if an instance of the MusicManager already exists
        MusicManager[] musicManagers = FindObjectsOfType<MusicManager>();
        if (musicManagers.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        _AudioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        _CurrentSceneName = SceneManager.GetActiveScene().name;
        PlayMusic(_CurrentSceneName);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string newSceneName = scene.name;
        if (newSceneName != _CurrentSceneName)
        {
            _CurrentSceneName = newSceneName;
            PlayMusic(_CurrentSceneName);
        }
    }

    void PlayMusic(string sceneName)
    {
        foreach (MusicClip musicClip in MusicClips)
        {
            if (musicClip.SceneName == sceneName)
            {
                _AudioSource.clip = musicClip.Clip;
                _AudioSource.volume = musicClip.Volume;
                _AudioSource.Play();
                return;
            }
        }

        _AudioSource.Stop();
    }
    public void RestartMusic()
    {
        _AudioSource.Stop();
        _AudioSource.Play();
    }

    public void SetVolume(float newVolume)
    {
        _AudioSource.volume = newVolume;
    }

    public void PlaySpecificMusic(int index)
    {
        if (index >= 0 && index < MusicClips.Length)
        {
            _AudioSource.clip = MusicClips[index].Clip;
            _AudioSource.volume = MusicClips[index].Volume;
            _AudioSource.Play();
        }
    }
}

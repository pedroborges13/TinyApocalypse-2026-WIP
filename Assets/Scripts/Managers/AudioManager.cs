using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] AudioMixer mixer;

    [Header("References")]
    [SerializeField] private SoundLibraryData library;

    [Header("Pool Settings")]
    [SerializeField] private int defaultPoolSize;
    [SerializeField] private int maxPoolSize;

    private IObjectPool<AudioSource> audioPool;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            if (library != null) library.Initialize(); //Prepare the dictionary

            DontDestroyOnLoad(gameObject); //Keeps audio playing even during scene transitions
        }
        else
        {
            //AudioManager is required in the "MainMenu" scene, a duplicate exists in "Gameplay" for testing purposes, but it will be destroyed if the game is initialised from the menu
            Destroy(gameObject);
            return;
        }

        audioPool = new ObjectPool<AudioSource>(CreateAudioSource, GetFromPool, BackToPool, OnDestroyAudioSource, false, defaultPoolSize, maxPoolSize);
    }

    void Start()
    {
        LoadVolume();
    }

    public void PlaySound(SoundType type, Vector3 position)
    {
        //Fetch the sound settings from the library
        SoundEffectData effect = library.GetSound(type);

        if (effect == null) return;

        AudioSource audioSource = audioPool.Get(); //Get from Pool
        audioSource.transform.position = position;  

        //Execute the Play logic defined inside the SoundEffectData Scriptable Object
        effect.Play(audioSource);

        if (audioSource.clip != null)
        {
            float realDuration = audioSource.clip.length / Mathf.Max(0.1f, audioSource.pitch); //Mathf.Max prevents division by zero

            StartCoroutine(ReleaseAudioRoutine(audioSource, realDuration));
        }
        else
        {
            audioPool.Release(audioSource);
        }
    }

    // ----- AudioMixer Settings -----
    public void SetMusicVolume(float volume)
    {
        //Due to the slider art, values range from 0 to 17. Therefore, normalisation is required to avoid affecting the calculation
        float normalizedVolume = volume / 17;

        float dbValue = Mathf.Log10(Mathf.Max(normalizedVolume, 0.0001f)) * 20; //Mathf.Max prevents the value from hitting absolute zero, which would break the Log10 calculation

        //Updates the exposed parameter in the AudioMixer
        mixer.SetFloat("MusicVol", dbValue); 
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        //Due to the slider art, values range from 0 to 17. Therefore, normalisation is required to avoid affecting the calculation
        float normalizedVolume = volume / 17;

        float dbValue = Mathf.Log10(Mathf.Max(normalizedVolume, 0.0001f)) * 20; //Mathf.Max prevents the value from hitting absolute zero, which would break the Log10 calculation

        //Updates the exposed parameter in the AudioMixer
        mixer.SetFloat("SFXVol", dbValue);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    void LoadVolume()
    {
        //Retrieves the saved volume values or uses a default value if none is found
        float music = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        //Applies the loaded values to the AudioMixer groups
        SetMusicVolume(music);
        SetSFXVolume(sfx);
    }

    // ----- Pool Settings -----
    private AudioSource CreateAudioSource()
    {
        //Create a temporary GameObject
        GameObject go = new GameObject("Pooled AudioSource");
        go.transform.SetParent(transform);

        AudioSource audioSource = go.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f; //3D audio
        audioSource.minDistance = 3f;
        audioSource.maxDistance = 17.5f;
        audioSource.playOnAwake = false;

        return audioSource;
    }

    void GetFromPool(AudioSource audioSource)
    {
        audioSource.gameObject.SetActive(true);
    }

    void BackToPool(AudioSource audioSource)
    {
        audioSource.Stop();
        audioSource.clip = null;
        audioSource.gameObject.SetActive(false);
    }

    void OnDestroyAudioSource(AudioSource audioSource)
    {
        Destroy(audioSource.gameObject);
    }

    IEnumerator ReleaseAudioRoutine(AudioSource audioSource, float delay)
    {
        yield return new WaitForSeconds(delay);

        audioPool.Release(audioSource); 
    }
    //-------------------

}

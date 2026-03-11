using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

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
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); //Keeps audio playing even during scene transitions

        audioPool = new ObjectPool<AudioSource>(CreateAudioSource, GetFromPool, BackToPool, OnDestroyAudioSource, false, defaultPoolSize, maxPoolSize);
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
}

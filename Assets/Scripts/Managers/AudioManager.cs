using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("References")]
    [SerializeField] private SoundLibraryData library;

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
    }

    public void PlaySound(SoundType type, Vector3 position)
    {
        //Fetch the sound settings from the library
        SoundEffectData effect = library.GetSound(type);

        if (effect == null) return;

        //Create a temporary GameObject
        GameObject go = new GameObject("TempAudio");
        go.transform.position = position;   

        AudioSource audioSource = go.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.minDistance = 3f;
        audioSource.maxDistance = 20f;

        //Execute the Play logic defined inside the SoundEffectData Scriptable Object
        effect.Play(audioSource);

        //Calculate actual duration: Clip length divided by Pitch 
        //(Fast pitch = shorter sound, Slow pitch = longer sound)
        //float length = audioSource.clip.length / Mathf.Max(0.1f, audioSource.pitch);

        if (audioSource.clip != null) Destroy(go, audioSource.clip.length);
        else Destroy(go);
    }
}

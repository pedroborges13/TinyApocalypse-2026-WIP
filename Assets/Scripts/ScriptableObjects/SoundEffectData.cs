using Mono.Cecil.Cil;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "NewSound", menuName = "Audio/Sound Effect")]
public class SoundEffectData : ScriptableObject
{
    public AudioMixerGroup mixerGroup;

    public AudioClip[] clips;

    public float volume = 0.7f;
    public float pitchMin = 0.9f;
    public float pitchMax = 1.1f;

    public void Play(AudioSource source)
    {
        if (clips.Length == 0) return;

        source.clip = clips[Random.Range(0, clips.Length)];
        source.volume  = volume;
        source.pitch = Random.Range(pitchMin, pitchMax);
        source.outputAudioMixerGroup = mixerGroup;
        source.Play();
    }
}

using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType { PistolShot, SubmachineShot, ShotgunShot, SniperShot, BarrelExplosion, ZombieMoan }

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Audio/Sound Library")]
public class SoundLibraryData : MonoBehaviour
{
    [System.Serializable]
    public struct SoundMapping
    {
        public SoundType type; //The ID
        public SoundEffectData data; //The settings (Clips, Pitch, Volume)
    }

    public List<SoundMapping> soundList;

    private Dictionary<SoundType, SoundEffectData> soundDictionary;

    //Converts the List into a Dictionary for faster access during gameplay
    public void Initialize()
    {
        soundDictionary = new Dictionary<SoundType, SoundEffectData>(); 

        foreach (var mapping in soundList)
        {
            //Ensures I don't have duplicate keys
            if (!soundDictionary.ContainsKey(mapping.type))
            {
                soundDictionary.Add(mapping.type, mapping.data);
            }
        }
    }

    public SoundEffectData GetSound(SoundType type)
    {
        //Lazy initialization in case Initialize() wasn't called in Awake
        if (soundDictionary == null) Initialize();

        //Tries to get the value.
        //Returns null and logs a warning if the Enum is missing from the list
        if (soundDictionary.TryGetValue(type, out SoundEffectData data)) return data;

        Debug.LogWarning($"Sound type {type} not found in the library! Make sure it's assigned in the SO.");
        return null;
    }
}

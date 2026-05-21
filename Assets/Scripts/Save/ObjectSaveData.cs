using UnityEngine;
using System;
using NUnit.Framework;
using System.Collections.Generic;

[Serializable]
public class ObjectSaveData
{
    public PlacedObjectType objectType;
    public Vector3 position;
    public Quaternion rotation; 
}

[Serializable]
public class GameSaveData
{
    //Progress and money
    public int currentWave;
    public int currentMoney;

    //Weapons
    public List<string> savedWeaponNames = new List<string>();
    public int savedWeaponIndex;

    //Items
    public List<ObjectSaveData> placedObjects = new List<ObjectSaveData>();

}

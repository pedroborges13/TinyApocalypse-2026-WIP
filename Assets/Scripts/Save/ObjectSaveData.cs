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
    public int playerMoney;

    //Equipments
    public string currentWeaponName;
    public List<string> unlockedWeaponsNames = new List<string>();

    //Items
    public List<ObjectSaveData> placedObjects = new List<ObjectSaveData>();

}

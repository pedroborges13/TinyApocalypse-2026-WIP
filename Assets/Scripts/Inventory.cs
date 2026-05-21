using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Weapons")]
    [SerializeField] private List<GameObject> weapons = new(); //Sem o new() da NullReference
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private GameObject pistolPrefab;
    private int currentWeaponIndex;

    private int maxWeapons = 4;
    //private int maxGranades = 2;
    private GameObject currentWeapon;

    [Header("Prefabs for SaveManager")]
    [SerializeField] private GameObject smgPrefab;
    [SerializeField] private GameObject shotgunPrefab;
    [SerializeField] private GameObject sniperPrefab;

    void Awake()
    {
        //Equip pistol
        AddWeapon(pistolPrefab);
        EquipWeapon(0);
        //Debug.Log(currentWeaponIndex);
    }

    public Weapon GetCurrentWeapon()
    {
        if (currentWeapon == null) return null;
        return currentWeapon.GetComponent<Weapon>();
    }

    public void AddWeapon(GameObject weaponPrefab)
    {
        if (weapons.Count >= maxWeapons) return;

        GameObject newWeapon = Instantiate(weaponPrefab, weaponTransform);
        newWeapon.gameObject.SetActive(false); //Disable until equipped

        //Add to the inventory list
        weapons.Add(newWeapon);
    }

    public void EquipWeapon(int index)
    {
        if (index < 0 || index >= weapons.Count) return;

        //Disable current weapon
        if (currentWeapon != null) currentWeapon.GetComponent<Weapon>().OnUnequip();

        currentWeaponIndex = index;
        currentWeapon = weapons[currentWeaponIndex];

        //Enable new weapon
        currentWeapon.GetComponent<Weapon>().OnEquip();
    }

    public void NextWeapon()
    {
        int nextIndex = (currentWeaponIndex + 1) % weapons.Count;
        EquipWeapon(nextIndex);
    }

    public void PreviousWeapon()
    {
        //The "%" operator wraps the index around, preventing negative values
        int previousIndex = (currentWeaponIndex - 1 + weapons.Count) % weapons.Count;
        EquipWeapon(previousIndex);
    }

    // --- METHODS FOR SYSTEM SAVE---
    public List<string> GetWeaponNamesInIventory()
    {
        List<string> names = new List<string>();
        foreach(GameObject weaponObj in weapons)
        {
            Weapon w = weaponObj.GetComponent<Weapon>();
            if (w != null) names.Add(w.WeaponName);
        }

        return names;
    }

    // Returns the index of the weapon the player is holding
    public int GetCurrentWeaponIndex() => currentWeaponIndex;

    public void LoadInventoryState(List<string> savedNames, int savedIndex)
    {
        foreach (GameObject weaponObj in weapons)
        {
            if (weaponObj != null) Destroy(weaponObj);
        }

        weapons.Clear();
        currentWeapon = null;

        foreach (string name in savedNames)
        {
            GameObject prefab = GetWeaponPrefabByName(name);

            if (prefab != null)
            {
                AddWeapon(prefab);
            }
        }

        EquipWeapon(savedIndex);
    }

    private GameObject GetWeaponPrefabByName(string name)
    {
        switch (name)
        {
            case "Pistol":
                return pistolPrefab;
            case "SMG":
                return smgPrefab;
            case "Shotgun":
                return shotgunPrefab;
            case "Sniper":
                return sniperPrefab;
            default: 
                return null;
        }
    }

}

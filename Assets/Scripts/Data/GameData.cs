using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameData
{
    public static int Souls 
    {
        get => PlayerPrefs.GetInt("souls", 0);
        set => PlayerPrefs.SetInt("souls", value);
    }

    public static int CondensedSouls
    {
        get => PlayerPrefs.GetInt("condensedSouls", 0);
        set => PlayerPrefs.SetInt("condensedSouls", value);
    }

    public static string Weapon
    {
        get => PlayerPrefs.GetString("weapon", "");
        set => PlayerPrefs.SetString("weapon", value);
    }

    public static int HealthSeal
    {
        get => PlayerPrefs.GetInt("healthSeal", 0);
        set => PlayerPrefs.SetInt("healthSeal", value);
    }

    public static int PortalSeal
    {
        get => PlayerPrefs.GetInt("portalSeal", 0);
        set => PlayerPrefs.SetInt("portalSeal", value);
    }

    public static int LaserSeal
    {
        get => PlayerPrefs.GetInt("laserSeal", 0);
        set => PlayerPrefs.SetInt("laserSeal", value);
    }

    public static int Yallah
    {
        get => PlayerPrefs.GetInt("yallah", 0);
        set => PlayerPrefs.SetInt("yallah", value);
    }

    [System.Serializable]
    private class Wrapper { public List<string> items; }

    public static List<string> ownedUpgrades = new List<string>();

    public static void AddUpgrade(string displayName) => ownedUpgrades.Add(displayName);

    public static int GetLevel(string displayName) => ownedUpgrades.Count(n => n == displayName);

    public static void Save() =>
        PlayerPrefs.SetString("upgrades", JsonUtility.ToJson(new Wrapper { items = ownedUpgrades }));

    public static void Load()
    {
        if (PlayerPrefs.HasKey("upgrades"))
            ownedUpgrades = JsonUtility.FromJson<Wrapper>(PlayerPrefs.GetString("upgrades")).items;
    }
}

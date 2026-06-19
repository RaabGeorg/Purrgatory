using UnityEngine;
using UnityEngine.UI;

public class WeaponImage : MonoBehaviour
{
    public Texture2D[] images;
    public RawImage inventoryImage;

    void Start()
    {
        if (GameData.Weapon.Equals("Railgun"))
        {
            inventoryImage.texture = images[0];
        }
        else if (GameData.Weapon.Equals("Rifle"))
        {
            inventoryImage.texture = images[1];
        }
        else if (GameData.Weapon.Equals("Shotgun"))
        {
            inventoryImage.texture = images[2];
        }
    }
}

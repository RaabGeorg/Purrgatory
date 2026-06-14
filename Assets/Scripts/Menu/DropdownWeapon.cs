using TMPro;
using UnityEngine;

public class DropdownWeapon : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;

    public void GetDropdownValue() 
    {
        int pickedEntry = dropdown.value;
    }
}

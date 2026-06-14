using TMPro;
using UnityEngine;

public class SoulDisplayShop : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI soulsText;

    void Update()
    {
        soulsText.text = $"Souls Captured: {GameData.Souls.ToString()}";
    }
}

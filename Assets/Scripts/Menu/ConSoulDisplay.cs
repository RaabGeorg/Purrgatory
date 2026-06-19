using TMPro;
using UnityEngine;

public class ConSoulDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI soulsText;

    void Update()
    {
        soulsText.text = $"Condensed Souls Captured: {GameData.CondensedSouls.ToString()}";
    }
}

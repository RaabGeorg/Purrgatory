using UnityEngine;
using UnityEngine.UI;

public class Ability2UI : MonoBehaviour
{
    public Image icon;

    private PlayerControls _controls;
    
    public float cooldown = 2f;
    private float timer;

    private void Awake()
    {
        _controls = new PlayerControls();
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    void Update()
    {
        if (_controls.Player.Spell2.WasPressedThisFrame() && timer <= 0 && SceneSwitchManager.Instance.CurrentLevel == "Level_Hell")
        {
            timer = cooldown;
        }

        if (timer > 0)
        {
            timer -= Time.deltaTime;
            icon.fillAmount = 1 - (timer / cooldown);
        }
    }
}
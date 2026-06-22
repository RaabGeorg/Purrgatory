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
        if (VortexSpawner.instance ==null)
        {
            return;
        }
        
        timer = VortexSpawner.instance.cooldownTimer;

        if (timer > 0)
        {
            icon.fillAmount = 1 - (timer / cooldown);
        }
    }
}
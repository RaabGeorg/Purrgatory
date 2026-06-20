using Components;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class Modthing : MonoBehaviour
{
    public Text label;
    public TextMeshProUGUI description;
    public Toggle toggle;
    public GameObject modthing;
    public RaycastWeapon railgun;
    private EntityManager _em;

    void Start()
    {
        _em = World.DefaultGameObjectInjectionWorld.EntityManager;


        if (GameData.Weapon == "Railgun")
        {
            label.fontSize = 28;
            label.text = "Beam Overdrive";
            description.text = "Hold [Right-Click] to charge beam. Release to fire a high-power shot that pierces through all enemies.";
        }
        else
        {
            label.fontSize = 20;
            label.text = "Excessive, Explosive, (Non-)Economic";
            description.text = "Adds explosive payloads to your ammunition. Bullets travel normally and trigger a massive explosion at the point of impact.";
        }


        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    public void addModToInventory() 
    {
        modthing.SetActive(true);
    }

    void OnToggleChanged(bool isOn) 
    {
        if (isOn)
        {
            if (GameData.Weapon == "Railgun")
            {
                railgun.overchargeUnlocked = true;
            }
            else
            {
                var query = _em.CreateEntityQuery(typeof(Weapon), typeof(WeaponFromPlayerTag));
                if (!query.HasSingleton<WeaponFromPlayerTag>()) return;

                var entity = query.GetSingletonEntity();
                if (_em.HasComponent<VortexMod>(entity))
                {
                    var vortex = _em.GetComponentData<VortexMod>(entity);
                    vortex.Radius = 5;
                    _em.SetComponentData(entity, vortex);
                }
                
            }
        }
        else
        {
            if (GameData.Weapon == "Railgun")
            {
                railgun.overchargeUnlocked = false;
            }
            else
            {
                var query = _em.CreateEntityQuery(typeof(Weapon), typeof(WeaponFromPlayerTag));
                if (!query.HasSingleton<WeaponFromPlayerTag>()) return;

                var entity = query.GetSingletonEntity();
                if (_em.HasComponent<VortexMod>(entity))
                {
                    var vortex = _em.GetComponentData<VortexMod>(entity);
                    vortex.Radius = 0;
                    _em.SetComponentData(entity, vortex);
                }

            }
        }
    }
}

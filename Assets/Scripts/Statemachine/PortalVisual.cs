using System.Collections.Generic;
using UnityEngine;

// Am Portal GameObject
public class PortalVisual : MonoBehaviour
{
    public static List<PortalVisual> All = new();
    void Awake()
    {
        gameObject.SetActive(false);
    }
    void OnEnable()  => All.Add(this);
    void OnDisable() => All.Remove(this);
    void OnDestroy() => All.Remove(this);

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}

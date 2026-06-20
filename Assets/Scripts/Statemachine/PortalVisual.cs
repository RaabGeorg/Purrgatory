using System.Collections.Generic;
using UnityEngine;


public class PortalVisual : MonoBehaviour
{
    public static List<PortalVisual> All = new();

    void Awake()  
    { 
        All.Add(this);
        gameObject.SetActive(false); 
    }
    
    void OnDestroy() => All.Remove(this);
}

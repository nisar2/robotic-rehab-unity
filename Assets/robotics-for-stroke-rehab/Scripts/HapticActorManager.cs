using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HapticActorManager : MonoBehaviour
{
    public GameObject hapticActor;
    
    public void DisableActor()
    {
        hapticActor.SetActive(false);  
    }

    
    public void EnableActor()
    {
        hapticActor.SetActive(true);   
    }
}

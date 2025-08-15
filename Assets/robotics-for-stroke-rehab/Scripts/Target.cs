//Version: 15.7.25
//added explanatory comments by DS
using System.Collections; //for list and arrays
using System.Collections.Generic;
using TMPro; //TectMeshPro for text handling
using UnityEngine; //Unity engine core functions

public class Target : MonoBehaviour
{
    public Material CollidingMaterial; //material to show when the stylus collides with this arget
    public Material DefaultMaterial; // normal look without touching
    public TextMeshProUGUI TargetLabelTxt; //displays label above the target.
    
    private void OnTriggerEnter(Collider collision)//runs when there is a collision
    {
        //Debug.Log("Colliding with " + collision.gameObject.name + " at distance of " + Vector3.Distance(transform.position, collision.gameObject.transform.position));
        ChangeMaterial(CollidingMaterial);
    }

    private void OnTriggerExit(Collider collision)//runs when the colliding object leaves the trigger area
    {
        //Debug.Log("Colliding with " + collision.gameObject.name);
        ChangeMaterial(DefaultMaterial);
    }

    private void ChangeMaterial(Material m)
    {
        Renderer r = GetComponent<Renderer>(); //finds renderer
        Material[] mats = r.materials;//gets the arrau of material used
        mats[0] = m;//replaces the first material woth the new material
        r.materials = mats;//sets it back to the update
    }

    public void UpdateTargetLabelText(string text) //sets new display text
    {
        TargetLabelTxt.text = text;
    }
}

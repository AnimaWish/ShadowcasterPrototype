using UnityEngine;
using System.Collections;

public class raycastForward : MonoBehaviour
{

    RaycastHit hit;
    GameObject shadow = null;

    // Update is called once per frame
    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 5;

        // This won't show up in game, but gives you an idea of where the ray is projecting
        Debug.DrawRay(transform.position, forward, Color.green);

        // will only cast shadows on a Cube that is in front of the player
        if (Physics.Raycast(transform.position, forward, out hit) && hit.collider.name == "Box")
        {
            Vector3 hitPos = hit.transform.position;
            Vector3 myPos = transform.position;
            Vector3 hitScale = hit.transform.localScale;

            // only create one primitive at a time
            if (shadow == null)
            {
                shadow = GameObject.CreatePrimitive(PrimitiveType.Cube);
                shadow.GetComponent<Renderer>().material.color = Color.black;
                shadow.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            // scale of the shadow is relative to the height and width of the object that you're "hitting" and the length of the shadow is 
            // determined by the players distance from the hit. 
            shadow.transform.localScale = new Vector3(hitScale.x, hitScale.y,15);
            // position is relative to the players local transform. (0,0,1) is right in front of the player. I add the distance / 2 to spawn
            // the shadow cube behind the cube you're looking at.
            float hitShadow = shadow.transform.localScale.z / 2 + hit.transform.localScale.z / 2;
            shadow.transform.position = new Vector3(hitPos.x - hitShadow, hitPos.y, hitPos.z);
            // rotation of the shadow matches the players rotation
            shadow.transform.rotation = transform.rotation;
        }
        // if not colliding with an object, destroy the shadow that exists. 
        else
        {
            if (shadow != null)
            {
                Destroy(shadow);
            }
            shadow = null;
        }
    }
    public float shadowDistance()
    {

        return 0.0f;
    }
}

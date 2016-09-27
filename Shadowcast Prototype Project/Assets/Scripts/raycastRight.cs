using UnityEngine;
using System.Collections;

public class raycastRight : MonoBehaviour
{

    RaycastHit hit;
    RaycastHit hit2;
    GameObject shadow = null;

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 right = transform.TransformDirection(Vector3.right) * 5;

        // This won't show up in game, but gives you an idea of where the ray is projecting
        Debug.DrawRay(transform.position, right, Color.green);

        // will only cast shadows on a Cube that is in front of the player
        if (Physics.Raycast(transform.position, right, out hit) && hit.collider.name == "Box")
        {
            Physics.Raycast(hit.transform.position, right, out hit2, 30, LayerMask.GetMask("wall"));
            Vector3 hitPos = hit.transform.position;
            Vector3 myPos = transform.position;
            Vector3 hitScale = hit.transform.localScale;

            // only create one primitive at a time
            if (shadow == null)
            {
                shadow = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //shadow.GetComponent<Renderer>().material.color = new Color(0.5f, 1.0f, 1.0f, 0.5f);
                shadow.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                shadow.GetComponent<Renderer>().material.color = new Color(0f, 0f, 0f, 0.8f);
                var mat = shadow.GetComponent<Renderer>().material;
                mat.SetFloat("_Mode", 3);
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.DisableKeyword("_ALPHABLEND_ON");
                mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
                shadow.GetComponent<Renderer>().material = mat;
            }

            // scale of the shadow is relative to the height and width of the object that you're "hitting" and the length of the shadow is 
            // determined by the players distance from the hit. 
            shadow.transform.localScale = new Vector3(hit2.distance, hitScale.y, hitScale.z);
            // position is relative to the players local transform. (0,0,1) is right in front of the player. I add the distance / 2 to spawn
            // the shadow cube behind the cube you're looking at.
            float hitShadow = shadow.transform.localScale.x / 2 + hit.transform.localScale.x / 2;
            shadow.transform.position = new Vector3(hitPos.x, hitPos.y, hitPos.z + hitShadow);
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

using UnityEngine;
using System.Collections;

public class PickupObject : MonoBehaviour {

    private GameObject mainCamera;
    private bool carrying;
    private GameObject carriedObject;

    public float floatDistance;
    public float floatSpeed;
    public float pickupRange;

    // Use this for initialization
    void Start ()
    {
        mainCamera = GameObject.FindWithTag("MainCamera"); // Sets the main camera to variable mainCamera by finding its tag
        
	}

    // FixedUpdate is called every fixed framerate frame
    void FixedUpdate ()
    {
	    if (carrying)
        {
            Carry(carriedObject);
            DropObject();
        }
        else
        {
            Pickup();
        }
	}

    private void Carry(GameObject obj)
    {
        // Object floats in front of player at "floatDistance"
        obj.transform.position = Vector3.Lerp(obj.transform.position, mainCamera.transform.position + mainCamera.transform.forward * floatDistance, Time.deltaTime * floatSpeed);

        // Prevents object from rotating while held
        obj.transform.rotation = Quaternion.identity; // !! CHANGE FOR ROTATING WITH BODY !!
    }

    private void Pickup()
    {
        if (Input.GetMouseButtonDown(0)) // On left click
        {
            // Middle of Screen
            int x = Screen.width / 2;
            int y = Screen.height / 2;

            // The ray
            Ray ray = mainCamera.GetComponent<Camera>().ScreenPointToRay(new Vector3(x,y));
            RaycastHit hit;

            // Shoots the ray
            if (Physics.Raycast(ray, out hit))
            {
                //Debug.Log(hit.distance);
                
                // If object hit is whithin range
                if (hit.distance <= pickupRange)
                {
                    // Checks if object has the Pickupable script
                    Pickupable p = hit.collider.GetComponent<Pickupable>();

                    if (p != null)
                    {
                        carrying = true;
                        carriedObject = p.gameObject;

                        // While carrying, object is unaffected by gravity
                        p.gameObject.GetComponent<Rigidbody>().useGravity = false;
                        //p.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    }
                }
            }
        }
    }

    private void DropObject()
    {
        if (Input.GetMouseButtonDown(0)) // On left click
        {
            carrying = false;
            carriedObject.gameObject.GetComponent<Rigidbody>().useGravity = true;
            //carriedObject.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            carriedObject = null;
        }
    }
}

using UnityEngine;
using System.Collections;

public class triggerButton : MonoBehaviour {

    GameObject door;
    bool entered = false;

    void Start()
    {
        door = GameObject.Find("Door");
    }
	void OnTriggerEnter(Collider other)
    {
        if (other.name == "Block")
        {
            entered = true;
            Vector3 end = door.transform.position;
            end.x = door.transform.position.x - door.transform.localScale.x;
            float i = 0.0f;
            float rate = 1.0f / Time.deltaTime;
            while (i < 1.0)
            {
                i += Time.deltaTime * rate;
                door.transform.position = Vector3.Lerp(door.transform.position, end, i);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.name == "Block")
        {
            Vector3 end = door.transform.position;
            end.x = door.transform.position.x + door.transform.localScale.x;
            float i = 0.0f;
            float rate = 1.0f / Time.deltaTime;
            while (i < 1.0)
            {
                i += Time.deltaTime * rate;
                door.transform.position = Vector3.Lerp(door.transform.position, end, i);
            }
        }
    }
}

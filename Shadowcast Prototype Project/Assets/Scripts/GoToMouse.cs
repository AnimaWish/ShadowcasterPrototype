using UnityEngine;
using System.Collections;

public class GoToMouse : MonoBehaviour
{

    public float speed = 1.5f;
    private Vector3 target;
    LayerMask groundMask;
    //float moveSpeed = 0.1f;
    float moveSpeed = 8f;
    bool drag;

    void Start()
    {
        groundMask = LayerMask.GetMask("ground");
        target = transform.position;
    }

    void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0))
        {
            drag = true;
            gameObject.GetComponent<Rigidbody>().useGravity = false; // Added for scroll wheel up and down so cube can float
        }

        else if (Input.GetMouseButtonUp(0))
        {
            drag = false;
            gameObject.GetComponent<Rigidbody>().useGravity = true; // Added for scroll wheel up and down so cube can float
        }

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))
        {
            if (drag)
                Move(hit.point);
        }
        
    }

    void Move(Vector3 TargetPos)
    {
        TargetPos.y = transform.position.y;

        // Scroll Wheel code:
        var yPos = Input.GetAxis("Mouse ScrollWheel");
        if (yPos > 0f) // Scroll Up
        {
            TargetPos.y+=2;
        }
        else if (yPos < 0f) // Scroll Down
        {
            TargetPos.y-=2;
        }
        
        Vector3 StartPosition = transform.position;
        Vector3 EndPosition = TargetPos;

        //float t = 0.0f;

        //while (t < 1.0)
        //{
            //t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(StartPosition, EndPosition, Time.deltaTime * moveSpeed);
        //}
    }
}

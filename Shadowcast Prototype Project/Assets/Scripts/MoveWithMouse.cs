using UnityEngine;
using System.Collections;

public class MoveWithMouse : MonoBehaviour
{

    public float speed = 1.5f;
    private Vector3 target;
    LayerMask groundMask;
    LayerMask objectMask;
    //float moveSpeed = 0.1f;
    float moveSpeed = 8f;
    bool drag = false;

    void Start()
    {
        groundMask = LayerMask.GetMask("ground");
        objectMask = LayerMask.GetMask("objects");
        target = transform.position;
    }

    void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit, Mathf.Infinity, objectMask) && !drag)
        {
            if (hit.transform == transform)
            {
                drag = true;
                gameObject.GetComponent<Rigidbody>().useGravity = false; // used for scroll wheel
            }
        }
        if (Input.GetMouseButtonDown(1) && drag)
        {
            gameObject.GetComponent<Rigidbody>().useGravity = true;
            drag = false;
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
            TargetPos.y += 2;
        }
        else if (yPos < 0f) // Scroll Down
        {
            TargetPos.y -= 2;
        }

        Vector3 StartPosition = transform.position;
        Vector3 EndPosition = TargetPos;

        transform.position = Vector3.Lerp(StartPosition, EndPosition, Time.deltaTime * moveSpeed);
    }
}
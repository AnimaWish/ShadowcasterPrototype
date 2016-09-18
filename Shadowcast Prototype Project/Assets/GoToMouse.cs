using UnityEngine;
using System.Collections;

public class GoToMouse : MonoBehaviour
{

    public float speed = 1.5f;
    private Vector3 target;
    LayerMask groundMask; 
    float moveSpeed = 0.1f;
    bool drag;

    void Start()
    {
        groundMask = LayerMask.GetMask("ground");
        target = transform.position;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0))
        {
            drag = true;
        }

        else if (Input.GetMouseButtonUp(0))
        {
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
        Vector3 StartPosition = transform.position;
        Vector3 EndPosition = TargetPos;

        float t = 0.0f;

        while (t < 1.0)
        {
            t += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(StartPosition, EndPosition, t);
        }
    }
}

using UnityEngine;
using System.Collections;

public class raycastForward : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        float theDistance;
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 5;
        Debug.DrawRay(transform.position, (forward), Color.green);
    }
}
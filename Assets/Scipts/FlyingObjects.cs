using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingObjects : MonoBehaviour, IDrag
{
    public void OnDragStart()
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }

    public void OnDragEnd()
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }
}

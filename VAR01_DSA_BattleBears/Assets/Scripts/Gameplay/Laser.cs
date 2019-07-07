using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private float lifeTime = 0.1f;

    public void Init (Vector3 start, Vector3 end)
    {
        LineRenderer line = GetComponent<LineRenderer>();
        line.SetPosition(0, start);
        line.SetPosition(1, end);
        Destroy(gameObject, lifeTime);
    }
}

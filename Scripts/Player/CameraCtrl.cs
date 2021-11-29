using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{

    private Transform tr;
    public Transform neckTr;

    public float forward;
    public float up;
    public float right;

    public Vector3 offset;

    void Start()
    {
        tr = GetComponent<Transform>();
        offset = new Vector3(0.1f, -0.06f, 0.01f);

    }

    private void LateUpdate()
    {
        tr.position = neckTr.position - (neckTr.up * offset.y + neckTr.right * offset.x + neckTr.forward * offset.z);

    }

}

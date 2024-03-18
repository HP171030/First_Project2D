using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class LineRendererAtoB : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;


    }

    public void LineOn(Vector3 from ,Vector3 to)
    {
        lineRenderer.enabled =true;
        lineRenderer.SetPosition(0, from);
        lineRenderer.SetPosition(1, to);
    }

    public void Stop()
    {
        lineRenderer.enabled = false;
    }
}

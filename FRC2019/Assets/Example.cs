using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{
    // Detects manually if obj is being seen by the main camera
    LimeLightData LL;
    BoxCollider CameraView;
    GameObject[] VisionTargets;
    public List<GameObject> visionMemory;
    public GameObject CurrentTarget;
    private void Start()
    {
        if (!CameraView)
        {
            CameraView = GetComponent<BoxCollider>();
        }
        LL = transform.parent.GetComponent<LimeLightData>();
        VisionTargets = GameObject.FindGameObjectsWithTag("VisionTarget");
        HideAllVisionTargets();
    }
    private void Update()
    {
        showClosestValidTarget();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "VisionTarget")
        {
            HideAllVisionTargets();
            visionMemory.Add(other.gameObject);
            showClosestValidTarget();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "VisionTarget" && visionMemory.Contains(other.gameObject))
        {
            HideAllVisionTargets();
            visionMemory.Remove(other.gameObject);
            showClosestValidTarget();
        }
    }
    void showClosestValidTarget()
    {
        if (visionMemory.Count <= 0)
            return;
        double closest = 10000.0f;
        GameObject close = visionMemory[0];
        foreach (GameObject g in visionMemory)
        {
            double distanceLength = Vector3.Distance(transform.position, g.transform.position);
            if (distanceLength < closest)
            {
                closest = distanceLength;
                close = g;
            }
        }
        close.GetComponent<SpriteRenderer>().enabled = true;
        CurrentTarget = close;
        LL.tvValidTarget = 1.0;
        Camera cam = GetComponent<Camera>();
        LL.txOffsetX = cam.pixelWidth / 2.0 
            - cam.WorldToScreenPoint(close.transform.position).x;
        LL.tyOffsetY = cam.WorldToScreenPoint(close.transform.position).y
           - cam.pixelHeight / 2.0;
        LL.tsSkewRotation = Vector3.Angle(close.transform.position - transform.position, transform.forward);
        LL.taTargetArea = 10.0-closest;

    }
    /*private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "VisionTarget" && !visionMemory)
        {
            HideAllVisionTargets();
            other.GetComponent<SpriteRenderer>().enabled = true;
            visionMemory = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "VisionTarget" && visionMemory == other.gameObject)
        {
            HideAllVisionTargets();
            visionMemory = null;
        }
    }*/
    void HideAllVisionTargets()
    {
        for (int i = 0; i < VisionTargets.Length; i++)
        {
            VisionTargets[i].GetComponent<SpriteRenderer>().enabled = false;
            CurrentTarget = null;
            LL.resetValues();
        }
    }
}

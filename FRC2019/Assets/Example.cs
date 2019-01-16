using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{
    // Detects manually if obj is being seen by the main camera
    BoxCollider CameraView;
    public GameObject[] VisionTargets;
    public List<GameObject> visionMemory;
    public float range = 5f;
    private void Start()
    {
        if (!CameraView)
        {
            CameraView = GetComponent<BoxCollider>();
        }
        VisionTargets = GameObject.FindGameObjectsWithTag("VisionTarget");
        HideAllVisionTargets();
    }
    private void Update()
    {
        showClosestValidTarget();
        //EnableClosestTarget();
        //CheckTargetGone();
    }
    /*void CheckTargetGone()
    {
        if (!visionMemory)
            return;
        if (Vector3.Distance(visionMemory.transform.position, transform.position) > range)
        {
            HideAllVisionTargets();
            visionMemory = null;
        }
    }
    void EnableClosestTarget()
    {
        float closest = 10000.0f;
        int index = -1;
        for (int i = 0; i < VisionTargets.Length; i++)
        {
            float distanceLength = Vector3.Distance(transform.position, VisionTargets[i].transform.position);
            if (distanceLength <= range && distanceLength < closest)
            {
                closest = distanceLength;
                index = i;
                print(index);
            }
        }
        if (index != -1)
        {
            HideAllVisionTargets();
            VisionTargets[index].GetComponent<SpriteRenderer>().enabled = true;
            visionMemory = VisionTargets[index].gameObject;
        }
    }*/
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
        float closest = 10000.0f;
        GameObject close = visionMemory[0];
        foreach (GameObject g in visionMemory)
        {
            float distanceLength = Vector3.Distance(transform.position, g.transform.position);
            if (distanceLength < closest)
            {
                closest = distanceLength;
                close = g;
            }
        }
        close.GetComponent<SpriteRenderer>().enabled = true;
        
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
            
        }
    }
}

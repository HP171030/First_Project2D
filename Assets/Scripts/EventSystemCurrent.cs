using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemCurrent : MonoBehaviour
{
    GameObject obj;
    // Update is called once per frame
    void Update()
    {
        Debug.Log(EventSystem.current.IsPointerOverGameObject());
        Debug.Log(EventSystem.current.currentSelectedGameObject);
        if (obj != EventSystem.current.currentSelectedGameObject )
        {
            obj = EventSystem.current.currentSelectedGameObject;
            
        }

    }
}

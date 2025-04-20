using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SpinButton : MonoBehaviour
{
    [SerializeField] Wheel wheel;
    private void OnTouchPress()
    {
        Debug.Log("Spin Button");
        wheel.Spin();
    }
}

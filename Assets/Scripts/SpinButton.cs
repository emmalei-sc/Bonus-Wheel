using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SpinButton : MonoBehaviour
{
    [SerializeField] WheelSpinner wheel;
    private void OnTouchPress()
    {
        wheel.Spin();
    }
}

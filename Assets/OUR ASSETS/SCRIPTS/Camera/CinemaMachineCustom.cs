using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineFreeLook))]
public class FreeLookUserInput : MonoBehaviour
{
    private bool _freeLookActive;

    // Use this for initialization
    private void Start()
    {
        CinemachineCore.GetInputAxis = GetInputAxis;
    }

    private void Update()
    {
        _freeLookActive = Input.GetMouseButton(1); // 0 = left mouse btn or 1 = right
    }

    private float GetInputAxis(string axisName)
    {
        return !_freeLookActive ? 0 : Input.GetAxis(axisName == "Mouse Y" ? "Mouse Y" : "Mouse X");
    }
}
// Written by Joy de Ruijter
using UnityEngine;

public class IsometricCamera : MonoBehaviour
{
    #region Variables

    [Header ("Camera Properties")]
    [SerializeField] private GameObject target;
    [SerializeField] private float size = 10;
    [SerializeField] private float scrollSpeed = 30;
    [SerializeField] private float distance = 30;

    private Camera cam;

    #endregion
   
    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
        cam.transform.rotation = Quaternion.Euler(30, 45, 0);
    }

    private void LateUpdate()
    {
        Zoom();
        Move();
        Rotate();
    }

    // Zoom in or out with the camera using the mouse scrollwheel
    private void Zoom()
    {
        cam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed * Time.deltaTime;
    }

    // Moves the camera depending on the position of the target
    private void Move()
    {
        transform.position = Vector3.Lerp(transform.position, target.transform.position + new Vector3(-distance, distance, -distance), 0.5f * Time.deltaTime);
    }

    // Rotates the camera to look at the target
    private void Rotate()
    {
        cam.transform.LookAt(target.transform);
    }
}

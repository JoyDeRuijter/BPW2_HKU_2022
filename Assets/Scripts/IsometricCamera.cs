// Written by Joy de Ruijter
using UnityEngine;

public class IsometricCamera : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject target;
    [SerializeField] private float size = 10;
    [SerializeField] private float scrollSpeed = 30;

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
        this.cam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * scrollSpeed * Time.deltaTime;

        float distance = 30;
        transform.position = Vector3.Lerp(transform.position, target.transform.position + new Vector3(-distance, distance, -distance), 0.5f * Time.deltaTime);
        this.cam.transform.LookAt(target.transform);
    }
}

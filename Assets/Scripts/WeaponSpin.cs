// Written by Joy de Ruijter
using UnityEngine;

public class WeaponSpin : MonoBehaviour
{
    #region Variables
    
    #endregion

    private void Awake()
    {
        
    }

    private void Update()
    {
        transform.Rotate(Vector3.up * Time.deltaTime * 20);
    }
}

using UnityEngine;

namespace UnitySampleAssets.Vehicles.Car
{
    // this script is specific to the car supplied in the the assets
    // it controls the suspension hub to make it move with the wheel are it goes over bumps

    public class Suspension : MonoBehaviour
    {

        public Wheel wheel; // The wheel that the script needs to referencing to get the postion for the suspension

        private Vector3 originalPosition;

        private void Start()
        {
            originalPosition = transform.localPosition;
        }

        private void Update()
        {
            transform.localPosition = originalPosition + wheel.suspensionSpringPos*transform.up;
        }
    }
}
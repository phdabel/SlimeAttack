using UnityEngine;

public class DynamicCam : MonoBehaviour
{
    public GameObject CameraAerea;

    private void OnTriggerEnter(Collider other)
    {
        if (CameraAerea == null)
        {
            Debug.LogError("CameraAerea is not assigned in the Inspector");
            return;
        }

        if (other.CompareTag("Cam Trigger"))
        {
            Debug.Log("CameraAerea activated");
            CameraAerea.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (CameraAerea == null)
        {
            Debug.LogError("CameraAerea is not assigned in the Inspector");
            return;
        }

        if (other.CompareTag("Cam Trigger"))
        {
            Debug.Log("CameraAerea deactivated");
            CameraAerea.SetActive(false);
        }
    }
}
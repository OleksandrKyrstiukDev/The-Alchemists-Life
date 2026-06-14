
using Unity.Cinemachine;
using UnityEngine;

public class CameraFocusController : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private CinemachineCamera playerCam;
    [SerializeField] private CinemachineCamera brewCam;

    public void FocusCauldron()
    {
        playerCam.Priority = 10;
        brewCam.Priority = 20;
    }

    public void ReturnToPlayer()
    {
        brewCam.Priority = 10;
        playerCam.Priority = 20;
    }
}
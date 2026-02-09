using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    [Header("Cinemachine Virtual Cameras")]
    public CinemachineCamera player1Camera;
    public CinemachineCamera player2Camera;

    [Header("Settings")]
    public int currentPlayer = 1; // Start with Player 1

    private void Start()
    {
        // Ensure only the first player's camera is active at start
        SetActiveCamera(player1Camera, player2Camera);
    }

    // Call this when turn ends
    public void SwitchTurn()
    {
        if (currentPlayer == 1)
        {
            currentPlayer = 2;
            SetActiveCamera(player2Camera, player1Camera);
        }
        else
        {
            currentPlayer = 1;
            SetActiveCamera(player1Camera, player2Camera);
        }
    }

    private void SetActiveCamera(CinemachineCamera activeCam, CinemachineCamera inactiveCam)
    {
        activeCam.Priority = 10;   // Higher priority = active
        inactiveCam.Priority = 1;  // Lower priority = inactive
    }



}

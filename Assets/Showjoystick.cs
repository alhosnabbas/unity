using UnityEngine;

public class Showjoystick : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject joystickUI; // Assign your joystick GameObject in Inspector

    void Start()
    {
#if UNITY_ANDROID || UNITY_IOS
        joystickUI.SetActive(true);   // Enable on mobile
#else
        joystickUI.SetActive(false);  // Disable on PC / Console
#endif
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class inputField : MonoBehaviour
{
    public TMP_InputField inputFieldX;
    public TMP_InputField inputFieldY;
    public TextMeshProUGUI terrainSizeText;

    private TMP_InputField activeField;

    void Start()
    {
        Terrain terrain = Terrain.activeTerrain;

        if (terrain != null)
        {
            Vector3 size = terrain.terrainData.size;
            terrainSizeText.margin = new Vector4(25, 0, 0, 0);
            terrainSizeText.text = "Terrain: " + size.x + " x " + size.z;
        }
        else
        {
            //Debug.LogError("No active terrain found!");
        }

        // Register TMP events
        inputFieldX.onSelect.AddListener(OnInputSelected);
        inputFieldY.onSelect.AddListener(OnInputSelected);
        inputFieldX.onDeselect.AddListener(OnInputDeselect);
        inputFieldY.onDeselect.AddListener(OnInputDeselect);
    }

    private void OnInputSelected(string value)
    {
        // Track the active field
        if (EventSystem.current.currentSelectedGameObject == inputFieldX.gameObject)
            activeField = inputFieldX;
        else if (EventSystem.current.currentSelectedGameObject == inputFieldY.gameObject)
            activeField = inputFieldY;

#if UNITY_ANDROID || UNITY_IOS
        // Explicitly open keyboard
        if (activeField != null)
        {
            activeField.ActivateInputField();
            TouchScreenKeyboard.Open(activeField.text, TouchScreenKeyboardType.Default);
        }
#endif
    }

    private void OnInputDeselect(string value)
    {
#if UNITY_ANDROID || UNITY_IOS
        // Prevent accidental close while typing
        if (TouchScreenKeyboard.visible && activeField != null)
        {
            activeField.ActivateInputField();
        }
#endif
    }

    private void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
        // Keep the keyboard alive
        if (TouchScreenKeyboard.visible && activeField != null && !activeField.isFocused)
        {
            activeField.ActivateInputField();
        }
#endif
    }

    public void ReadInput()
    {
        string textX = inputFieldX.text;
        string textY = inputFieldY.text;
        //.Log("Player typed: " + textX + " " + textY);
    }
}

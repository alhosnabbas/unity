using UnityEngine;
using UnityEngine.UI;

public class CanvasScaler : MonoBehaviour
{
    public RectTransform bottomPanel;
    public Button toggleButton;
    public float slideDuration = 0.5f;

    private Vector2 shownPos;
    private Vector2 hiddenPos;
    private Vector2 targetPos;
    private bool isVisible = false;
    private float slideTimer = 0f;
    private bool isSliding = false;

    void Start()
    {
        float height = bottomPanel.rect.height;

        // Panel fully visible
        shownPos = new Vector2(0, 100);

        // Panel hidden, but leave the top border (and button) visible
        float buttonHeight = toggleButton.GetComponent<RectTransform>().rect.height;
        hiddenPos = new Vector2(0, -(height - 100));
        //        Debug.Log($"Panel Height: {height}, Button Height: {buttonHeight}, Hidden Y: {hiddenPos.y}");

        // Start in hidden state if you want
        //bottomPanel.anchoredPosition = hiddenPos;
    }

    public void TogglePanel()
    {
        isVisible = !isVisible;
        targetPos = isVisible ? shownPos : hiddenPos;
        slideTimer = 0f;
        isSliding = true;
    }

    void Update()
    {
        if (isSliding)
        {
            slideTimer += Time.deltaTime;
            float t = Mathf.Clamp01(slideTimer / slideDuration);
            bottomPanel.anchoredPosition = Vector2.Lerp(bottomPanel.anchoredPosition, targetPos, t);

            if (t >= 1f)
                isSliding = false;
        }
    }
}

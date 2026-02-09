using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    public GameObject healthBarUI;

    private void Start()
    {
        if (healthBarUI != null)
            healthBarUI.SetActive(false);
    }

    public void ShowHealthBar()
    {
        if (healthBarUI != null)
            healthBarUI.SetActive(true);
    }

    public void HideHealthBar()
    {
        if (healthBarUI != null)
            healthBarUI.SetActive(false);
    }
}

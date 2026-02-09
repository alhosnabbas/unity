using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;

    private SelectableObject currentSelected;
    private bool isLaunched = false;
    PlayerTurn playerTurn;

    void Awake()
    {
        Instance = this;
        playerTurn = FindFirstObjectByType<PlayerTurn>();
    }

    void Update()
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        HandleMouseSelection();
        HandleKeyboardLaunch();
#elif UNITY_ANDROID || UNITY_IOS
        HandleTouchSelection();
        // Launch is now only via UI button
#endif
    }

    // ---------------- PC Selection ----------------
    void HandleMouseSelection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            TrySelect(Camera.main.ScreenPointToRay(Input.mousePosition));
        }
    }

    void HandleKeyboardLaunch()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isLaunched)
        {
            if (playerTurn.GetCurrentPlayer() == 1)
            {

                LaunchCurrentSelected();
            }
        }
    }

    // ---------------- Mobile Selection ----------------
    void HandleTouchSelection()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                if (UIUtils.IsTouchOverUI(touch.position))
                    return; // ignore touches on UI/joysticks

                TrySelect(Camera.main.ScreenPointToRay(touch.position));
            }
        }
    }


    // ---------------- Shared ----------------
    void TrySelect(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            SelectableObject newSelectable = hit.collider.GetComponentInParent<SelectableObject>();

            // ✅ Only allow selecting Player launchers
            if (newSelectable != null && newSelectable.CompareTag("Player"))
            {
                // If we clicked the same one → deselect it
                if (currentSelected == newSelectable)
                {
                    //Deselect();
                    return;
                }

                // Otherwise switch to the new one
                if (currentSelected != null)
                {
                    currentSelected.HideHealthBar();
                }

                currentSelected = newSelectable;
                currentSelected.ShowHealthBar();

                // Debug.Log("Selected: " + currentSelected.gameObject.name);
            }
            else
            {
                // Hit something else → deselect
                Deselect();
            }
        }
        else
        {
            // Hit nothing → deselect
            Deselect();
        }
    }


    public void LaunchCurrentSelected()
    {
        // Debug.Log("LaunchCurrentSelected called");
        if (currentSelected == null || isLaunched) return;

        // Debug.Log("Launching: " + currentSelected.name);

        if (currentSelected.name == "Tochka(Clone)")
        {
            //Debug.Log("Launching Tochka");
            TochkaLauncher rl = currentSelected.GetComponent<TochkaLauncher>();
            if (rl != null) StartLaunch(rl.InitiateRocket(() => { isLaunched = false; }));
        }
        else if (currentSelected.name == "Smerch(Clone)")
        {
            RocketLauncher rl = currentSelected.GetComponent<RocketLauncher>();
            if (rl != null) StartLaunch(rl.InitiateRocket(() => { isLaunched = false; }));
        }
        else if (currentSelected.name == "V2Rocket(Clone)")
        {
            V2 rl = currentSelected.GetComponent<V2>();
            if (rl != null) StartLaunch(rl.InitiateRocket(() => { isLaunched = false; }));
        }
    }

    private void StartLaunch(System.Collections.IEnumerator coroutine)
    {
        isLaunched = true;
        StopAllCoroutines();
        StartCoroutine(coroutine);
    }

    public void OnLaunchButtonPressed() // Assign this to your UI button
    {

        if (playerTurn.GetCurrentPlayer() == 1)
        {

            LaunchCurrentSelected();
        }

    }




    // Optional: helper to check if a launcher is selected
    public bool HasSelection()
    {
        return currentSelected != null;
    }

    public void KeepFocus(TMP_InputField field)
    {
        field.ActivateInputField();
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);

    }

    public void Deselect()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (UIUtils.IsTouchOverUI(touch.position))
                return; // don’t deselect when tapping UI
        }

        if (currentSelected != null)
        {
            currentSelected.HideHealthBar();
            currentSelected = null;
        }
    }


}

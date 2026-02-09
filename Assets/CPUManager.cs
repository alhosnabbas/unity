using System.Collections;
using UnityEngine;

public class CPUManager : MonoBehaviour
{
    private LauncherManager launcherManager;
    public GameObject[] cpuLauncherPrefabs;
    private bool isCPUCreated = false;
    private bool isLaunched = false;
    public SelectableObject currentSelected; // Currently selected launcher

    PlayerTurn playerTurn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        Terrain terrain = Terrain.activeTerrain;
        launcherManager = FindFirstObjectByType<LauncherManager>();
        playerTurn = FindFirstObjectByType<PlayerTurn>();
        if (isCPUCreated == false)
        {
            if (launcherManager.isMaxReachedCPU == false)
            {
                for (int i = 0; i <= launcherManager.getMaxLaunchersCPU(); i++)
                {
                    Debug.Log("Creating CPU Launcher " + i);
                    Debug.Log("Max CPU Launchers: " + launcherManager.getMaxLaunchersCPU());
                    float x = Random.Range(256, terrain.terrainData.size.x - 256);
                    float z = Random.Range(256, terrain.terrainData.size.z - 256);
                    Vector3 cpuPosition = new Vector3(x, 20f, z);
                    GameObject prefabToSpawn = cpuLauncherPrefabs[Random.Range(0, cpuLauncherPrefabs.Length)];
                    GameObject cpuRocket = Instantiate(prefabToSpawn, cpuPosition, Quaternion.identity);
                    Debug.Log("CPU Launcher created at: " + cpuPosition);
                    cpuRocket.tag = "Enemy";

                    Rigidbody rb = cpuRocket.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.isKinematic = false;
                        rb.useGravity = true;
                    }

                    // ✅ Add the instance’s Launcher, not the prefab’s
                    Launcher launcherComp = cpuRocket.GetComponent<Launcher>();
                    launcherComp.SetOwnership(false);
                    launcherManager.AddLauncher(launcherComp, false);
                    if (launcherManager.isMaxReachedCPU == true)
                    {
                        break;
                    }
                }

                isCPUCreated = true;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void StartCPUTurn()
    {
        Launcher[] cpuLaunchers = launcherManager.getCPULaunchers();
        Launcher[] playerLaunchers = launcherManager.getPlayerLaunchers();

        if (cpuLaunchers.Length == 0 || playerLaunchers.Length == 0)
            return;

        StartCoroutine(FireAllCPULaunchersSequentially(cpuLaunchers, playerLaunchers));
    }

    private IEnumerator FireAllCPULaunchersSequentially(Launcher[] cpuLaunchers, Launcher[] playerLaunchers)
    {
        foreach (Launcher cpuLauncher in cpuLaunchers)
        {
            if (!cpuLauncher.isPlayerOwned)
            {
                // Select this launcher
                SelectableObject selectable = cpuLauncher.GetComponent<SelectableObject>();
                if (selectable != null)
                {
                    currentSelected = selectable;
                    currentSelected.ShowHealthBar();
                }

                // Pick the closest player launcher as target
                Launcher closestPlayerLauncher = null;
                float closestDistance = Mathf.Infinity;

                foreach (Launcher playerLauncher in playerLaunchers)
                {
                    float distance = Vector3.Distance(cpuLauncher.transform.position, playerLauncher.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPlayerLauncher = playerLauncher;
                    }
                }

                if (closestPlayerLauncher != null)
                {
                    // Fire based on launcher type
                    yield return FireSelectedLauncherAtCoroutine(closestPlayerLauncher.transform.position);
                }

                // Deselect after firing
                if (currentSelected != null)
                {
                    currentSelected.HideHealthBar();
                    currentSelected = null;
                }

                // Wait a short delay before next launcher fires
                yield return new WaitForSeconds(1.5f); // adjust as needed
            }
        }
        Deselect();
        Debug.Log("CPU turn finished");
        playerTurn.SwichTurn();
    }

    private IEnumerator FireSelectedLauncherAtCoroutine(Vector3 targetPos)
    {
        if (currentSelected == null)
            yield break;

        TochkaLauncher t = currentSelected.GetComponent<TochkaLauncher>();
        if (t != null)
        {
            yield return StartCoroutine(t.Fire(targetPos, 5f, () => { isLaunched = false; }));
            yield break;
        }

        RocketLauncher r = currentSelected.GetComponent<RocketLauncher>();
        if (r != null)
        {
            yield return StartCoroutine(r.Fire(targetPos, 5f, () => { isLaunched = false; }));
            yield break;
        }

        V2 v = currentSelected.GetComponent<V2>();
        if (v != null)
        {
            yield return StartCoroutine(v.Fire(targetPos, 5f, () => { isLaunched = false; }));
            yield break;
        }
    }




    public void SelectLauncher(SelectableObject newSelectable)
    {
        if (newSelectable == null)
        {
            Deselect();
            return;
        }

        // If we clicked/selected the same one → do nothing
        if (currentSelected == newSelectable)
            return;

        // Hide old health bar
        if (currentSelected != null)
            currentSelected.HideHealthBar();

        // Switch to new selection
        currentSelected = newSelectable;
        currentSelected.ShowHealthBar();

        Debug.Log("Selected: " + currentSelected.gameObject.name);
    }

    // ✅ CPU uses this (no raycast)
    public void CPUSelectLauncher(Launcher cpuLauncher)
    {
        if (cpuLauncher == null) return;

        // Try to get SelectableObject from the launcher (adjust GetComponentInParent if needed)
        SelectableObject selectable = cpuLauncher.GetComponentInParent<SelectableObject>() ?? cpuLauncher.GetComponent<SelectableObject>();
        if (selectable != null)
            SelectLauncher(selectable);
    }

    public void Deselect()
    {

        if (currentSelected != null)
        {
            currentSelected.HideHealthBar();
            Debug.Log($"CPU deselected launcher: {currentSelected.gameObject.name}");
            currentSelected = null;
        }
    }
}

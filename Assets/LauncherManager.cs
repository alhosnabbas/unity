using UnityEngine;
using System.Collections.Generic;
using TMPro;
public class LauncherManager : MonoBehaviour
{

    private int maxLaunchers = 4;
    private int maxLaunchersCPU = 4;
    private int currentLaunchers = 0;
    private int currentLaunchersCPU = 0;
    public bool isMaxReached = false;
    public bool isMaxReachedCPU = false;
    private PlayerTurn playerTurn;
    private List<Launcher> playerLaunchers = new List<Launcher>();
    private List<Launcher> cpuLaunchers = new List<Launcher>();

    private static TextMeshProUGUI errorText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //playerTurn = FindFirstObjectByType<PlayerTurn>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void Awake()
    {
        // Try to find manager if not assigned
        playerTurn = FindFirstObjectByType<PlayerTurn>();
        if (playerTurn == null) { }
        if (errorText == null)
        {
            GameObject errorText1 = GameObject.Find("Error Message");
            if (errorText1 != null)
                errorText = errorText1.GetComponent<TextMeshProUGUI>();
            errorText.text = ""; // Initialize error text to empty
        }
    }
    public void AddLauncher(Launcher launcher, bool isPlayer)
    {


        launcher.SetOwnership(isPlayer);

        if (isPlayer)
        {
            playerLaunchers.Add(launcher);
            currentLaunchers++;
        }
        else
        {
            cpuLaunchers.Add(launcher);
            addCPULauncher();
        }

        if (currentLaunchers >= maxLaunchers)
        {
            isMaxReached = true;
            //Debug.Log("Max launchers reached");
            return;
        }

        //Debug.Log(playerLaunchers.Count + " player launchers, " + cpuLaunchers.Count + " CPU launchers.");
    }
    public int getPlayerLaunchersCount()
    {
        return playerLaunchers.Count;
    }

    public int getCPULaunchersCount()
    {
        return cpuLaunchers.Count;
    }
    public void addCPULauncher()
    {
        if (currentLaunchersCPU < maxLaunchersCPU - 1)
        {
            currentLaunchersCPU++;
        }
        else
        {
            isMaxReachedCPU = true;
            // Debug.Log("Max CPU launchers reached");
        }
    }

    public bool returnMaxReached()
    {
        return isMaxReached;
    }

    public int getMaxLaunchersCPU()
    {
        return maxLaunchersCPU;
    }

    public Launcher[] getPlayerLaunchers()
    {
        playerLaunchers.RemoveAll(l => l == null); // cleanup destroyed ones
        return playerLaunchers.ToArray();
    }

    public Launcher[] getCPULaunchers()
    {
        cpuLaunchers.RemoveAll(l => l == null); // cleanup destroyed ones
        return cpuLaunchers.ToArray();
    }

    public void RemoveLauncher(Launcher launcher)
    {
        if (launcher == null) return;

        if (launcher.isPlayerOwned)
        {
            playerLaunchers.Remove(launcher);
            if (playerLaunchers.Count == 0)
            {

                errorText.text = "All your launchers are destroyed! Game Over!";

            }
        }
        else
        {
            cpuLaunchers.Remove(launcher);
            if (cpuLaunchers.Count == 0)
            {

                errorText.text = "All CPU launchers are destroyed! You Win!";

            }
        }
    }
}

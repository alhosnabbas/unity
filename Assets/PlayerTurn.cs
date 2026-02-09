using System.Collections;
using UnityEngine;

public class PlayerTurn : MonoBehaviour
{
    private int playerNumber = 1;
    private int numberOfShots = 0;

    private LauncherManager launcherManager;
    CPUManager cpuManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        launcherManager = FindFirstObjectByType<LauncherManager>();
        cpuManager = FindFirstObjectByType<CPUManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator WaitAndPrint(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        //Debug.Log("Waited for " + waitTime + " seconds.");
    }

    public void SwichTurn()
    {
        if (playerNumber == 1)
        {
            //Debug.Log("CPU Turn");
            WaitAndPrint(10f);
            playerNumber = 2;
            cpuManager.StartCPUTurn();

        }
        else
        {
            WaitAndPrint(10f);
            playerNumber = 1;
        }
    }

    public int GetCurrentPlayer()
    {
        return playerNumber;
    }

    public void AddShot()
    {
        numberOfShots++;
        if (numberOfShots == launcherManager.getPlayerLaunchersCount())
        {
            Debug.Log(launcherManager.getPlayerLaunchersCount());
            numberOfShots = 0;
            StartCoroutine(WaitAndSwitchTurn(5f));
            //SwichTurn();
        }
    }


    public void AddCPUShot()
    {
        numberOfShots++;
        if (numberOfShots == launcherManager.getCPULaunchersCount())
        {
            Debug.Log(launcherManager.getCPULaunchersCount() + "uuuuuuuuuuuuuuuuuu");
            numberOfShots = 0;
            StartCoroutine(WaitAndSwitchTurn(5f));
            //SwichTurn();
        }
    }

    private IEnumerator WaitAndSwitchTurn(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        SwichTurn();
    }
}

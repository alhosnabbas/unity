using UnityEngine;
using System.Collections;
using System;

public abstract class Launcher : MonoBehaviour
{
    [Header("Ownership")]
    public bool isPlayerOwned; // true = player, false = CPU

    public abstract IEnumerator Fire(Vector3 position, float someValue, System.Action onComplete);
    public virtual void SetOwnership(bool isPlayer)
    {
        Debug.Log(isPlayer);
        isPlayerOwned = isPlayer;
    } // every launcher must implement this
}

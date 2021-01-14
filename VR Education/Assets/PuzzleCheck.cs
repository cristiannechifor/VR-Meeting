using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCheck : MonoBehaviour
{
    public GameManager gameManager;
    public string puzzleObjectName;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger enter: " + other.name);
        if (other.name == puzzleObjectName)
            gameManager.PuzzlePieceCount(1);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger exit: " + other.name);
        if (other.name == puzzleObjectName)
            gameManager.PuzzlePieceCount(-1);
    }
}

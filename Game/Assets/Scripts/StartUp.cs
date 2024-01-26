using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUp : MonoBehaviour
{
    IGameManager gameManager;
    void Start()
    {
        gameManager.Init();
    }
}

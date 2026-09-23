using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{

    public void OnStartClick() => GameManager.Instance.StartGame();

    public void OnExitClick() => GameManager.Instance.ExitGame();
}

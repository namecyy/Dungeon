using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Victory : MonoBehaviour
{

    public void OnRestartClick() => GameManager.Instance.StartGame();

    public void OnMainMenuClick() => GameManager.Instance.MainMenu();

    public void OnExitClick() => GameManager.Instance.ExitGame();

}

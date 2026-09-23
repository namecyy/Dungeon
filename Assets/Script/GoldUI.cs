using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoldUI : MonoBehaviour
{
    private Text goldText;
    private void OnEnable()
    {
        goldText = GetComponent<Text>();
        goldText.text = "½ð±Ò£º" + PlayerData.Instance.GetGold();
        GameEvents.OnGoldChanged += UpdateGoldUI;
    }

    private void OnDisable()
    {
        GameEvents.OnGoldChanged -= UpdateGoldUI;
    }


    private void UpdateGoldUI(int gold)
    {
        goldText.text = "½ð±Ò£º" + gold;
    }
}

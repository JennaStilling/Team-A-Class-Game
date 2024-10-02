using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{

    public TextMeshProUGUI tokenText;
    public TextMeshProUGUI ticketText;
    public TextMeshProUGUI ammoText;

    void OnEnable()
    {
        GameManager.onValuesChanged += UpdateUI;
    }

    void OnDisable()
    {
        GameManager.onValuesChanged -= UpdateUI;
    }

    void Start()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        tokenText.text = "Tokens: " + GameManager.Instance.GetTokenValue();
        ticketText.text = "Tickets: " + GameManager.Instance.GetTicketValue();
        if (GameManager.Instance.GunUnlockedProp)
        {
            ammoText.text = "Ammo: " + GameManager.Instance.BlasterShotsProp;
        }
        else
        {
            ammoText.text = "";
        }
    }
}

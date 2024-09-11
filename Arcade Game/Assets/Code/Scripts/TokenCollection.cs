using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TokenCollection : MonoBehaviour
{
    private int Token = 0;

    public TextMeshProUGUI tokenText;


    private void OnTriggerEnter(Collider other)
    {
        
        if(other.transform.tag == "token")
        {
            Token++;
            tokenText.text = "Tokens: " + Token.ToString();
            Debug.Log(Token);
            Destroy(other.gameObject);
        }
    }

}

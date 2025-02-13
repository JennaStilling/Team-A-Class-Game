using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TokenCollection : MonoBehaviour
{
    private GameManager _gameManager;
    [SerializeField] private AudioSource _tokenCollect;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "token")
        {
            _gameManager.AddTokens(1);
            _tokenCollect.Play();
            Destroy(other.gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private int _ticketValue = 0;
    public int ticketValueProp
    {
        get { return _ticketValue; }
        set
        {
            _ticketValue = value;
            onValuesChanged?.Invoke();
        }
    }

    public int tokenValue = 10;

    public delegate void OnValuesChanged();
    public static event OnValuesChanged onValuesChanged;

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddTickets(int amount)
    {
        _ticketValue += amount;
        Debug.Log("Tickets Added: " + amount + ", Total Tickets: " + _ticketValue);

        onValuesChanged?.Invoke();
    }

    public bool SpendToken(int amount)
    {
        if (tokenValue >= amount)
        {
            tokenValue -= amount;
            Debug.Log("Tokens Spent: " + amount + ", Remaining Tokens: " + tokenValue);

            onValuesChanged?.Invoke();
            return true;
        }
        else
        {
            Debug.Log("Not enough tokens to play.");
            return false;
        }
    }

    public void AddTokens(int amount)
    {
        tokenValue += amount;
        Debug.Log("Tokens Added: " + amount + ", Total Tokens: " + tokenValue);

        onValuesChanged?.Invoke();
    }

    public int GetTicketValue()
    {
        return _ticketValue;
    }

    public int GetTokenValue()
    {
        return tokenValue;
    }

    public void StartGame() 
    {
        SceneManager.LoadScene("arcadeGame1");
    }

    public void QuitGame() 
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }
}

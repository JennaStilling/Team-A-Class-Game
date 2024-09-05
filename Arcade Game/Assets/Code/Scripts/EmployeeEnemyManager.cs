using UnityEngine;

public class EmployeeEnemyManager : MonoBehaviour
{
    private int _healthpoints;
    private int _tokensUponDeath;
    public bool isManager; // keep to have some employees be worth more, like managers?
    private bool _isDead = false;
    public bool _isUnderAttack {  get;  set; }= false;

    private void Awake()
    {
        _healthpoints = 30;
        _tokensUponDeath = Random.Range(1, 3);

        if (Random.Range(1, 10) >= 8)
            isManager = true;
    }

    public bool TakeDamage()
    {
        _healthpoints -= 10;
        _isDead = _healthpoints <= 0;
        if (_isDead) Die();
        return _isDead;
    }

    private void Die()
    {
        // drop tokens
        if (isManager)
            _tokensUponDeath *= 2;
        for (int i = 0; i < _tokensUponDeath; i++)
            Debug.Log("Spawning coin #"+i); // spawn coins at location of death
        Destroy(gameObject);
    }
}
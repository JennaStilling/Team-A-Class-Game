using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;
using UnityEngine.AI;

public class EmployeeTaskPatrol : Node
{
    private Transform _transform;
    private Animator _animator; // to be added later, see code below
    private Transform[] _waypoints;
    private EmployeeEnemyManager _selfManager;
    private NavMeshAgent _navMeshAgent;

    private int _currentWaypointIndex = 0;

    private float _waitTime = 1f; // in seconds
    private float _waitCounter = 0f;
    private bool _waiting = false;

    public EmployeeTaskPatrol(Transform transform, Transform[] waypoints)
    {
        _transform = transform;
        // initialize animator
        _waypoints = waypoints;
        _selfManager = _transform.GetComponent<EmployeeEnemyManager>();
        _navMeshAgent = _selfManager.GetComponent<NavMeshAgent>();
    }

    public override NodeState Evaluate()
    {
        if (_waiting)
        {
            _waitCounter += Time.deltaTime;
            if (_waitCounter >= _waitTime)
            {
                _waiting = false;
                // animation code - walking
            }
        }
        else
        {
            Transform wp = _waypoints[_currentWaypointIndex];
            if (Vector3.Distance(_navMeshAgent.transform.position, wp.position) < 1f)
            {
                _navMeshAgent.destination = wp.position;
                _waitCounter = 0f;
                _waiting = true;
                //Debug.Log("Current index: " + _currentWaypointIndex);
                _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
                // animation code - no longer walking
            }
            else
            {
                //bool temp = _navMeshAgent.SetDestination(wp.position);
                //Debug.Log(temp);
                _navMeshAgent.destination = wp.position;
                // _transform.position = Vector3.MoveTowards(
                //     _transform.position, wp.position, EmployeeBT.speed * Time.deltaTime);
                //_transform.LookAt(wp.position);
                //Debug.Log("Current index: " + _currentWaypointIndex);
            }
        }


        state = NodeState.RUNNING;
        return state;
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class EmployeeTaskPatrol : Node
{
    private Transform _transform;
    private Animator _animator; // to be added later, see code below
    private Transform[] _waypoints;

    private int _currentWaypointIndex = 0;

    private float _waitTime = 1f; // in seconds
    private float _waitCounter = 0f;
    private bool _waiting = false;

    public EmployeeTaskPatrol(Transform transform, Transform[] waypoints)
    {
        _transform = transform;
        // initialize animator
        _waypoints = waypoints;
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
            if (Vector3.Distance(_transform.position, wp.position) < 0.01f)
            {
                _transform.position = wp.position;
                _waitCounter = 0f;
                _waiting = true;

                _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
                // animation code - no longer walking
            }
            else
            {
                _transform.position = Vector3.MoveTowards(
                    _transform.position, wp.position, EmployeeBT.speed * Time.deltaTime);
                _transform.LookAt(wp.position);
            }
        }


        state = NodeState.RUNNING;
        return state;
    }

}
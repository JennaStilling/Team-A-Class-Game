using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;
using UnityEngine.AI;

public class TaskGoToTarget : Node
{
    private Transform _transform;
    private Animator _animator;
    private EmployeeEnemyManager _selfManager;
    private NavMeshAgent _navMeshAgent;
    

    public TaskGoToTarget(Transform transform)
    {
        _transform = transform;
        // initialize animator
        _selfManager = _transform.GetComponent<EmployeeEnemyManager>();
        _navMeshAgent = _selfManager.GetComponent<NavMeshAgent>();
    }

    public override NodeState Evaluate()
    {
        //Debug.Log("In go to target task");
        Transform target = (Transform)GetData("target");

        if (Vector2.Distance(_transform.position, target.position) > _navMeshAgent.stoppingDistance)
        {
            //Debug.Log("Not at target");
            _navMeshAgent.destination = target.position;
            _transform.LookAt(new Vector3(target.position.x, _transform.position.y, target.position.z));
            // set animation - walking
        }

        state = NodeState.RUNNING;
        return state;
    }

}
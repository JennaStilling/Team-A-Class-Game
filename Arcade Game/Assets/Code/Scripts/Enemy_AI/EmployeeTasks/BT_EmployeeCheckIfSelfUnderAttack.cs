using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class EmployeeCheckIfSelfUnderAttack : Node
{
    private Transform _transform;
    private EmployeeEnemyManager _selfManager;
    
    public EmployeeCheckIfSelfUnderAttack(Transform transform)
    {
        _transform = transform;
        _selfManager = _transform.GetComponent<EmployeeEnemyManager>();
    }
    public override NodeState Evaluate()
    {
        if (_selfManager.isUnderAttack)
        {
            parent.parent.SetData("target", GameObject.Find("Player").transform); // rewrite this line later to get reference to player instead of ... could be a reference to what object set off the notify alert
            return NodeState.SUCCESS;
        }
        return NodeState.FAILURE;
    }
}
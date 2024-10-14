using System.Collections.Generic;
using BehaviorTree;
using UnityEngine;
using Tree = BehaviorTree.Tree;

public class EmployeeBT : Tree
{
    public UnityEngine.Transform[] waypoints; // can change in future if / when we want enemy to randomly walk around arcade
    
    public static float speed = 2f;
    public static float fovRange = 4f;
    public static float attackRange = 4f;

    protected override Node SetupTree()
    {
       Node root = new Selector(new List<Node>
       {
           new Sequence(new List<Node> // am I under attack?
           {
               new EmployeeCheckIfSelfUnderAttack(transform),
               new TaskGoToTarget(transform),
           }),
           new Sequence(new List<Node> // am I attacking?
           {
               new EmployeeCheckIfSelfUnderAttack(transform),
               new EmployeeCheckEnemyInAttackRange(transform),
               new EmployeeTaskAttack(transform),
           }),
           new EmployeeTaskPatrol(transform, waypoints), // default
       });
       return root;
    }
    
    public void SetSpeed(float s)
    {
        speed = s;
    }
}
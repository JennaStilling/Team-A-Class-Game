using System.Collections.Generic;
using BehaviorTree;

public class EmployeeBT : Tree
{
    public UnityEngine.Transform[] waypoints; // can change in future if / when we want enemy to randomly walk around arcade

    public static float speed = 2f;
    public static float fovRange = 4f;
    public static float attackRange = 2f;

    protected override Node SetupTree()
    {
        Node root = new Selector(new List<Node>
         {
             new Sequence(new List<Node>
             {
                 new EmployeeCheckEnemyInAttackRange(transform),
                 new EmployeeTaskAttack(transform),
             }),
             new Sequence(new List<Node>
             {
                 new EmployeeCheckEnemyInFOVRange(transform),
                 new TaskGoToTarget(transform),
             }),
             new EmployeeTaskPatrol(transform, waypoints), // default - will walk if player not in range
         });

        return root; 
       /*Node root = new Selector(new List<Node>
       {
           new Sequence(new List<Node> // am I under attack?
           {
               // check if under attack,
               // new TaskGoToTarget(transform),
           }),
           new Sequence(new List<Node> // am I attacking?
           {
               // check if under attack,
               // new EmployeeCheckEnemyInAttackRange(transform),
               // new EmployeeTaskAttack(),
           }),
           new EmployeeTaskPatrol(transform, waypoints), // default
       });
       return root;*/
    }
    
    
    /* TODO
     -Rewrite task lists / sequences
     -Update names of all tasks and find common naming scheme
     -Review code and make modifications as needed
     */
    
    public void SetSpeed(float s)
    {
        speed = s;
    }
}
using UnityEngine;
using System.Collections.Generic;

public class RandomWaypointState : IState
{
    private readonly BossController _boss;
    private List<Transform> _shuffledWaypoints;
    private int _currentIndex;
    
    public RandomWaypointState(BossController boss)
    {
        _boss = boss;
    }

    public void Enter()
    {
        if (_boss.waypoints == null || _boss.waypoints.Length == 0) 
        {
            Debug.LogError("No waypoints assigned to BossController.");
            return;
        }

        // Initialize and shuffle the waypoints using Fisher-Yates
        _shuffledWaypoints = new List<Transform>(_boss.waypoints);
        ShuffleWaypoints();
        _currentIndex = 0;
    }

    public void Tick()
    {
        if (_shuffledWaypoints.Count == 0) return;

        Transform target = _shuffledWaypoints[_currentIndex];

        // Move towards the target at a constant speed
        _boss.transform.position = Vector3.MoveTowards(
            _boss.transform.position, 
            target.position, 
            _boss.moveSpeed * Time.deltaTime
        );

        // Use sqrMagnitude instead of Vector3.Distance to avoid an expensive square root operation
        if ((_boss.transform.position - target.position).sqrMagnitude < 0.01f)
        {
            _currentIndex++;

            if (_currentIndex >= _shuffledWaypoints.Count)
            {
                _boss.SwitchState(new RandomWaypointState(_boss)); 
            }
        }
    }

    public void Exit()
    {
        _shuffledWaypoints?.Clear();
    }

    private void ShuffleWaypoints()
    {
        // Standard Fisher-Yates shuffle algorithm (O(n) time complexity)
        for (int i = _shuffledWaypoints.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Transform temp = _shuffledWaypoints[i];
            _shuffledWaypoints[i] = _shuffledWaypoints[randomIndex];
            _shuffledWaypoints[randomIndex] = temp;
        }
    }
}
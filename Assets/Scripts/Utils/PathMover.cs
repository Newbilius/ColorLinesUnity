using System;
using System.Collections.Generic;
using UnityEngine;

public class PathMover
{
    Queue<Vector3> path = new Queue<Vector3>();

    public float Speed;
    private GameObject GameObject;
    private Action OnMoveComplete;
    private Action<Vector3Combined> OnStepComplete;
    public bool Moving;

    public PathMover(GameObject gameObject, float speed)
    {
        GameObject = gameObject;
        Speed = speed;
    }

    public void MoveByPath(Vector3[] newPath,
        Action onMoveComplete,
        Action<Vector3Combined> onStepComplete)
    {
        OnMoveComplete = onMoveComplete;
        OnStepComplete = onStepComplete;
        foreach (var step in newPath)
            path.Enqueue(step);
    }

    public void Update()
    {
        if (path.Count == 0)
        {
            Moving = false;
            return;
        }
        Moving = true;

        var newPosition = path.Peek();
        GameObject.transform.position = Vector3.MoveTowards(GameObject.transform.position,
                 newPosition,
                 Speed * Time.deltaTime);

        if (GameObject.transform.position == newPosition)
        {
            path.Dequeue();
            if (path.Count > 0)
                OnStepComplete(new Vector3Combined
                {
                    Start = newPosition,
                    End = path.Peek()
                });
        }

        if (path.Count == 0)
        {
            Moving = false;
            OnMoveComplete();
            OnMoveComplete = null;
            OnStepComplete = null;
        }
    }

    public bool IsMoving()
    {
        return Moving;
    }
}
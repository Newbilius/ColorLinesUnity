using System;
using UnityEngine;

public class GameBoardManager : MonoBehaviour
{
    public static GameBoardManager Instance;

    public GameObject ballSpace;
    public GameObject gameField;
    public GameObject ballExample;

    Vector3 leftTopAngle;
    float xPadding;
    float yPadding;
    public BallBehavior[,] field = new BallBehavior[9, 9];

    void Awake()
    {
        Instance = this;
        var gameFieldSpriteRenderer = gameField.GetComponent<SpriteRenderer>();
        var ballSpriteRenderer = ballExample.GetComponent<SpriteRenderer>();
        leftTopAngle = Helpers.GetTopLeftPosition(gameFieldSpriteRenderer, ballSpriteRenderer);

        xPadding = gameFieldSpriteRenderer.bounds.size.x / 9;
        yPadding = -gameFieldSpriteRenderer.bounds.size.y / 9;

        PrepareField();
    }

    private void PrepareField()
    {
        for (int x = 0; x < 9; x++)
            for (int y = 0; y < 9; y++)
                Instantiate(ballSpace, GetPos(x, y), Quaternion.identity);
    }

    public Vector3 GetPos(int x, int y)
    {
        return new Vector3(leftTopAngle.x + x * xPadding, leftTopAngle.y + y * yPadding);
    }

    public Point ScreenCoordToBoardCoords(Vector3 position)
    {
        var xCoord = (int)Math.Round((position.x - leftTopAngle.x) / xPadding);
        var yCoord = (int)Math.Round((position.y - leftTopAngle.y) / yPadding);
        return new Point(xCoord, yCoord);
    }
}
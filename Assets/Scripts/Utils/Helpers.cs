using System;
using UnityEngine;

public static class Helpers
{
    public static Vector2 GetTopLeftPosition(SpriteRenderer field, SpriteRenderer gameObject)
    {
        return new Vector2(field.transform.position.x - field.bounds.extents.x + gameObject.bounds.extents.x,
                        field.transform.position.y + field.bounds.extents.y - gameObject.bounds.extents.y);
    }

    public static bool IntToBool(this int value)
    {
        return value == 1 ? true : false;
    }

    public static void ForceLandscapeOrientation()
    {
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.orientation = ScreenOrientation.AutoRotation;
    }

    public static int BoolToInt(this bool value)
    {
        return value ? 1 : 0;
    }

    public static void Set2DCameraToObject(GameObject field)
    {
        var size1 = field.GetComponent<SpriteRenderer>().bounds.size.y
            * Screen.height
            / Screen.width * 0.5f * Camera.main.aspect; ;

        var size2 = field.GetComponent<SpriteRenderer>().bounds.size.x
            * Screen.height
            / Screen.width * 0.5f; ;

        Camera.main.orthographicSize = Math.Max(size1, size2);
    }
}
using UnityEngine;
using System.Collections;

public class CurveGenerator
{
    private const float RIGHT_ANGLE = 0.5f * Mathf.PI;

    public static Vector2[] GenerateCurved(float radius, float segmentLength, Vector2 direction, Vector2 offset)
    {
        float totalArcLength = RIGHT_ANGLE * Mathf.Abs(radius);
        int segmentCount = (int)(totalArcLength / segmentLength);
        float segmentAngle = RIGHT_ANGLE / segmentCount;
        return GenerateAllCurvePoints(segmentCount, segmentAngle, radius, direction, offset);
    }

    private static Vector2[] GenerateAllCurvePoints(int segmentCount, float segmentAngle, float radius, Vector2 direction, Vector2 offset)
    {
        Vector2[] points = new Vector2[segmentCount + 1];
        
        //points[0] = new Vector2(radius, 0);
        for (int i = 0; i < points.Length; i++)
            points[i] = GenerateCurvePoint(i, segmentAngle, radius, direction, offset);

        return points;
    }

    private static Vector2 GenerateCurvePoint(int index, float angle, float radius, Vector2 direction, Vector2 offset)
    {
        float theta = index * angle;
        float x = (direction.x * radius * Mathf.Cos(theta)) + offset.x;
        float y = (direction.y * radius * Mathf.Sin(theta)) + offset.y;
        return new Vector2(x, y);
    }
}


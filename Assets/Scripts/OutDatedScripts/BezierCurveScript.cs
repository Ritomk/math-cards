using UnityEngine;

[ExecuteAlways]
public class BezierCurveScript : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    [Range(0f, 1f)]
    public float t = 0f;

    private Vector3 currentPosition;

    void Update()
    {
        // Calculate current position
        currentPosition = Vector3.Lerp(startPoint.position, endPoint.position, t);

        // Update this object's position
        transform.position = currentPosition;
    }

    void OnDrawGizmos()
    {
        if (startPoint == null || endPoint == null) return;

        // Draw the line
        Gizmos.color = Color.white;
        Gizmos.DrawLine(startPoint.position, endPoint.position);

        // Draw the start and end points
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(startPoint.position, 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(endPoint.position, 0.1f);

        // Draw the current position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(currentPosition, 0.15f);
    }
}
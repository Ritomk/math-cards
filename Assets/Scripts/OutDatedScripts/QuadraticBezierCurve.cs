using UnityEngine;


public class QuadraticBezierCurve : MonoBehaviour
{
    public Transform startPoint;
    public Transform controlPoint;
    public Transform endPoint;
    [Range(0f, 1f)]
    public float t = 0f;

    private Vector3 currentPosition;
    public GameObject card;

    void Update()
    {
        // Calculate current position using quadratic Bezier formula
        currentPosition = CalculateQuadraticBezierPoint(t, startPoint.position, controlPoint.position, endPoint.position);

        // Update this object's position
        transform.position = currentPosition;

        if(Input.GetKeyDown(KeyCode.Space))
        {
            var cardD = Instantiate(card);
            cardD.transform.position = currentPosition;
            t += 0.1f;
        }
    }

    Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0; // (1-t)^2 * P0
        p += 2 * u * t * p1; // 2(1-t)t * P1
        p += tt * p2; // t^2 * P2

        return p;
    }

    void OnDrawGizmos()
    {
        if (startPoint == null || controlPoint == null || endPoint == null) return;

        // Draw the Bezier curve
        Gizmos.color = Color.white;
        Vector3 prevPoint = startPoint.position;
        for (float i = 0; i <= 1; i += 0.05f)
        {
            Vector3 point = CalculateQuadraticBezierPoint(i, startPoint.position, controlPoint.position, endPoint.position);
            Gizmos.DrawLine(prevPoint, point);
            prevPoint = point;
        }

        // Draw the control lines
        Gizmos.color = Color.gray;
        Gizmos.DrawLine(startPoint.position, controlPoint.position);
        Gizmos.DrawLine(controlPoint.position, endPoint.position);

        // Draw the points
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(startPoint.position, 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(controlPoint.position, 0.1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(endPoint.position, 0.1f);

        // Draw the current position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(currentPosition, 0.15f);
    }
}
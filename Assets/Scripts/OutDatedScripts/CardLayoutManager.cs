using UnityEngine;

public class CardLayoutManager : MonoBehaviour
{
    public Transform pointX1;
    public Transform pointX2;
    public Transform pointY;
    public GameObject prefab; // Prefab do instancji
    public int resolution = 50;
    public Color arcColor = Color.yellow;

    private Vector3 prevPointX1Pos, prevPointX2Pos, prevPointYPos;
    private float ax, bx, cx;
    private float ay, by, cy;
    private float az, bz, cz;
    private int currentStep = 0;
    private bool nextOnX1 = true;

    void Start()
    {
        prevPointX1Pos = pointX1.position;
        prevPointX2Pos = pointX2.position;
        prevPointYPos = pointY.position;

        CalculateCoefficients();
    }

    void Update()
    {
        if (pointX1.position != prevPointX1Pos || pointX2.position != prevPointX2Pos || pointY.position != prevPointYPos)
        {
            UpdateSymmetricalPositions();
            CalculateCoefficients();
            prevPointX1Pos = pointX1.position;
            prevPointX2Pos = pointX2.position;
            prevPointYPos = pointY.position;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddPrefabOnArc();
        }
    }

    void CalculateCoefficients()
    {
        Vector3 p1 = pointX1.position;
        Vector3 p2 = pointX2.position;
        Vector3 p = pointY.position;

        // Oblicz wspó³czynniki dla funkcji kwadratowej w osi X: x = ax*t^2 + bx*t + cx
        cx = p.x;
        float denominatorX = (p1.x - p2.x) * (p1.x - 0) * (p2.x - 0);
        if (Mathf.Abs(denominatorX) > Mathf.Epsilon) // Zapobieganie dzieleniu przez zero
        {
            ax = (p2.x * (p1.x - p.x) + p1.x * (p.x - p2.x)) / denominatorX;
            bx = (p1.x - p.x - ax * p1.x * p1.x) / p1.x;
        }
        else
        {
            Debug.LogError("Error: Denominator for X calculation is zero, cannot compute coefficients.");
            ax = bx = 0;
        }

        // Oblicz wspó³czynniki dla funkcji kwadratowej w osi Y: y = ay*t^2 + by*t + cy
        cy = p.y;
        float denominatorY = (p1.x - p2.x) * (p1.x - 0) * (p2.x - 0);
        if (Mathf.Abs(denominatorY) > Mathf.Epsilon) // Zapobieganie dzieleniu przez zero
        {
            ay = (p2.x * (p1.y - p.y) + p1.x * (p.y - p2.y)) / denominatorY;
            by = (p1.y - p.y - ay * p1.x * p1.x) / p1.x;
        }
        else
        {
            Debug.LogError("Error: Denominator for Y calculation is zero, cannot compute coefficients.");
            ay = by = 0;
        }

        // Oblicz wspó³czynniki dla funkcji kwadratowej w osi Z: z = az*t^2 + bz*t + cz
        cz = p.z;
        float denominatorZ = (p1.x - p2.x) * (p1.x - 0) * (p2.x - 0);
        if (Mathf.Abs(denominatorZ) > Mathf.Epsilon) // Zapobieganie dzieleniu przez zero
        {
            az = (p2.x * (p1.z - p.z) + p1.x * (p.z - p2.z)) / denominatorZ;
            bz = (p1.z - p.z - az * p1.x * p1.x) / p1.x;
        }
        else
        {
            Debug.LogError("Error: Denominator for Z calculation is zero, cannot compute coefficients.");
            az = bz = 0;
        }

        // Wypisz wzory funkcji w konsoli
        Debug.Log($"Funkcja X: x = {ax}*t^2 + {bx}*t + {cx}");
        Debug.Log($"Funkcja Y: y = {ay}*t^2 + {by}*t + {cy}");
        Debug.Log($"Funkcja Z: z = {az}*t^2 + {bz}*t + {cz}");
    }


    void OnDrawGizmos()
    {
        if (pointX1 == null || pointX2 == null || pointY == null) return;

        Gizmos.color = arcColor;

        float x1 = pointX1.position.x;
        float x2 = pointX2.position.x;

        Vector3 prevPoint = Vector3.zero;
        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            float x = ax * t * t + bx * t + cx;
            float y = ay * t * t + by * t + cy;
            float z = az * t * t + bz * t + cz;
            Vector3 point = new Vector3(x, y, z);

            if (i > 0)
            {
                Gizmos.DrawLine(prevPoint, point);
            }
            prevPoint = point;
        }

        // Rysowanie punktów kontrolnych
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(pointX1.position, 0.1f);
        Gizmos.DrawSphere(pointX2.position, 0.1f);
        Gizmos.DrawSphere(pointY.position, 0.1f);
    }

    void UpdateSymmetricalPositions()
    {
        Vector3 x1Pos = pointX1.position;
        Vector3 x2Pos = pointX2.position;

        if (x1Pos != prevPointX1Pos)
        {
            x2Pos = new Vector3(-x1Pos.x, x2Pos.y, x2Pos.z);
        }
        else if (x2Pos != prevPointX2Pos)
        {
            x1Pos = new Vector3(-x2Pos.x, x1Pos.y, x1Pos.z);
        }

        pointX1.position = x1Pos;
        pointX2.position = x2Pos;
    }

    void AddPrefabOnArc()
    {
        float t = (currentStep / 2 + 1) * 0.5f;
        if (!nextOnX1) t = -t;

        float x = ax * t * t + bx * t + cx;
        float y = ay * t * t + by * t + cy;
        float z = az * t * t + bz * t + cz;
        Vector3 position = new Vector3(x, y, z);

        Instantiate(prefab, position, Quaternion.identity);

        currentStep++;
        nextOnX1 = !nextOnX1;
    }
}

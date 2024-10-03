using UnityEngine;
using UnityEditor;
using Unity.VisualScripting.FullSerializer;

[ExecuteAlways]
public class QuadraticFunctionVisualizer : MonoBehaviour
{
    [SerializeField] private QuadraticFunctionCalculator calculator;
    [SerializeField] private bool visualizePoints = true;
    [SerializeField] private bool visualizeFunction = true;
    [SerializeField] private int pointCount = 100;
    //public float visualizationWidth = 10f;
    [SerializeField] private Color functionColor = Color.red;
    [SerializeField] Color pointColor = Color.blue;
    

    private Vector3[] linePoints;
    private Vector3[] visualizedPoints;

    private void Update()
    {
        if (calculator != null)
        {
            UpdateVisualization();
        }
    }

    private void UpdateVisualization()
    {
        if (visualizeFunction)
        {
            VisualizeFunction(calculator.a, calculator.b, calculator.c);
        }

        if (visualizePoints)
        {
            VisualizePoints();
        }
    }

    private void VisualizeFunction(float a, float b, float c)
    {
        linePoints = new Vector3[pointCount];
        float visualizationWidth = (calculator.pointX1.position - calculator.pointX2.position).magnitude;
        float step = visualizationWidth / (pointCount - 1);

        for (int i = 0; i < pointCount; i++)
        {
            float x = -visualizationWidth / 2 + i * step;
            float y = a * x * x + b * x + c;
            linePoints[i] = new Vector3(x, y, 0);
        }
    }

    private void VisualizePoints()
    {
        if (calculator.pointX1 != null && calculator.pointX2 != null && calculator.pointY != null)
        {
            visualizedPoints = new Vector3[]
            {
                calculator.pointX1.position,
                calculator.pointX2.position,
                calculator.pointY.position
            };
        }
    }

    private void OnDrawGizmos()
    {
        if (visualizeFunction && linePoints != null && linePoints.Length > 1)
        {
            Gizmos.color = functionColor;
            for (int i = 0; i < linePoints.Length - 1; i++)
            {
                Gizmos.DrawLine(linePoints[i], linePoints[i + 1]);
            }
        }

        if (visualizePoints && visualizedPoints != null)
        {
            Gizmos.color = pointColor;
            foreach (var point in visualizedPoints)
            {
                Gizmos.DrawSphere(point, 0.1f);
            }
        }
    }
}
using UnityEngine;
using UnityEditor;

[ExecuteAlways]
public class QuadraticFunctionCalculator : MonoBehaviour
{
    [SerializeField] private bool updateRealTime = false;

    public Transform pointX1;
    public Transform pointX2;
    public Transform pointY;
    private Vector3 prevPointX1Pos, prevPointX2Pos, prevPointYPos;

    [HideInInspector] public float a { get; private set; }
    [HideInInspector] public float b { get; private set; }
    [HideInInspector] public float c { get; private set; }
    
    private void Update()
    {
        if (ArePointsAssigned())
            return;

        if (updateRealTime) 
            CalculateFunction();

        UpdateSymmetricalPosition();
    }

    public bool ArePointsAssigned()
    {
        return !pointX1 || !pointX2 || !pointY;
    }

    public void CalculateFunction()
    {
        float x1 = pointX1.position.x;
        float x2 = pointX2.position.x;
        float y = pointY.position.y;

        a = -y / ((x1 - x2) * (x1 - x2) / 4);
        b = -a * (x1 + x2);
        c = y - a * (x1 + x2) * (x1 + x2) / 4;

        string functionString = string.Format("f(x) = {0:F2}x² + {1:F2}x + {2:F2}", a, b, c);

        Debug.Log("Obliczona funkcja: " + functionString);
    }

    private void UpdateSymmetricalPosition()
    {
        Vector3 x1Pos = pointX1.position;
        Vector3 x2Pos = pointX2.position;
        Vector3 yPos = pointY.position;

        if (x1Pos != prevPointX1Pos)
        {
            x2Pos = 2 * yPos - x1Pos;
            x2Pos.y = x1Pos.y;
            pointX2.position = x2Pos;
            //Debug.Log($"Update1 |x1Pos {x1Pos} | prevPointX1Pos {prevPointX1Pos}");
        }
        else if (x2Pos != prevPointX2Pos)
        {
            x1Pos = 2 * yPos - x2Pos;
            x1Pos.y = x2Pos.y;
            pointX1.position = x1Pos;
            //Debug.Log($"Update2 |x2Pos {x2Pos} | prevPointX2Pos {prevPointX2Pos}");
        }
        else if (yPos != prevPointYPos)
        {
            Vector3 offset = yPos - prevPointYPos;
            x1Pos += offset;
            x2Pos += offset;
            pointX1.position = x1Pos;
            pointX2.position = x2Pos;
            //Debug.Log($"Update3 |yPos {yPos} | prevPointYPos {prevPointYPos} | offset {offset}");
        }

        prevPointX1Pos = x1Pos;
        prevPointX2Pos = x2Pos;
        prevPointYPos = yPos;
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(QuadraticFunctionCalculator))]
public class QuadraticFunctionCalculatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        QuadraticFunctionCalculator calculator = (QuadraticFunctionCalculator)target;

        if (GUILayout.Button("Calculate function"))
        {
            if (calculator.ArePointsAssigned())
            {
                Debug.LogError("Nie wszystke punkty zostaly przypisane");
                return;
            }

            calculator.CalculateFunction();
        }
    }
}
#endif
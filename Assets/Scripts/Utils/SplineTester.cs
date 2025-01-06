using UnityEngine;
using UnityEngine.Splines;


[ExecuteInEditMode]
public class SplineTester : MonoBehaviour
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private GameObject objectToMove;
    [Range(0, 1)] [SerializeField] private float positionOnSpline = 0f;

    private void OnValidate()
    {
        if (splineContainer != null && objectToMove != null)
        {
            UpdateObjectPosition();
        }
    }

    private void UpdateObjectPosition()
    {
        if (splineContainer == null || objectToMove == null) return;
        
        Vector3 position = splineContainer.Spline.EvaluatePosition(positionOnSpline);

        objectToMove.transform.localPosition = position;
    }
}

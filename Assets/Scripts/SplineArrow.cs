using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineArrow : MonoBehaviour
{
    public SplineContainer splineContainer; // Reference to the SplineContainer
    public Transform arrowStartPoint; // The start point of the arrow
    public float curveSideOffset = 2.0f; // How much the arrow bends sideways on the XZ plane
    public float straightThreshold = 0.1f; // Threshold for determining when the line should be straight

    public LayerMask raycastLayerMask; // To define which objects the raycast should interact with (e.g., ground or 3D objects)

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Perform a raycast from the mouse position to find the end point on a physical object
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Vector3 endPosition;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, raycastLayerMask))
        {
            endPosition = hit.point; // Use the raycast hit position as the end point
        }
        else
        {
            return; // If the raycast doesn't hit anything, don't update the spline
        }

        // Create a 2-point spline from the object center to the raycast hit position
        if (splineContainer != null)
        {
            // Clear any previous knots from the spline
            splineContainer.Spline.Clear();

            // Start position (XZ plane)
            Vector3 startPosition = arrowStartPoint.position;

            // Ensure that the endPosition is on the XZ plane (Y stays the same)
            endPosition.y = startPosition.y; // Match Y-axis of end position with start position (so it's flat in XZ)

            // Create the start and end knots (positions)
            BezierKnot startKnot = new BezierKnot(startPosition);
            BezierKnot endKnot = new BezierKnot(endPosition);

            // Determine if the line should be straight (i.e., when aligned along X or Z axis)
            if (Mathf.Abs(startPosition.x - endPosition.x) < straightThreshold ||
                Mathf.Abs(startPosition.z - endPosition.z) < straightThreshold)
            {
                // The points are aligned along X or Z axis; make the spline straight
                startKnot.TangentOut = (endPosition - startPosition) * 0.5f; // Straight tangents
                endKnot.TangentIn = (startPosition - endPosition) * 0.5f;
            }
            else
            {
                // The points are not aligned; we need to curve the spline
                // Calculate a midpoint for the curve, offset to the side (XZ plane)
                Vector3 midPoint = (startPosition + endPosition) / 2;

                // Calculate the perpendicular direction on XZ plane to determine left or right bend
                Vector3 perpendicularDirection = Vector3.Cross(endPosition - startPosition, Vector3.up).normalized;

                // Check if the curve should bend left or right
                float direction = Mathf.Sign(Vector3.Dot(perpendicularDirection, Vector3.right));

                // Adjust the midpoint sideways based on the sign of the direction
                midPoint += perpendicularDirection * curveSideOffset * direction; 

                // Set tangents to curve the spline sideways (on the XZ plane)
                startKnot.TangentOut = (midPoint - startPosition) * 0.5f; // Curve out from the start point
                endKnot.TangentIn = (midPoint - endPosition) * 0.5f; // Curve in towards the end point
            }

            // Add the knots to the spline
            splineContainer.Spline.Add(startKnot);
            splineContainer.Spline.Add(endKnot);
        }
    }
}

using UnityEngine;
using UnityEngine.Splines;

public class HandContainer : CardContainerBase
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private float initialSpacing = 1f;
    private Vector3 _centerPosition;

    private void Start()
    {
        Vector3 firstPoint = splineContainer.transform.TransformPoint(splineContainer.Spline[0].Position);
        Vector3 lastPoint = splineContainer.transform.TransformPoint(splineContainer.Spline[splineContainer.Spline.Count - 1].Position);
        _centerPosition = (firstPoint + lastPoint) / 2;
        _centerPosition -= new Vector3(0, 3, 0);
    }

    public override bool AddCard(Card card)
    {
        bool result = base.AddCard(card);
        if (result)
        {
            UpdateCardPositions();
        }
        
        return result;
    }

    public override bool RemoveCard(int cardId)
    {
        bool result = base.RemoveCard(cardId);
        if (result)
        {
            UpdateCardPositions();
        }
        
        return result;
    }

    private void UpdateCardPositions()
    {
        int count = cardsDictionary.Count;
        if (count == 0) return;

        float totalSpacing = initialSpacing * (count - 1);
        float splineLength = splineContainer.Spline.GetLength();
        float scaleFactor = (totalSpacing > splineLength) ? (splineLength / totalSpacing) : 1f;

        int i = 0;
        foreach(var cardEntry in cardsDictionary.Values)
        {
            Transform cardTransform = cardEntry.transform;
            float offset = i - (count - 1) / 2f;
            float t = 0.5f + (offset * initialSpacing * scaleFactor) / splineLength;
            t = Mathf.Clamp01(t);
            Vector3 position = splineContainer.Spline.EvaluatePosition(t);
            cardTransform.localPosition = position;
            cardTransform.LookAt(_centerPosition);
            i++;
        }
    }
}
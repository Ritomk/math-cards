// using System.Collections;
// using System.Collections.Generic;
// using UnityEditor.Rendering;
// using UnityEngine;
// using UnityEngine.InputSystem;
//
// public class Hand : MonoBehaviour
// {
//     [SerializeField] private Camera cam;
//     [SerializeField] private GameObject cardPrefab;
//     [SerializeField] private List <Transform> cards = new List<Transform>();
//     [SerializeField] private Transform centerPointTransform;
//     [SerializeField] private float startAngle = 30f;
//     [SerializeField] private float finalAngle = 70f;
//     [SerializeField] private int finalNumberOfCards = 13;
//     [SerializeField] private float radius = 5f;
//     [SerializeField] private int selectedIndex = 0;
//
//     [SerializeField]private bool enableCardsSelect = false;
//     private GameObject lastHitObject = null; // Pami�� ostatniego trafionego obiektu
//
//     void Start()
//     {
//         StartCoroutine(GiveCards(13));
//         //testPoint.transform.LookAt(centerPointTransform);
//         //CalculateAngleForPosition(testPoint.transform.position, centerPointTransform, radius);
//     }
//
//     void Update()
//     {
//         PickUpCards();
//
//         if (Input.GetKeyDown(KeyCode.Space))
//         {
//             var newCard = Instantiate(cardPrefab);
//             newCard.transform.parent = transform;
//             cards.Add(newCard.transform);
//             ArrangeCards();
//         }
//         if(Input.GetKeyDown(KeyCode.LeftControl))
//         {
//             Destroy(cards[cards.Count - 1].gameObject);
//             cards.Remove(cards[cards.Count - 1]);
//             ArrangeCards();
//         }
//     }
//
//     private void PickUpCards()
//     {
//         if (cam == null)
//         {
//             Debug.LogError("cam is not set. Please assign it in the Unity Inspector.");
//             return;
//         }
//
//         RaycastHit hit;
//         Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
//
//         if (Physics.Raycast(ray, out hit))
//         {
//             if (lastHitObject != hit.collider.gameObject)
//             {
//                 if (lastHitObject != null)
//                 {
//                     PutCardDown(lastHitObject);
//                 }
//
//                 lastHitObject = hit.collider.gameObject;
//                 PutCardUp(lastHitObject);
//             }
//         }
//         else
//         {
//             if (lastHitObject != null)
//             {
//                 PutCardDown(lastHitObject);
//                 lastHitObject = null;
//             }
//         }
//     }
//
//     private void PutCardUp(GameObject obj)
//     {
//         obj.GetComponent<Card>().VisualPosition += new Vector3(0, 2f, 0);
//         //Transform cardTransform = obj.GetComponent<Card>().visualTransform;
//         //cardTransform.position += new Vector3(0, 2f, 0);
//         //cardTransform.transform.LookAt(centerPointTransform.position);
//     }
//
//     private void PutCardDown(GameObject obj)
//     {
//         obj.GetComponent<Card>().VisualPosition -= new Vector3(0, 2f, 0);
//         //Transform cardTransform = obj.GetComponent<Card>().visualTransform;
//         //cardTransform.position -= new Vector3(0, 2f, 0);
//         //cardTransform.transform.localRotation = Quaternion.Euler(0, 0, 0);
//     }
//
//     private IEnumerator GiveCards(int numberOfCards)
//     {
//         for(int i = 0; i < numberOfCards; i++)
//         {
//             var newCard = Instantiate(cardPrefab);
//             newCard.transform.parent = transform;
//             newCard.transform.name = $"Card {i}";
//             cards.Add(newCard.transform);
//             //yield return new WaitForSeconds(0.2f);
//         }
//         ArrangeCards();
//         yield return null;
//     }
//
//     private float LinearInterpolation(float start, float end, int currentCount, int maxCount)
//     {
//         if (currentCount > maxCount) return end;
//
//         return start + (end - start) * (currentCount - 1) / (maxCount - 1);
//     }
//
//     void ArrangeCards()
//     {
//         int objectCount = cards.Count;
//         float angle = LinearInterpolation(startAngle, finalAngle, objectCount, finalNumberOfCards);
//         float increment = (angle * 2) / Mathf.Max(1, (objectCount - 1));
//
//         for (int i = 0; i < objectCount; i++)
//         {
//             float angleInRadians = (-angle + i * increment) * Mathf.Deg2Rad;
//
//             float x = centerPointTransform.position.x + Mathf.Cos(angleInRadians) * radius;
//             float z = centerPointTransform.position.z + Mathf.Sin(angleInRadians) * radius;
//
//             cards[i].transform.position = new Vector3(x, centerPointTransform.position.y, z);
//
//             cards[i].transform.LookAt(new Vector3(centerPointTransform.position.x, cards[i].transform.position.y, centerPointTransform.position.z));
//             cards[i].Rotate(0, 0, -5);
//             Debug.Log($"Cards number: {i} Angle: {(-angle + i * increment)}\n Cards rotation: {cards[i].rotation.eulerAngles}");
//         }
//     }
//
//     float CalculateAngleForPosition(Vector3 objectPosition, Transform centerPointTransform, float radius)
//     {
//         // Obliczanie r�nicy po�o�enia mi�dzy centrum a pozycj� obiektu
//         float dx = objectPosition.x - centerPointTransform.position.x;
//         float dz = objectPosition.z - centerPointTransform.position.z;
//
//         // Obliczanie k�ta w radianach przy u�yciu arctangensa
//         float angleInRadians = Mathf.Atan2(dz, dx);
//
//         // Konwersja radian�w na stopnie
//         float angleInDegrees = angleInRadians * Mathf.Rad2Deg;
//
//         // Ajustowanie wyniku tak, aby by� z zakresu 0-360 stopni
//         /*if (angleInDegrees < 0)
//         {
//             angleInDegrees += 360;
//         }*/
//         Debug.Log($"Object at ({objectPosition.x}, {objectPosition.z}) has an angle of {angleInDegrees} degrees relative to the center at ({centerPointTransform.position.x}, {centerPointTransform.position.z})");
//         return angleInDegrees;
//     }
//
//     void ArrangeCardsMozeSiePrzydac(float startAngle, int objectCount)
//     {
//         float endAngle = startAngle; // Startowy k�t jest r�wnie� k�tem ko�cowym dla 1 karty
//         float radius = 5f; // Mo�esz dostosowa� promie� wg potrzeb
//
//         // Je�li jest wi�cej ni� jedna karta, oblicz maksymalny k�t wychylenia
//         if (objectCount > 1)
//         {
//             endAngle = startAngle + (objectCount - 1) * 10; // Zwi�kszaj k�t o 10 stopni na ka�d� dodatkow� kart�
//         }
//
//         // Zapewnienie, �e startAngle jest mniejszy ni� endAngle
//         if (startAngle > endAngle)
//         {
//             float temp = startAngle;
//             startAngle = endAngle;
//             endAngle = temp;
//         }
//
//         float angleRange = endAngle - startAngle;
//         float increment = objectCount > 1 ? angleRange / (objectCount - 1) : 0; // Aby r�wnomiernie rozmie�ci�, unikaj�c dzielenia przez 0
//
//         for (int i = 0; i < objectCount; i++)
//         {
//             float angleInRadians = (-startAngle + i * increment) * Mathf.Deg2Rad;
//
//             // Obliczanie pozycji x i y na podstawie k�ta i promienia
//             float x = centerPointTransform.position.x + Mathf.Cos(angleInRadians) * radius;
//             float z = centerPointTransform.position.z + Mathf.Sin(angleInRadians) * radius;
//
//             // Ustawianie pozycji karty
//             cards[i].transform.position = new Vector3(x, centerPointTransform.position.y, z);
//
//             // Obracanie karty w stron� centrum
//             cards[i].transform.LookAt(new Vector3(centerPointTransform.position.x, cards[i].transform.position.y, centerPointTransform.position.z));
//         }
//     }
// }

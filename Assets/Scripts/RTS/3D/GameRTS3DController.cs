using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRTS3DController : MonoBehaviour
{
    [SerializeField]
    RectTransform selectionAreaTransform;
    [SerializeField]
    Canvas canvas;
    [SerializeField]
    private LayerMask layerMask;
    private Vector3 startPosition;
    private Vector3 areaStartPosition;
    private Vector3 endPosition;
    private List<UnitRTS3D> selectedUnitArray;
    private Collider[] colliders;
    // Start is called before the first frame update
    void Start()
    {
        selectedUnitArray = new List<UnitRTS3D>();
        selectionAreaTransform.gameObject.SetActive(false);
    }

 
    void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectionAreaTransform.gameObject.SetActive(true);
            for (int i = 0; i < selectedUnitArray.Count; i++)
            {
                selectedUnitArray[i].gameObject.GetComponent<UnitRTS3D>().SetSelectedVisible(false);
            }
            selectedUnitArray.Clear();
            startPosition = MyUtils.GetScreenToMapPosition();
            endPosition = startPosition;
            areaStartPosition = Input.mousePosition / canvas.scaleFactor;
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 currentMousePosition = Input.mousePosition/canvas.scaleFactor;
            Debug.Log("currentMousePosition======" + currentMousePosition);
            Vector3 lowerLeft = new Vector3(
                Mathf.Min(areaStartPosition.x, currentMousePosition.x),
                Mathf.Min(areaStartPosition.y, currentMousePosition.y)
                );
            Vector3 upperRight = new Vector3(
                Mathf.Max(areaStartPosition.x, currentMousePosition.x),
                Mathf.Max(areaStartPosition.y, currentMousePosition.y)
                );
            selectionAreaTransform.position = lowerLeft;
            selectionAreaTransform.localScale = upperRight - lowerLeft;
            endPosition = MyUtils.GetScreenToMapPosition();
        }
        if (Input.GetMouseButtonUp(0))
        {
            selectionAreaTransform.gameObject.SetActive(false);
            Vector3 centerPosition = startPosition + (endPosition - startPosition) / 2;
            centerPosition.y = 1;
            colliders = Physics.OverlapBox(centerPosition, new Vector3(Mathf.Abs(endPosition.x - startPosition.x), 1, Mathf.Abs(endPosition.z - startPosition.z))/2,Quaternion.identity,1<<9);
            
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].gameObject.GetComponent<UnitRTS3D>().SetSelectedVisible(true);
                selectedUnitArray.Add(colliders[i].gameObject.GetComponent<UnitRTS3D>());
            }
            
        }
        if (Input.GetMouseButtonDown(1))
        {
            foreach (UnitRTS3D unitRTS in selectedUnitArray)
            {
                unitRTS.SetTargetPosition(MyUtils.GetScreenToMapPosition());
            }
        }
       
    }
    private void OnDrawGizmos()
    {
        Vector3 centerPosition = startPosition + (endPosition - startPosition) / 2;
        centerPosition.y = 1;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(centerPosition, new Vector3(endPosition.x - startPosition.x, 1,endPosition.z - startPosition.z));
    }
}


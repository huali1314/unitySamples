using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRTSController : MonoBehaviour
{
    [SerializeField]
    private Transform selectionAreaTransform;
    private Vector3 startPosition;
    List<UnitRTS> selectedUnitRTSList;
    // Start is called before the first frame update
    void Start()
    {
        selectedUnitRTSList = new List<UnitRTS>();
        selectionAreaTransform.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectionAreaTransform.gameObject.SetActive(true);
            startPosition = MyUtils.GetMouseWorldPosition();
            Debug.Log(startPosition);
        }
        if (Input.GetMouseButton(0))
        {
            Vector3 currentMousePosition = MyUtils.GetMouseWorldPosition();
            Vector3 lowerLeft = new Vector3(
                Mathf.Min(startPosition.x,currentMousePosition.x),
                Mathf.Min(startPosition.y,currentMousePosition.y)
                );
            Vector3 upperRight = new Vector3(
                Mathf.Max(startPosition.x, currentMousePosition.x),
                Mathf.Max(startPosition.y, currentMousePosition.y)
                );
            selectionAreaTransform.position = lowerLeft;
            selectionAreaTransform.localScale = upperRight - lowerLeft;
        }
        if (Input.GetMouseButtonUp(0))
        {
            
            Collider2D[] collider2DArray = Physics2D.OverlapAreaAll(startPosition, MyUtils.GetMouseWorldPosition());
            foreach (UnitRTS unit in selectedUnitRTSList)
            {
                unit.SetSelectedVisible(false);
            }
            selectedUnitRTSList.Clear();
            if (collider2DArray.Length != 0)
            {
                if (Vector2.Distance(startPosition, MyUtils.GetMouseWorldPosition()) < 0.2f)
                {
                    UnitRTS unitRTS = collider2DArray[0].GetComponent<UnitRTS>();
                    if (unitRTS != null)
                    {
                        unitRTS.SetSelectedVisible(true);
                        selectedUnitRTSList.Add(unitRTS);
                    }
                }
                else
                {
                    foreach (Collider2D collider2D in collider2DArray)
                    {
                        UnitRTS unitRTS = collider2D.GetComponent<UnitRTS>();
                        if (unitRTS != null)
                        {
                            unitRTS.SetSelectedVisible(true);
                            selectedUnitRTSList.Add(unitRTS);
                        }

                    }
                }
            }
            
            selectionAreaTransform.gameObject.SetActive(false);
        }
        if (Input.GetMouseButtonDown(1))
        {
            int targetPositionIndex = 0;
            Vector3 moveToPosition = MyUtils.GetMouseWorldPosition();
            List<Vector3> positionList = GetPositionListAround(moveToPosition, new float[] { 1f, 2f, 3f }, new int[] { 5, 10, 15 });
            Debug.Log(selectedUnitRTSList.Count);
            //List<Vector3> positionList = GetPositionListQuad(moveToPosition, 1f, selectedUnitRTSList.Count);
            foreach (UnitRTS unit in selectedUnitRTSList)
            {

                unit.SetTargetPosition(positionList[targetPositionIndex]);
                targetPositionIndex = (targetPositionIndex + 1)%positionList.Count;
            }
            
        }
    }
    //得到更多层的目标点，一环套一环
    private List<Vector3> GetPositionListAround(Vector3 startPosition, float[] ringDistanceArray, int[] ringPositionCountArray)
    {
        List<Vector3> targetPositionArray = new List<Vector3>();
        targetPositionArray.Add(startPosition);
        for (int i = 0; i < ringDistanceArray.Length; i++)
        {
            targetPositionArray.AddRange(GetPositionListAround(startPosition, ringDistanceArray[i], ringPositionCountArray[i]));
        }
        return targetPositionArray;
    }
    //根据目标点的位置，算出周围目标点的位置，点与点之间的距离由distance决定，点的数量由positonCount决定
    private List<Vector3> GetPositionListAround(Vector3 startPosition,float distance,int positonCount)
    {
        List<Vector3> positionList = new List<Vector3>();
        for (int i = 0; i < positonCount; i++)
        {
            float angle = i * 360f / positonCount;
            Vector3 dir = ApplyRotationToVector(new Vector3(1, 0), angle);
            Vector3 position = startPosition + dir * distance;
            positionList.Add(position);
        }
        return positionList;
    }
    //方阵显示
    private List<Vector3> GetPositionListQuad(Vector3 startPosition,float distance, float maxPositionCount)
    {
        float count = Mathf.Ceil(Mathf.Sqrt(maxPositionCount));
        List<Vector3> positionList = new List<Vector3>();
        for (int i = 0; i <= count - 1; i++)
        {
            for (int j = 0; j < count; j++)
            {
                positionList.Add(startPosition + new Vector3(j,i,0));
            }
            
        }
        return positionList;
        //
        //positionList.Add(startPosition);
        //positionList.Add(startPosition + new Vector3(1, 0, 0));
        //positionList.Add(startPosition + new Vector3(1, 1, 0));
        //positionList.Add(startPosition + new Vector3(1, -1, 0));
        //positionList.Add(startPosition + new Vector3(0, 1, 0));
        //positionList.Add(startPosition + new Vector3(1, -1, 0));
        //positionList.Add(startPosition + new Vector3(-1, 1, 0));
        //positionList.Add(startPosition + new Vector3(-1, 0, 0));
        //positionList.Add(startPosition + new Vector3(-1, -1, 0));
    }
    private Vector3 ApplyRotationToVector(Vector3 vec, float angle)
    {
        return Quaternion.Euler(0, 0, angle) * vec;
    }
}

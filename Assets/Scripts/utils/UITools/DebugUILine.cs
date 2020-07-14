#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

[ExecuteInEditMode]
public class DebugUILine : MonoBehaviour
{
    private Type colliderType;
    static Vector3[] fourCorners = new Vector3[4];
    void OnDrawGizmos()
    {
        Debug.Log("===========OnDrawGizmos=============");
        foreach (MaskableGraphic g in GameObject.FindObjectsOfType<MaskableGraphic>())
        {
            if (g.raycastTarget)
            {
                RectTransform rectTransform = g.transform as RectTransform;
                rectTransform.GetWorldCorners(fourCorners);
                Gizmos.color = Color.red;
                for (int i = 0; i < 4; i++)
                    Gizmos.DrawLine(fourCorners[i], fourCorners[(i + 1) % 4]);

            }
        }
        foreach (Collider c in GameObject.FindObjectsOfType<Collider>())
        {
            colliderType = c.GetType();
            if (colliderType == typeof(BoxCollider))
            {
                Debug.Log("type==========" + c.GetType());
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(c.transform.position, c.transform.localScale * 1.1f);
            }
            else if (colliderType == typeof(SphereCollider))
            {
                Debug.Log("type==========" + c.GetType());
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(c.transform.position,(c as SphereCollider).radius * 1.1f);
            }
            else if (colliderType == typeof(CapsuleCollider))
            {
                Debug.Log("type==========" + c.GetType());
                Gizmos.color = Color.yellow;
                float temp = (c as CapsuleCollider).radius * 2;
                Gizmos.DrawWireCube(c.transform.position, new Vector3(temp, (c as CapsuleCollider).height, temp));
            }
        }
    }
}
#endif
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class UIScreenAdapt : MonoBehaviour
// {
// 	float devHeight = 960.0f;
// 	float devWidth = 640.0f;
// 	private Transform [] trans;
//     // Start is called before the first frame update
//     void Start()
//     {
//     	float screenHeight = Screen.height;
//     	float screenWidth = Screen.width;
//     	float radio1 = screenHeight/screenWidth;
//     	float radio2 = devHeight/devWidth;
//     	float diff_radio = radio2 - radio1;
//     	UnityEngine.Debug.Log(radio1 + "radio1=======");
//     	UnityEngine.Debug.Log(radio2 + "radio2=======");
//     	UnityEngine.Debug.Log(diff_radio + "diff_radio=======");
// 		// UnityEngine.Debug.Log(diff + "diff========");
//         // for (int i = 0;i < transform.childCount;i++)
// 		// {
// 		// 	trans[i] = transform.GetChild(i).transform;
// 		// }
// 	// }
// }
using UnityEngine;
using System.Collections;
  
public class UIScreenAdapt : MonoBehaviour {
  
      float devHeight = 9.6f;
      float devWidth = 6.4f;
  
     // Use this for initialization
    void Start () {
     
         float screenHeight = Screen.height;
 
        Debug.Log ("screenHeight = " + screenHeight);

         //this.GetComponent<Camera>().orthographicSize = screenHeight / 200.0f;
 
         float orthographicSize = this.GetComponent<Camera>().orthographicSize;
 
         float aspectRatio = Screen.width * 1.0f / Screen.height;
 
         float cameraWidth = orthographicSize * 2 * aspectRatio;

         Debug.Log ("cameraWidth = " + cameraWidth);
 
         if (cameraWidth < devWidth)
         {
             orthographicSize = devWidth / (2 * aspectRatio);
             Debug.Log ("new orthographicSize = " + orthographicSize);
             this.GetComponent<Camera>().orthographicSize = orthographicSize;
         }
 
     }
     
     // Update is called once per frame
    void Update () {
     
     }
 }
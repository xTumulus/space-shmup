using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
  public GameObject playerShip;
  public GameObject[] panels;
  public float scrollSpeed = -30f;
  public float motionMultiplier = 0.25f;
  private float panelHeight; 
  private float panelDepth; 

  void Start () {
    panelHeight = panels[0].transform.localScale.y;
    panelDepth = panels[0].transform.position.z;
    
    // Set initial positions of panels
    panels[0].transform.position = new Vector3(0,0,panelDepth);
    panels[1].transform.position = new Vector3(0,panelHeight,panelDepth);
  }

  // Update is called once per frame
  void Update () {
    panelHeight = panels[0].transform.localScale.y;
    panelDepth = panels[0].transform.position.z;
    
    // Set initial positions of panels
    panels[0].transform.position = new Vector3(0,0,panelDepth);
    panels[1].transform.position = new Vector3(0,panelHeight,panelDepth);
    
    float tY, tX=0;
    tY= Time.time * scrollSpeed % panelHeight + (panelHeight*0.5f);
    if (playerShip != null) {
      tX = -playerShip.transform.position.x * motionMultiplier;
    }

    panels[0].transform.position = new Vector3(tX, tY, panelDepth);
    if (tY >= 0) {
      panels[1].transform.position = new Vector3(tX, tY-panelHeight, panelDepth);
    } 
    else {
      panels[1].transform.position = new Vector3(tX, tY+panelHeight, panelDepth);
    }
  }
}

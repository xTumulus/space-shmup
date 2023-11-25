using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy {
  public Vector3[] points;
  public float birthTime;
  public float lifeTime = 10;

  // Determines how much the Sine wave will affect movement
  public float sinEccentricity = 0.6f;

  void Start () {
    powerUpDropChance = 0.75f;

    // Initialize the points
    points = new Vector3[2];
    
    // Find Utils.camBounds
    Vector3 cbMin = Utils.cameraBounds.min;
    Vector3 cbMax = Utils.cameraBounds.max;
    Vector3 v = Vector3.zero;
    
    // Pick any point on the left side of the screen
    v.x = cbMin.x - Main.mainSingleton.enemySpawnPadding;
    v.y = Random.Range( cbMin.y, cbMax.y );
    points[0] = v;
    
    // Pick any point on the right side of the screen
    v = Vector3.zero;
    v.x = cbMax.x + Main.mainSingleton.enemySpawnPadding;
    v.y = Random.Range( cbMin.y, cbMax.y );
    points[1] = v;
    
    // Possibly swap sides
    if (Random.value < 0.5f) {
    // Setting the .x of each point to its negative will move it to the
    // other side of the screen
    points[0].x *= -1;
    points[1].x *= -1;
    }
    
    // Set the birthTime to the current time
    birthTime = Time.time;
  }
  public override void Move() {
    // Bézier curves work based on a u value between 0 & 1
    float lifeTime = (Time.time - birthTime) / this.lifeTime;
    
    // If u>1, then it has been longer than lifeTime since birthTime
    if (lifeTime > 1) {
      // This Enemy_2 has finished its life
      Destroy( this.gameObject );
      return;
    }
    
    // Adjust position using lifeTime and sin easing curve
    lifeTime = lifeTime + sinEccentricity*(Mathf.Sin(lifeTime*Mathf.PI*2));
    position = (1-lifeTime)*points[0] + lifeTime*points[1];
  }
}

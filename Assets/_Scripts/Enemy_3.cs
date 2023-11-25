using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_3 : Enemy
{
  // Enemy_3 will move following a Bezier curve, which is a linear
  // interpolation between more than two points.
  public Vector3[] points;
  public float birthTime;
  public float lifeTime = 10;

  public void Start() {
    powerUpDropChance = 0.5f;

    points = new Vector3[3]; // Initialize points
    // The start position has already been set by Main.SpawnEnemy()
    points[0] = position;
    
    // Set xMin and xMax the same way that Main.SpawnEnemy() does
    float xMin = Utils.cameraBounds.min.x + Main.mainSingleton.enemySpawnPadding;
    float xMax = Utils.cameraBounds.max.x - Main.mainSingleton.enemySpawnPadding;
    Vector3 v;
    
    // Pick a random middle position in the bottom half of the screen
    v = Vector3.zero;
    v.x = Random.Range( xMin, xMax );
    v.y = Random.Range( Utils.cameraBounds.min.y, 0 );
    points[1] = v;

    // Pick a random final position above the top of the screen
    v = Vector3.zero;
    v.y = position.y;
    v.x = Random.Range( xMin, xMax );
    points[2] = v;
    // Set the birthTime to the current time
    birthTime = Time.time;
  }

  public override void Move() {
    float lifeTime = (Time.time - birthTime) / this.lifeTime;
    
    if (lifeTime > 1) {
      // This Enemy_3 has finished its life
      Destroy( this.gameObject );
      return;
    }
    // Interpolate the three Bezier curve points
    Vector3 p01, p12;
    p01 = (1-lifeTime)*points[0] + lifeTime*points[1];
    p12 = (1-lifeTime)*points[1] + lifeTime*points[2];
    position = (1-lifeTime)*p01 + lifeTime*p12;
  }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy
{
  public float waveFrequency = 2;
  // in meters
  public float waveWidth = 4;
  public float waveRotY = 45;
  private float x0 = -12345; // The initial x value of pos
  private float birthTime;

  void Start() {
    x0 = position.x;
    birthTime = Time.time;
  }

  public override void Move() {
  Vector3 tempPosition = position;
  // theta adjusts based on time
  float age = Time.time - birthTime;
  float theta = Mathf.PI * 2 * age / waveFrequency;
  float sin = Mathf.Sin(theta);
  tempPosition.x = x0 + waveWidth * sin;
  position = tempPosition;

  // rotate a bit about y
  Vector3 rot = new Vector3(0, sin*waveRotY, 0);
  this.transform.rotation = Quaternion.Euler(rot);

  // base.Move() still handles the movement down in y
  base.Move();
  }

}

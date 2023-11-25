using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
  public float rotationsPerSecond = 0.1f;
  public bool ________________;
  public int levelShown = 0;

  void Update() {
      int currentLevel = Mathf.FloorToInt(Hero.heroSingleton.shieldLevel);

      if (levelShown != currentLevel) {
        levelShown = currentLevel;
        Material material = this.GetComponent<Renderer>().material;
        material.mainTextureOffset = new Vector2(0.2f * levelShown, 0);
      }

      float shieldRotation = (rotationsPerSecond * Time.time * 360) % 360f;
      transform.rotation = Quaternion.Euler(0, 0, shieldRotation);
  }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Part {
  // These three fields need to be defined in the Inspector pane
  public string name;
  public float health;

  // These two fields are set automatically in Start().
  // Caching like this makes it faster and easier to find these later
  public GameObject gameObject;
  public Material material;
}

public class Enemy_4 : Enemy
{
  // Enemy_4 will start offscreen and then pick a random point on screen to
  // move to. Once it has arrived, it will pick another random point and
  // continue until the player has shot it down.
  public Vector3[] points; // Stores the p0 & p1 for interpolation
  public float timeStart; // Birth time for this Enemy_4
  public float duration = 4; // Duration of movement
  public Part[] parts;

  void Start() {
    powerUpDropChance = 1f;

    points = new Vector3[2];// There is already an initial position chosen by Main.SpawnEnemy()
    points[0] = position;
    points[1] = position;

    InitMovement();

    //Cache gameObject and transform for each part
    Transform partTransform;
    foreach (Part part in parts) {
      partTransform = transform.Find(part.name);
      print("caching part: " + part.name);

      if (partTransform != null) {
        part.gameObject = partTransform.gameObject;
        print("game object: " + part.gameObject);
        part.material = part.gameObject.GetComponent<Renderer>().material;
        print("material: " + part.material);
      }
    }
  }

  void InitMovement() {
    // Pick a new point to move to that is on screen
    Vector3 p1 = Vector3.zero;
    float esp = Main.mainSingleton.enemySpawnPadding;
    Bounds cBounds = Utils.cameraBounds;
    p1.x = Random.Range(cBounds.min.x + esp, cBounds.max.x - esp);
    p1.y = Random.Range(cBounds.min.y + esp, cBounds.max.y - esp);
    points[0] = points[1]; // Shift points[1] to points[0]
    points[1] = p1;

    timeStart = Time.time;
  }

  public override void Move() {
    // This completely overrides Enemy.Move() with a linear interpolation
    float u = (Time.time-timeStart)/duration;
    if (u>=1) { // if u >=1...
      InitMovement(); // ...then initialize movement to a new point
      u=0;
    }
    
    u = 1 - Mathf.Pow( 1-u, 2 );
    // Apply Ease Out easing to u
    position = (1-u)*points[0] + u*points[1]; // Simple linear interpolation
  }

  // This will override the OnCollisionEnter that is part of Enemy.cs
  // Because of the way that MonoBehaviour declares common Unity functions
  // like OnCollisionEnter(), the override keyword is not necessary.
  void OnCollisionEnter(Collision collision) {
    GameObject other = collision.gameObject;
    
    switch (other.tag) {
      case "ProjectileHero":
        Projectile projectile = other.GetComponent<Projectile>();

        bounds.center = transform.position + boundsCenterOffset;
        if (bounds.extents == Vector3.zero || Utils.ScreenBoundsCheck(bounds, BoundsTest.offScreen) != Vector3.zero) {
          Destroy(other);
          break;
        }
        
        bool damageDealt = false;
        Part partHit;
        int partIndex = parts.Length - 1;
        while(!damageDealt) {
          // Check if part is destroyed
          if (!Destroyed(parts[partIndex])) {
            // Part not destroyed, deal damage to part
            partHit = parts[partIndex];
            partHit.health--;
            damageDealt = true;
            print("Damaged part " + partHit.name + " material: " + partHit.material + " health: " + partHit.health);
            
            //Destroy projectile
            Destroy(other);
            
            //Blink red for damage
            ShowLocalizedDamage(partHit.material);

            // Destroy part if health depleted
            if(partHit.health <= 0) {
              partHit.gameObject.SetActive(false);
              print("deactivated part " + partIndex + " : " + partHit);

              // Destroy enemy if all parts destroyed
              if (partIndex == 0) {
                Main.mainSingleton.ShipDestroyed(this);
                Destroy(this.gameObject);
              }
            }
          }
          else {
            // Part at this index was already destroyed
            partIndex --;
          }
        }
        
        break;
    }
  }

  bool Destroyed(Part part) {
    if (part == null) { // If no real Part was passed in
      return(true); // Return true (meaning yes, it was destroyed)
    }
  
    // Returns the result of the comparison: prt.health <= 0
    // If prt.health is 0 or less, returns true (yes, it was destroyed)
    return (part.health <= 0);
  }
  
  // This changes the color of just one Part to red instead of the whole ship
  void ShowLocalizedDamage(Material m) {
    m.color = Color.red;
    remainingDamageFrames = showDamageFrames;
  }
}

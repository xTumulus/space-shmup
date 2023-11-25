using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
  public float speed = 10f;
  public float fireRate = 0.3f;
  public float health = 10;
  public int score = 100;

  public int showDamageFrames = 10;
  public float powerUpDropChance = 0.25f;
  public bool ________________;

  public Color[] originalColors;
  public Material[] materials;
  public int remainingDamageFrames = 0;
  public Bounds bounds;
  public Vector3 boundsCenterOffset;

  void Awake() {
    materials = Utils.GetAllMaterials(gameObject);
    originalColors = new Color[materials.Length];
    for (int i = 0; i < materials.Length; i++) {
      originalColors[i] = materials[i].color;
    }

    InvokeRepeating("CheckOffscreen", 0f, 2f);
  }

  void Update() {
    Move();

    if (remainingDamageFrames > 0) {
      remainingDamageFrames--;
      if (remainingDamageFrames == 0) {
        UnshowDamage();
      }
    }
  }

  public virtual void Move() {
    Vector3 tempPosition = position;
    tempPosition.y -= speed * Time.deltaTime;
    position = tempPosition;
  }

  public Vector3 position {
    get {
      return( this.transform.position );
    }
    set {
      this.transform.position = value;
    }
  }

  void CheckOffscreen() {
    // get bounds
    if (bounds.size == Vector3.zero) {
      bounds = Utils.CombineChildrenBounds(this.gameObject);
      boundsCenterOffset = bounds.center - transform.position;
    }

    // update the bounds to the current position
    bounds.center = transform.position + boundsCenterOffset;
    
    // Destroy enemy when off screen
    Vector3 off = Utils.ScreenBoundsCheck( bounds, BoundsTest.offScreen );
    if ( off != Vector3.zero ) {
      if (off.y < 0) {
        Destroy( this.gameObject );
      }
    }
  }

  void OnCollisionEnter( Collision collision ) {
    GameObject other = collision.gameObject;
    
    switch (other.tag) {
      case "ProjectileHero":
        Projectile projectile = other.GetComponent<Projectile>();
        
        // Enemies don't take damage unless they're onscreen
        bounds.center = transform.position + boundsCenterOffset;
        if (bounds.extents == Vector3.zero || Utils.ScreenBoundsCheck(bounds, BoundsTest.offScreen) != Vector3.zero) {
          Destroy(other);
          break;
        }
        
        ShowDamage();
        // Get the damage amount from the Projectile.type & Main.WEAPON_DEFINITIONS
        health -= Main.WEAPON_DEFINITIONS[projectile.type].damageOnHit;
        
        // Destroy enemy if needed
        if (health <= 0) {
          Destroy(this.gameObject);
          Main.mainSingleton.ShipDestroyed(this);
        }
        
        //Destroy player projectile
        Destroy(other);
        
        break;
    }
  }

  void ShowDamage() {
    // print("showing damage");
    foreach (Material material in materials) {
      material.color = Color.red;
    }
    remainingDamageFrames = showDamageFrames;
  }

  void UnshowDamage() {
    // print("unshowing damage");
    for (int i = 0; i < materials.Length; i++) {
      materials[i].color = originalColors[i];
    }
  }

}

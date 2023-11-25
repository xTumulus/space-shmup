using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This is an enum of the various possible weapon types
// It also includes a "shield" type to allow a shield power-up
// Items marked [NI] below are Not Implemented in this book
public enum WeaponType {
none, // The default / no weapon
blaster, // A simple blaster
spread, // Two shots simultaneously
phaser, // Shots that move in waves [NI]
missile, // Homing missiles [NI]
laser, // Damage over time [NI]
shield // Raise shieldLevel
}

// The WeaponDefinition class allows you to set the properties
// of a specific weapon in the Inspector. Main has an array
// of WeaponDefinitions.
[System.Serializable]
public class WeaponDefinition {
public WeaponType type = WeaponType.none;
public string letter;
public Color color = Color.white;
public GameObject projectilePrefab;
public Color projectileColor = Color.white;
public float damageOnHit = 0;
public float continuousDamage = 0;
public float delayBetweenShots = 0;
public float velocity = 20;
}
// Note: Weapon prefabs, colors, and so on. are set in the class Main.

public class Weapon : MonoBehaviour
{
  static public Transform PROJECTILE_ANCHOR;
  public bool ____________________;

  [Header("Serialized")]
  private WeaponType _type = WeaponType.none;
  public WeaponDefinition weaponDefinition;
  public GameObject collar;
  public float lastShotTime;
  
  void Start() {
    collar = transform.Find("Collar").gameObject;

    SetType( _type );
    
    if (PROJECTILE_ANCHOR == null) {
      GameObject gameObject = new GameObject("_Projectile_Anchor");
      PROJECTILE_ANCHOR = gameObject.transform;
    }

    // Find the fireDelegate of the parent
    GameObject parentGameObject = transform.parent.gameObject;
    if (parentGameObject.tag == "Hero") {
      Hero.heroSingleton.fireDelegate += Fire;
    }
  }

  public WeaponType type {
    get { return( _type ); }
    set { SetType( value ); }
  }

  public void SetType( WeaponType wt ) {
    _type = wt;
    if (type == WeaponType.none) {
      this.gameObject.SetActive(false);
      return;
    } 
    else {
      this.gameObject.SetActive(true);
    }

    weaponDefinition = Main.GetWeaponDefinition(_type);
    collar.GetComponent<Renderer>().material.color = weaponDefinition.color;
    lastShotTime = 0; // You can always fire immediately after _type is set.
  }
  public void Fire() {
    // gameObject is inactive, return
    if (!gameObject.activeInHierarchy) {
      return;
    } 
    
    // Not enough time between shots, return
    if (Time.time - lastShotTime < weaponDefinition.delayBetweenShots) {
      return;
    }

    Projectile projectile;
    
    switch (type) {
      case WeaponType.blaster:
        projectile = MakeProjectile();
        projectile.GetComponent<Rigidbody>().velocity = Vector3.up * weaponDefinition.velocity;
        break;
      case WeaponType.spread:
        projectile = MakeProjectile();
        projectile.GetComponent<Rigidbody>().velocity = Vector3.up * weaponDefinition.velocity;
        projectile = MakeProjectile();
        projectile.GetComponent<Rigidbody>().velocity = new Vector3( -.2f, 0.9f, 0 ) * weaponDefinition.velocity;
        projectile = MakeProjectile();
        projectile.GetComponent<Rigidbody>().velocity = new Vector3( .2f, 0.9f, 0 ) * weaponDefinition.velocity;
      break;
    }
  }

  public Projectile MakeProjectile() {
    GameObject gameObject = Instantiate( weaponDefinition.projectilePrefab ) as GameObject;
    
    if ( transform.parent.gameObject.tag == "Hero" ) {
      gameObject.tag = "ProjectileHero";
      gameObject.layer = LayerMask.NameToLayer("ProjectileHero");
    } 
    else {
      gameObject.tag = "ProjectileEnemy";
      gameObject.layer = LayerMask.NameToLayer("ProjectileEnemy");
    }
    
    gameObject.transform.position = collar.transform.position;
    gameObject.transform.parent = PROJECTILE_ANCHOR;
    
    Projectile projectile = gameObject.GetComponent<Projectile>();
    projectile.type = type;
    lastShotTime = Time.time;
    
    return( projectile );
  }
}

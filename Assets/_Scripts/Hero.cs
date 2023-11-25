using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
  static public Hero heroSingleton;

  // Gameplay
  public float gameRestartDelay = 2f;

  // Movement
  public float speed = 30;
  public float rollMultiplier = -45;
  public float pitchMultiplier = 30;

  // Status
  [Header("Serialized")]
  private float _shieldLevel = 1;
  public bool ____________________________;

  // Bounds
  public Bounds bounds;

  // Collision
  public GameObject lastCollisionObject = null;

  // Weapon
  public Weapon[] weapons;
  public delegate void WeaponFireDelegate();
  public WeaponFireDelegate fireDelegate;

  void Awake() {
    heroSingleton = this;
    bounds = Utils.CombineChildrenBounds(this.gameObject);
  }

  void Start() {
    ClearWeapons();
    weapons[0].SetType(WeaponType.blaster);
  }

  // Update is called once per frame
  void Update() {
    float xAxis = Input.GetAxis("Horizontal");
    float yAxis = Input.GetAxis("Vertical");

    Vector3 position = transform.position;
    position.x += xAxis * speed * Time.deltaTime;
    position.y += yAxis * speed * Time.deltaTime;
    transform.position = position;

    bounds.center = transform.position;

    Vector3 offset = Utils.ScreenBoundsCheck(bounds, BoundsTest.onScreen);
    if (offset != Vector3.zero) {
      position -= offset;
      transform.position = position;
    }

    transform.rotation = Quaternion.Euler(yAxis * pitchMultiplier, xAxis * rollMultiplier, 0);
    
    if (Input.GetAxis("Jump") == 1 && fireDelegate != null) {
      fireDelegate();
    }
  }

  void OnTriggerEnter(Collider other) {
    GameObject gameObject = Utils.FindTaggedParent(other.gameObject);

    if (gameObject != null) {
      if (gameObject == lastCollisionObject) {
        return;
      }

      lastCollisionObject = gameObject;
      if (gameObject.tag == "Enemy" || gameObject.tag == "EnemyProjectile") {
        shieldLevel--;
        Destroy(gameObject);
      } 
      else if (gameObject.tag == "PowerUp"){
        AbsorbPowerUp(gameObject);
      }
      else {
        print("Collided: " + gameObject.name);
      }
    }
  }

  public void AbsorbPowerUp( GameObject gameObject ) {
    PowerUp powerUp = gameObject.GetComponent<PowerUp>();
    
    switch (powerUp.type) {
      case WeaponType.shield:
        shieldLevel++;
        break;
      default: // any Weapon PowerUp
        if (powerUp.type == weapons[0].type) {
          Weapon weapon = GetEmptyWeaponSlot();
          if (weapon != null) {
            weapon.SetType(powerUp.type);
          }
        } 
        else {
          // new weapon type
          ClearWeapons();
          weapons[0].SetType(powerUp.type);
        }
        break;
    }
    
    powerUp.AbsorbedBy( this.gameObject );
  }

  Weapon GetEmptyWeaponSlot() {
    for (int i=0; i<weapons.Length; i++) {
      if ( weapons[i].type == WeaponType.none ) {
      return( weapons[i] );
      }
    }

    return( null );
  }

  void ClearWeapons() {
    foreach (Weapon weapon in weapons) {
      weapon.SetType(WeaponType.none);
    }
  }

  public float shieldLevel {
    get {
      return( _shieldLevel );
    }
    set {
      _shieldLevel = Mathf.Min( value, 4 );
      if (value < 0) {
        Destroy(this.gameObject);
        Main.mainSingleton.DelayedRestart(gameRestartDelay);
      }
    }
  }
}

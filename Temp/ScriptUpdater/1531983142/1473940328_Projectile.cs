using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
  [Header("Serialized")]
  private WeaponType _type;
  public WeaponType type {
    get {
      return( _type );
    }
    set {
      SetType( value );
    }
  }

  void Awake() {
    InvokeRepeating( "CheckOffscreen", 2f, 2f );
  }

  public void SetType( WeaponType eType ) {
    _type = eType;
    WeaponDefinition def = Main.GetWeaponDefinition( _type );
    GetComponent<Renderer>().material.color = def.projectileColor;
    // GetComponent<Renderer>().material.color = def.projectileColor;
  }

  void CheckOffscreen() {
    if ( Utils.ScreenBoundsCheck( GetComponent<Collider>().bounds, BoundsTest.offScreen ) != Vector3.zero ) {
      Destroy( this.gameObject );
    }
  }
}

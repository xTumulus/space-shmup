using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
  public Vector2 rotMinMax = new Vector2(15,90);
  public Vector2 driftMinMax = new Vector2(.25f,2);
  public float lifeTime = 6f; // Seconds the PowerUp exists
  public float fadeTime = 4f; // Seconds it will then fade
  public bool ________________;
  public WeaponType type; // The type of the PowerUp
  public GameObject cube; // Reference to the Cube child
  public TextMesh letter; // Reference to the TextMesh
  public Vector3 rotPerSecond; // Euler rotation speed
  public float birthTime;

  void Awake() {
    cube = transform.Find("Cube").gameObject;
    letter = GetComponent<TextMesh>();

    Vector3 velocity = Random.onUnitSphere; 
    velocity.z = 0; 
    velocity.Normalize();
    velocity *= Random.Range(driftMinMax.x, driftMinMax.y);    
    GetComponent<Rigidbody>().velocity = velocity;

    transform.rotation = Quaternion.identity;
    rotPerSecond = new Vector3( Random.Range(rotMinMax.x,rotMinMax.y),
    Random.Range(rotMinMax.x,rotMinMax.y),
    Random.Range(rotMinMax.x,rotMinMax.y) );

    InvokeRepeating( "CheckOffscreen", 2f, 2f );
    birthTime = Time.time;
  }

  void Update () {
    // rotate cube
    cube.transform.rotation = Quaternion.Euler( rotPerSecond*Time.time );// Fade out the PowerUp over time
    
    // track lifetime (percent time elapsed)
    float lifetime = (Time.time - (birthTime+lifeTime)) / fadeTime;

    if (lifetime >= 1) {
      Destroy( this.gameObject );
      return;
    }
    
    // Use lifetime to determine the alpha value of the Cube & Letter
    if (lifetime > 0) {
      Color color = cube.GetComponent<Renderer>().material.color;
      color.a = 1f-lifetime;
      cube.GetComponent<Renderer>().material.color = color;
      
      // Fade the Letter too, just not as much
      color = letter.color;
      color.a = 1f - (lifetime*0.5f);
      letter.color = color;
    }
  }

  // This SetType() differs from those on Weapon and Projectile
  public void SetType( WeaponType weaponType ) {
    WeaponDefinition weaponDefinition = Main.GetWeaponDefinition(weaponType);
    
    cube.GetComponent<Renderer>().material.color = weaponDefinition.color;
    letter.text = weaponDefinition.letter;
    type = weaponType;
  }

  public void AbsorbedBy( GameObject target ) {
    Destroy( this.gameObject );
  }

  void CheckOffscreen() {
    // If the PowerUp has drifted entirely off screen...
    if ( Utils.ScreenBoundsCheck( cube.GetComponent<Collider>().bounds, BoundsTest.offScreen) != Vector3.zero ) {
      Destroy( this.gameObject );
    }
  }
}

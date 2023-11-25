using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
  static public Main mainSingleton;
  static public Dictionary<WeaponType, WeaponDefinition> WEAPON_DEFINITIONS;

  //Enemy
  [Header("Inscribed")]

  public GameObject[] prefabEnemies;
  public float enemySpawnPerSecond = 0.5f;
  public float enemySpawnPadding = 1.5f;
  public bool ________________;
  public float enemySpawnRate;

  // Weapon
  public WeaponDefinition[] weaponDefinitions;
  public WeaponType[] activeWeaponTypes;

  //PowerUps
  public GameObject prefabPowerUp;
  public WeaponType[] powerUpFrequency = new WeaponType[] {
    WeaponType.blaster,
    WeaponType.blaster,
    WeaponType.spread,
    WeaponType.shield
  };

  void Awake() {
    mainSingleton = this;
    Utils.SetCameraBounds(this.GetComponent<Camera>());
    enemySpawnRate = 1f/enemySpawnPerSecond;
    Invoke( "SpawnEnemy", enemySpawnRate );

    WEAPON_DEFINITIONS = new Dictionary<WeaponType, WeaponDefinition>();
    foreach(WeaponDefinition definition in weaponDefinitions) {
      WEAPON_DEFINITIONS[definition.type] = definition;
    }
  }

  static public WeaponDefinition GetWeaponDefinition(WeaponType weaponType) {
    if (WEAPON_DEFINITIONS.ContainsKey(weaponType)) {
      return (WEAPON_DEFINITIONS[weaponType]);
    }

    return(new WeaponDefinition());
  }

  public void SpawnEnemy() {
    // Pick a random Enemy prefab to instantiate
    int ndx = Random.Range(0, prefabEnemies.Length);
    GameObject gameObject = Instantiate( prefabEnemies[ ndx ] ) as GameObject;
    
    // Position the Enemy above the screen with a random x position
    Vector3 position = Vector3.zero;
    float xMin = Utils.cameraBounds.min.x+enemySpawnPadding;float xMax = Utils.cameraBounds.max.x-enemySpawnPadding;
    position.x = Random.Range( xMin, xMax );
    position.y = Utils.cameraBounds.max.y + enemySpawnPadding;
    gameObject.transform.position = position;
    
    // Spawn another enemy
    Invoke( "SpawnEnemy", enemySpawnRate );
  }

  public void ShipDestroyed( Enemy enemy ) {
    if (Random.value <= enemy.powerUpDropChance) {
      // Decide PowerUp type
      int index = Random.Range(0,powerUpFrequency.Length);
      WeaponType powerUpType = powerUpFrequency[index];
      
      // Create PowerUp
      GameObject gameObject = Instantiate( prefabPowerUp ) as GameObject;
      PowerUp powerUp = gameObject.GetComponent<PowerUp>();
      powerUp.SetType( powerUpType );

      // Place on screen
      powerUp.transform.position = enemy.transform.position;
    }
  }

  void Start() {
    activeWeaponTypes = new WeaponType[weaponDefinitions.Length];
    for(int i=0; i<weaponDefinitions.Length; i++) {
      activeWeaponTypes[i] = weaponDefinitions[i].type;
    }
  }

  public void DelayedRestart( float delay ) {
    Invoke("Restart", delay);
  }
  public void Restart() {
    SceneManager.LoadScene("Game");
  }
}

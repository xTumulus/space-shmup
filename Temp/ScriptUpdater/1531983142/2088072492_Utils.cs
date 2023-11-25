using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoundsTest {
  center,
  onScreen,
  offScreen
}

public class Utils: MonoBehaviour
{
  // _____BOUNDS UTILS_____

  //Creates a bound that encapsulates two input bounds
  static public Bounds BoundsUnion(Bounds b0, Bounds b1) {
    if (b0.size == Vector3.zero && b1.size != Vector3.zero) {
      return b1;
    }
    else if (b0.size != Vector3.zero && b1.size == Vector3.zero) {
      return b0;
    }
    else if (b0.size == Vector3.zero && b1.size == Vector3.zero) {
      return b0;
    }

    b0.Encapsulate(b1.min);
    b0.Encapsulate(b1.max);
    return(b0);
  }

  static public Bounds CombineChildrenBounds(GameObject gameObject) {
    Bounds b = new Bounds(Vector3.zero, Vector3.zero);

    if(gameObject.GetComponent<Renderer>() != null) {
      b = BoundsUnion(b, gameObject.GetComponent<Renderer>().bounds);
    }

    if(gameObject.GetComponent<Collider>() != null) {
      b = BoundsUnion(b, gameObject.GetComponent<Collider>().bounds);
    }

    foreach (Transform t in gameObject.transform) {
      b = BoundsUnion(b, CombineChildrenBounds(t.gameObject));
    }

    return(b);
  }

  static private Bounds _cameraBounds;
  static public Bounds cameraBounds {
    get {
      if (_cameraBounds.size == Vector3.zero) {
        SetCameraBounds();
      }

      return _cameraBounds;
    }
  }

  // Method used by cameraBounds to set _cameraBounds. 
  // Assumes camera is Orthographic and R:[0,0,0]
  public static void SetCameraBounds(Camera camera=null) {
    if (camera == null) {
      camera = Camera.main;
    }

    Vector3 topLeft = new Vector3( 0, 0, 0 );
    Vector3 bottomRight = new Vector3( Screen.width, Screen.height, 0 );
    
    // Convert to world coordinates
    Vector3 boundTLN = camera.ScreenToWorldPoint( topLeft );
    Vector3 boundBRF = camera.ScreenToWorldPoint( bottomRight );

    // Adjust their zs to near and far Camera clipping planes
    boundTLN.z += camera.nearClipPlane;
    boundBRF.z += camera.farClipPlane;

    // Find bound centers
    Vector3 center = (boundTLN + boundBRF)/2f;
    _cameraBounds = new Bounds( center, Vector3.zero );

    // Expand _cameraBounds to encapsulate the extents.
    _cameraBounds.Encapsulate( boundTLN );
    _cameraBounds.Encapsulate( boundBRF );
  }

  // Checks to see whether the Bounds bnd are within the camBounds
public static Vector3 ScreenBoundsCheck(Bounds bounds, BoundsTest test = BoundsTest.center) {
  return( BoundsInBoundsCheck( cameraBounds, bounds, test ) );
}

// Checks to see whether Bounds lilB are within Bounds bigB
public static Vector3 BoundsInBoundsCheck( Bounds largeBounds, Bounds smallBounds, BoundsTest test = BoundsTest.onScreen ) {
  Vector3 smallBoundsPosition = smallBounds.center;
  Vector3 smallBoundsOffset = Vector3.zero;

  switch (test) {
    // center test determines what offset moves smallBounds center inside largeBounds
    case BoundsTest.center:
      
      if ( largeBounds.Contains( smallBoundsPosition ) ) {
        return( Vector3.zero );
      }
      
      if (smallBoundsPosition.x > largeBounds.max.x) {
        smallBoundsOffset.x = smallBoundsPosition.x - largeBounds.max.x;
      } else if (smallBoundsPosition.x < largeBounds.min.x) {
        smallBoundsOffset.x = smallBoundsPosition.x - largeBounds.min.x;
      }
      
      if (smallBoundsPosition.y > largeBounds.max.y) {
        smallBoundsOffset.y = smallBoundsPosition.y - largeBounds.max.y;
      } else if (smallBoundsPosition.y < largeBounds.min.y) {
        smallBoundsOffset.y = smallBoundsPosition.y - largeBounds.min.y;
      }
      
      if (smallBoundsPosition.z > largeBounds.max.z) {
        smallBoundsOffset.z = smallBoundsPosition.z - largeBounds.max.z;
      } else if (smallBoundsPosition.z < largeBounds.min.z) {
        smallBoundsOffset.z = smallBoundsPosition.z - largeBounds.min.z;
      }

      return( smallBoundsOffset );

    // onScreen test determines what offset keeps all of smallBounds inside largeBounds
    case BoundsTest.onScreen:
  
      if ( largeBounds.Contains( smallBounds.min ) && largeBounds.Contains( smallBounds.max ) ) {
        return( Vector3.zero );
      }
      
      if (smallBounds.max.x > largeBounds.max.x) {
        smallBoundsOffset.x = smallBounds.max.x - largeBounds.max.x;
      } else if (smallBounds.min.x < largeBounds.min.x) {
        smallBoundsOffset.x = smallBounds.min.x - largeBounds.min.x;
      }
      
      if (smallBounds.max.y > largeBounds.max.y) {
        smallBoundsOffset.y = smallBounds.max.y - largeBounds.max.y;
      } else if (smallBounds.min.y < largeBounds.min.y) {
        smallBoundsOffset.y = smallBounds.min.y - largeBounds.min.y;
      }
      
      if (smallBounds.max.z > largeBounds.max.z) {
        smallBoundsOffset.z = smallBounds.max.z - largeBounds.max.z;
      } else if (smallBounds.min.z < largeBounds.min.z) {
        smallBoundsOffset.z = smallBounds.min.z - largeBounds.min.z;
      }
      
      return( smallBoundsOffset );

    // The offScreen test determines what offset moves any part of smallBounds inside of largeBounds
    case BoundsTest.offScreen:
      bool cMin = largeBounds.Contains( smallBounds.min );
      bool cMax = largeBounds.Contains( smallBounds.max );
      
      if ( cMin || cMax ) {
      return( Vector3.zero );
      }
      
      if (smallBounds.min.x > largeBounds.max.x) {
        smallBoundsOffset.x = smallBounds.min.x - largeBounds.max.x;
      } else if (smallBounds.max.x < largeBounds.min.x) {
        smallBoundsOffset.x = smallBounds.max.x - largeBounds.min.x;
      }
      
      if (smallBounds.min.y > largeBounds.max.y) {
        smallBoundsOffset.y = smallBounds.min.y - largeBounds.max.y;
      } else if (smallBounds.max.y < largeBounds.min.y) {
        smallBoundsOffset.y = smallBounds.max.y - largeBounds.min.y;
      }
      
      if (smallBounds.min.z > largeBounds.max.z) {
        smallBoundsOffset.z = smallBounds.min.z - largeBounds.max.z;
      } else if (smallBounds.max.z < largeBounds.min.z) {
        smallBoundsOffset.z = smallBounds.max.z - largeBounds.min.z;
      }
      
      return( smallBoundsOffset );
    }

    return( Vector3.zero );
  }

  //________TRANSFORM_UTILS__________

  // Climbs the transform.parent tree until it finds a parent with a tag
  public static GameObject FindTaggedParent(GameObject gameObject) {
    if (gameObject.tag != "Untagged") {
      return(gameObject);
    }
  
    if (gameObject.transform.parent == null) {
      return( null );
    }
    
    // recurse tree
    return( FindTaggedParent( gameObject.transform.parent.gameObject ) );
  }

  // allows input of Transform
  public static GameObject FindTaggedParent(Transform t) {
    return( FindTaggedParent( t.gameObject ) );
  }

  //________MATERIAL_UTILS________

  // Returns a list of all Materials on this GameObject or its children
  static public Material[] GetAllMaterials(GameObject gameObject) {
    List<Material> materials = new List<Material>();
    if (gameObject != null) {
      materials.Add(gameObject.GetComponent<Renderer>().material);
    }
    
    foreach(Transform transform in gameObject.transform) {
      materials.AddRange(GetAllMaterials(transform.gameObject));
    }

    return(materials.ToArray());
  }

}

// The PrintAwake script is placed on a GameObject.  The Awake function is
// called when the GameObject is started at runtime.  The script is also
// called by the Editor.  An example is when the scene is changed to a
// different scene in the Project window.
// The Update() function is called, for example, when the GameObject transform
// position is changed in the Editor.

using UnityEngine;
using UnityEditor;

// [ExecuteInEditMode]
[CustomEditor( typeof( ParentDrop ) )]
public class ParentDropEditor : Editor
{

    void OnSceneGUI(){
        
        // get the chosen game object
        ParentDrop t = target as ParentDrop;

        if( t == null)
            return;

        Event e = Event.current;

        if(t.justSpawned){

            t.justSpawned = false;

            Vector3 mousePos = e.mousePosition;
            mousePos.z = 10;

            Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);

            RaycastHit2D hitInfo2D;

            hitInfo2D = Physics2D.Raycast(ray.origin, Vector2.zero);
            if(hitInfo2D){
                t.transform.parent = hitInfo2D.transform.GetChild(0);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

[CustomEditor(typeof(PlaceableObject))]
public class EditorObjectChanger : Editor
{
    PlaceableObject o;
    ObjectTypes type;

    private void OnEnable()
    {
        o = target as PlaceableObject;

        type = o.type;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUI.changed)
        {
            if (o.type != type)
            {
                DestroyImmediate(o.gameObject.GetComponent<ShelveObject>());
                DestroyImmediate(o.gameObject.GetComponent<CheckoutObject>());

                switch (o.type)
                {
                    case ObjectTypes.shelve:
                        o.gameObject.AddComponent<ShelveObject>();
                        break;
                    case ObjectTypes.checkout:
                        o.gameObject.AddComponent<CheckoutObject>();
                        break;
                }

                type = o.type;
            }
        }
    }
}

#endif

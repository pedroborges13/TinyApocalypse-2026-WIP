using UnityEngine;

public enum PlacedObjectType { Barrier, ExplosiveBarrel, Landmine}

public class PlaceableObject : MonoBehaviour
{
    [SerializeField] private PlacedObjectType objectType;

    public PlacedObjectType ObjectType => objectType;
    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;
}

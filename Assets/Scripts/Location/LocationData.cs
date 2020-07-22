using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationData : MonoBehaviour {
    public Loader.Scene scene;
    public Vector3 startingLocation;
    public Direction startingFacing;
    public bool exitAtEdge = true;
}
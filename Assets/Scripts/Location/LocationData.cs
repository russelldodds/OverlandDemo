using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationData : MonoBehaviour {
    public Loader.Scene scene;
    public List<Transform> startingLocations;
    public Direction startingFacing;
    public bool exitAtEdge = true;

    public Vector3 GetRandomStartingLocation() {
        int index = Random.Range(0, 1000 * startingLocations.Count) % startingLocations.Count;
        return startingLocations[index].position;
    }
}
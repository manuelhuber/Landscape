using System;
using System.Collections.Generic;
using System.Linq;
using Grimity.GameObjects;
using UnityEngine;

namespace Buildings {
public class Placeable : MonoBehaviour {
    private MeshFilter _filter;
    private BoxCollider _floorChecker;
    private List<Vector3> _terrainVertices;
    private List<Vector3> _terrainVerticesWorldSpace;
    private Mesh _terrainMesh;
    private MeshCollider _terrainCollider;
    private GameObject _terrain;
    private List<Tuple<int, Vector3>> _collisionVertices = new List<Tuple<int, Vector3>>();

    private void Start() {
        _filter = GetComponent<MeshFilter>();
        _floorChecker = gameObject.AddComponent<BoxCollider>();
        var bounds = _filter.sharedMesh.bounds;
        // TODO remove magic numbers
        _floorChecker.center = new Vector3(0, -0.5f, 0);
        _floorChecker.isTrigger = true;
        var floorCheckerSize = bounds.size;
        floorCheckerSize.y = 20;
        floorCheckerSize.x = 3;
        floorCheckerSize.z = 3;

        _floorChecker.size = floorCheckerSize;
    }

    public void Place() {
        var collisionVertices = CollisionVertices();

        var avg = collisionVertices.Select(tuple => tuple.Item2.y)
                      .Aggregate((sum, value) => sum + value) /
                  collisionVertices.Count;
        foreach (var (index, pos) in collisionVertices) {
            _terrainVertices.RemoveAt(index);
            _terrainVertices.Insert(index, new Vector3(pos.x, avg, pos.z));
        }


        var newHeight = Math.Abs(Geometry.LowerCenter(gameObject).y) + avg;

        transform.SetY(newHeight);
        _terrainMesh.SetVertices(_terrainVertices);
        _terrainCollider.sharedMesh = null;
        _terrainCollider.sharedMesh = _terrainMesh;

        Destroy(this);
    }

    private void OnTriggerEnter(Collider other) {
        _terrainMesh = other.GetComponent<MeshFilter>().sharedMesh;
        _terrainCollider = other.GetComponent<MeshCollider>();
        _terrain = other.gameObject;
        _terrainVertices = _terrainMesh.vertices.ToList();
        var localToWorld = _terrain.transform.localToWorldMatrix;
        _terrainVerticesWorldSpace =
            _terrainVertices.Select(vector3 => localToWorld.MultiplyPoint3x4(vector3)).ToList();
    }

    private void OnTriggerStay(Collider other) {
        var collisionVertices = CollisionVertices();
        var heights = collisionVertices.Select(tuple => tuple.Item2.y).ToArray();
        var dif = heights.Max() - heights.Min();
        // TODO implement "not placeable" feature
        if (dif > 2) {
            Debug.Log("Bad place");
        }
    }

    private List<Tuple<int, Vector3>> CollisionVertices() {
        _collisionVertices.Clear();
        for (var i = 0; i < _terrainVertices.Count; i++) {
            if (_floorChecker.bounds.Contains(_terrainVerticesWorldSpace[i])) {
                _collisionVertices.Add(new Tuple<int, Vector3>(i, _terrainVertices[i]));
            }
        }

        return _collisionVertices;
    }
}
}
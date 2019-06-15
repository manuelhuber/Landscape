using Buildings;
using Grimity.Cursor;
using Grimity.GameObjects;
using Terrain.Generation;
using UnityEngine;

namespace UI {
public class UserInput : MonoBehaviour {
    public LayerMask terrainLayer;
    public LayerMask clickableLayers;

    private Camera _camera;

    private DraggableObject _draggedObject;
    private bool _dragObject;
    private Placeable _placeable;
    private EndlessTerrainGenerator _terrainGenerator;
    private float _leftClickDownTime;
    private double _clickInterval = 0.5;
    private Vector3 _mouseStart;

    private void Start() {
        _camera = GetComponent<Camera>();
        if (_camera == null) {
            _camera = Camera.main;
        }

        _terrainGenerator = FindObjectOfType<EndlessTerrainGenerator>();
    }

    private void Update() {
        if (_dragObject) {
            _draggedObject.GameObject.transform.position = MouseToTerrain().point - _draggedObject.LowerCenter;
        }

        var rightClick = Input.GetMouseButtonUp(1);
        var leftClickDown = Input.GetMouseButtonDown(0);
        var leftClickPressed = Input.GetMouseButton(0);
        if (leftClickDown) _leftClickDownTime = Time.time;
        var leftClickUp = Input.GetMouseButtonUp(0);
        var isLeftClick = leftClickUp && Time.time - _leftClickDownTime < _clickInterval;

        if (Input.GetKeyDown(KeyCode.Space)) {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            AttachToCursor(cube);
        }

        if (isLeftClick && Input.GetKey(KeyCode.LeftShift)) {
            var terrainChunk = MouseToTerrain().transform.gameObject.GetComponent<TerrainChunk>();
            _terrainGenerator.SpawnNeighbours(terrainChunk);
        }

        if (isLeftClick && _dragObject) {
            DetachFromCursor();
        }

        if (leftClickPressed) {
            var cameraTransform = _camera.transform;
            var pos = cameraTransform.position;
            var diff = Input.mousePosition - _mouseStart;
            var movement = new Vector3(diff.x, 0, diff.y);
            
            cameraTransform.position = pos;
        }

        _mouseStart = Input.mousePosition;
    }


    private void DetachFromCursor() {
        _dragObject = false;
        _placeable.Place();
    }

    private void AttachToCursor(GameObject gameObject) {
        _placeable = gameObject.AddComponent<Placeable>();

        _dragObject = true;
        _draggedObject = new DraggableObject(gameObject, Geometry.LowerCenter(gameObject));
    }

    private RaycastHit MouseToTerrain() {
        CursorUtil.GetCursorLocation(out RaycastHit terrainHit, terrainLayer, _camera);
        return terrainHit;
    }
}

struct DraggableObject {
    public GameObject GameObject;
    public Vector3 LowerCenter;

    public DraggableObject(GameObject gameObject, Vector3 lowerCenter) {
        GameObject = gameObject;
        LowerCenter = lowerCenter;
    }
}
}
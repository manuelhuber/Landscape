using Grimity.Cursor;
using Grimity.GameObjects;
using UnityEngine;

namespace UI {
public class UserInput : MonoBehaviour {
    public LayerMask terrainLayer;
    public LayerMask clickableLayers;

    private Camera _camera;

    private DraggableObject _draggedObject;
    private bool _dragObject;

    private void Start() {
        _camera = GetComponent<Camera>();
        if (_camera == null) {
            _camera = Camera.main;
        }
    }

    private void Update() {
        if (_dragObject) {
            _draggedObject.GameObject.transform.position = MouseToTerrain() - _draggedObject.LowerCenter;
        }

        var rightClick = Input.GetMouseButtonUp(1);
        var leftClickDown = Input.GetMouseButtonDown(0);
        var leftClickUp = Input.GetMouseButtonUp(0);

        if (Input.GetKeyDown(KeyCode.Space)) {
            Debug.Log("PRESSED BUILD BUTTON");
            AttachToCursor(GameObject.CreatePrimitive(PrimitiveType.Cube));
        }

        if (leftClickUp && _dragObject) {
            DetachFromCursor();
        }
    }


    private void DetachFromCursor() {
        _dragObject = false;
    }

    private void AttachToCursor(GameObject gameObject) {
        _dragObject = true;
        _draggedObject = new DraggableObject(gameObject, Geometry.LowerCenter(gameObject));
    }

    private Vector3 MouseToTerrain() {
        CursorUtil.GetCursorLocation(out Vector3 terrainHit, terrainLayer, _camera);
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
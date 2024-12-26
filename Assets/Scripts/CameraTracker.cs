using UnityEngine;

public class CameraTracker : MonoBehaviour {
    public float height = 10f;
    public float xOffset = 0f;
    public float zOffset = 0f;
    public float fov = 90f;
    public bool ortographic = false;
    public float ortographicZoom = 5f;
    public GameObject track = null;
    public Camera cam = null;

    void Start() {
        if (ortographic == true) {
            cam.orthographic = true;
            cam.orthographicSize = ortographicZoom;
        } else {
            cam.orthographic = false;
            cam.fieldOfView = fov;
        }
    }

    void Update() {

        if (ortographic == true) {
            cam.orthographic = true;
            cam.orthographicSize = ortographicZoom;
        } else {
            cam.orthographic = false;
            cam.fieldOfView = fov;
        }

        //tracking object
        Vector3 newPos = transform.position;
        newPos.x = track.transform.position.x + xOffset;
        newPos.z = track.transform.position.z + zOffset;
        newPos.y = height;
        transform.position = newPos;
        cam.transform.LookAt(track.transform);

        //object mouse rotation
        Vector3 mouseScreenPosition = Input.mousePosition;
        Vector3 mouseWorldPosition = cam.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, cam.transform.position.y - track.transform.position.y));
        Vector3 direction = (mouseWorldPosition - track.transform.position);
        direction.y = 0; //flatten to 2d vector 
        direction.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        track.transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y - 90, 0);
        
    }

    public Vector3 RemapInputs(float forward, float right){
        Vector3 camForward = new Vector3(-xOffset, 0, -zOffset);
        Vector3 camRight = Vector3.Cross(Vector3.up, camForward).normalized;
        Vector3 inputDirection = (camForward * right + camRight * forward).normalized;

        return camForward.magnitude == 0 ? new Vector3(forward, 0, right) : inputDirection;
    }
}

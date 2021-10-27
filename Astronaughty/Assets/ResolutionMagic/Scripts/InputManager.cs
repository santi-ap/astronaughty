using UnityEngine;
using System.Collections;

namespace ResolutionMagic
{

    // this is a simple input script to show how to move the camera with the arrow keys
    
    public class InputManager : MonoBehaviour
    {

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                ResolutionManager.Instance.MoveCamera(ResolutionManager.CameraDirections.Up, 0.25f);

            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                ResolutionManager.Instance.MoveCamera(ResolutionManager.CameraDirections.Down,0.25f);

            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                ResolutionManager.Instance.MoveCamera(ResolutionManager.CameraDirections.Left,0.25f);

            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                ResolutionManager.Instance.MoveCamera(ResolutionManager.CameraDirections.Right,0.25f);

            }
        }
    }
}
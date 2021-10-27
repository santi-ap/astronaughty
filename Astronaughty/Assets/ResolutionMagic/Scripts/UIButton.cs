using UnityEngine;
using System.Collections;

namespace ResolutionMagic
{
    public class UIButton : MonoBehaviour
    {
        // this is a sample script that powers the buttons in the sample scenes
        // this isn't technically a part of Resolution Magic, but it does show a few examples of calling ResolutionManager methods and properties
 

        Vector2 inputPos;
        RaycastHit2D hit;
        public LayerMask mask = -1;
     
      
        void Update()
        {
           MouseControls();
        }

        void MouseControls()
        {
            inputPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);          //Get the position of the input       
            hit = Physics2D.Raycast(inputPos, new Vector2(0, 0), 0.1f, mask);             //Cast a ray to detect objets
            ScanForInput();
        }
     
        void ScanForInput()
        {          
            if (HasInput() && hit.collider != null)
            {
                AlignedObject hitButton = hit.transform.GetComponent<AlignedObject>();
                

                if (hitButton != null && !hitButton.Container)
                    hitButton.Bounce();

                if (hit.transform.name == "show_button")
                    ResolutionManager.Instance.ShowUI();

                if (hit.transform.name == "hide_button")
                    ResolutionManager.Instance.HideUI();

                if (hit.transform.name == "refresh_button")
                    ResolutionManager.Instance.RefreshResolution();

                if (hit.transform.name == "toggle_bars")
                {
                    GameObject.FindGameObjectWithTag("RM_Black_Bars").GetComponent<BlackBars>().Enabled =
                        !GameObject.FindGameObjectWithTag("RM_Black_Bars").GetComponent<BlackBars>().Enabled;
                }

                if (hit.transform.name == "flip_gravity")
                    FlipGravity(hit.transform);

                if (hit.transform.name == "zoom_type")
                {
                    if (ResolutionManager.Instance.ZoomTo == ResolutionManager.ZoomTypes.Canvas)
                        ResolutionManager.Instance.ZoomTo = ResolutionManager.ZoomTypes.MaxSize;
                    else
                        ResolutionManager.Instance.ZoomTo = ResolutionManager.ZoomTypes.Canvas;

                    ResolutionManager.Instance.RefreshResolution();
               
                }
            }
        }

        bool HasInput()
        {
            return Input.GetMouseButtonDown(0);          //Returns true if there is an active input
        }

        void FlipGravity(Transform button)
        {
            Physics2D.gravity *= -1;
            int flip;
            if (Physics2D.gravity.y < 0)
                flip = 180;
            else
                flip = 0;
            button.localRotation = Quaternion.Euler(new Vector3(0,0, flip));


        }


    }

}

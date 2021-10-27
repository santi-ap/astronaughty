using UnityEngine;
using System.Collections;

namespace ResolutionMagic
{
    public class BorderScript : MonoBehaviour
    {

        // this script calculates the correct size and position of border edges it's attached to
        // you should not need to modify the properties unless you want something unusual
        // to have walls on some edges but not others, simply disable (or remove) the edges you don't want to use
        // NOTE: the MoveEdge method is called from the Resolution Manager script, but only when the 'Use Solid Border' option is selected


        // each edge of the border must be aligned to a screen edge, such as the top or left
        [SerializeField]
        ResolutionManager.Edges alignTo;

        // alignType determines if the border is placed at the edge of the screen or at the edge of the canvas
        // you would align it to the canvas if your border is around a set play area, and align to the screen if you want a border at the edge of the player's screen regardless of resolution/ratio
        [SerializeField]
        ResolutionManager.AlignObjects alignType;

        Transform myTransform; // reference to the transform

        void Start()
        {
            myTransform = transform; // cache the transform
        }

        public void MoveEdge()
        {
            // place this at its correct location on the screen edge
            if (alignType == ResolutionManager.AlignObjects.Canvas)
                myTransform.position = ResolutionManager.Instance.CanvasEdgeAsVector(alignTo);
            else
                myTransform.position = ResolutionManager.Instance.ScreenEdgeAsVector(alignTo);

            // get the height or width of the object by setting its scale to 1 and then measuring
            float mySize = 0;
            myTransform.localScale = new Vector3(1, 1, 0);
            switch (alignTo)
            {
                case ResolutionManager.Edges.Bottom:
                case ResolutionManager.Edges.Top:
                    mySize = myTransform.GetComponent<Renderer>().bounds.size.x;
                    break;
                case ResolutionManager.Edges.Left:
                case ResolutionManager.Edges.Right:
                    mySize = myTransform.GetComponent<Renderer>().bounds.size.y;
                    break;
            }

            float scale = 1;

            // scale the transform to match the aligned edge, e.g. canvas left
            // note: since the screen and canvas are always rectangular the left/right and top/bottom always match
            switch (alignTo)
            {
                case ResolutionMagic.ResolutionManager.Edges.Bottom:
                case ResolutionMagic.ResolutionManager.Edges.Top:
                    if (alignType == ResolutionManager.AlignObjects.Canvas)
                        scale = ResolutionManager.Instance.CanvasWidth / mySize;
                    else
                        scale = ResolutionManager.Instance.ScreenWidth / mySize;

                    myTransform.localScale = new Vector3(scale, 1, 0);
                    break;

                case ResolutionMagic.ResolutionManager.Edges.Left:
                case ResolutionMagic.ResolutionManager.Edges.Right:
                    if (alignType == ResolutionManager.AlignObjects.Canvas)
                        scale = ResolutionManager.Instance.CanvasHeight / mySize;
                    else
                        scale = ResolutionManager.Instance.ScreenHeight / mySize;

                    myTransform.localScale = new Vector3(1, scale, 0);
                    break;
            }

        }
    }
}
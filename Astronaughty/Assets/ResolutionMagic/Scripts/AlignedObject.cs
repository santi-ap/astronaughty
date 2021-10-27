using UnityEngine;
using System.Collections;

namespace ResolutionMagic
{
    public class AlignedObject : MonoBehaviour
    {
        // put this script on any UI buttons you want to move off the screen (and back on again) by using the "HideUI" and "ShowUI" methods in the ResolutionManager script

        #region USER SETTINGS


        // AlignedPoint is the placement, e.g. top left corner or bottom
        // the object will be placed relative to this edge of the screen (but you design it relative to the canvas - e.g. in the designer your button is in the top-left of the canvas space, but at runtime it will move to the top-left of the screen)
        // if you align to the centre, you effectively keep the object in its position within the canvas space
        [SerializeField]
        ResolutionMagic.ResolutionManager.AlignPoints AlignedPoint;

        // EdgeToMoveOff determines the direction the object slides off/on the screen. This should usually match the AlignedPoint, but doesn't have to
        // NOTE: if you choose the centre, your object will move to the centre of the VISIBLE screen instead of moving off the screen
        [SerializeField]
        ResolutionMagic.ResolutionManager.AlignPoints EdgeToMoveOff;

        [SerializeField]
        float hideTime; // how long in seconds it takes to hide this UI item (set in the inspector)
        [SerializeField]
        float showTime; // how long in seconds it takes to show this UI item (set in the inspector)
        [SerializeField]
        bool StartOffScreen;

        [SerializeField]
        public bool DontMove; // ignore requests to move on/off the screen by the manager (can still be done manually via the HideThis() and ShowThis() public methods
        [SerializeField]
        public bool IgnoreManager; // ignore all commands from the manager for moving on/off the screen and being aligned to the screen - HideThis() and ShowThis() are still available, and the Bounce() method still works

        // containers are groups of aligned objects, such as a bar with multiple buttons
        // for scaling and positioning the container is considered one item (so the bar of buttons moves together as one)
        // individual buttons inside the container should have IgnoreManager selected so they inherit their position and scaling from the container
        public bool Container = false;

        #endregion


        #region PROPERTIES
        Vector2 origStartCentre; // where the object started in the designer, within the canvas - NEVER CHANGES
        Vector2 posWhenVisibleCentre; // the position of the object when visible on the screen, modified according to screen ratio if required
        Vector2 posWhenOffScreenCentre; // this is where the object will 'hide' when not visible on the screen
        Vector2 origDistFromEdgeEdge; // distance from the screen edges it's aligned to, used to place the item when the screen size is different from the canvas

        bool VisibleOnScreen = true; // whether the object is currently visible to the player
        float width
        {
            get
            {
                if (this.GetComponent<BoxCollider2D>())
                    return this.GetComponent<Collider2D>().bounds.size.x;
                else
                    return this.GetComponent<Renderer>().bounds.size.x;
            }
        } // the object's width, used for scaling
        float height
        {
            get
            {
                if (this.GetComponent<BoxCollider2D>())
                    return this.GetComponent<Collider2D>().bounds.size.y;
                else
                    return this.GetComponent<Renderer>().bounds.size.y;
            }
        } // the object's height, used for scaling

        Vector2 AlignedEdgePoint
        {
            // returns a vector from origin 0,0 for the position of the edge/corner of this item that is facing the aligned edge
            // e.g. the top-left corner if aligned to the top-left corner
            // this is based on the position of the object at the time this is called
            get
            {
                float distX = width / 2;
                float distY = height / 2;

                switch (AlignedPoint)
                {
                    case ResolutionManager.AlignPoints.Centre:
                        return new Vector2(myTransform.position.x, myTransform.position.y);

                    case ResolutionManager.AlignPoints.Left:
                        return new Vector2(myTransform.position.x - distX, myTransform.position.y);


                    case ResolutionManager.AlignPoints.Bottom:
                        return new Vector2(myTransform.position.x, myTransform.position.y - distY);


                    case ResolutionManager.AlignPoints.Right:
                        return new Vector2(myTransform.position.x + distX, myTransform.position.y);

                    case ResolutionManager.AlignPoints.Top:
                        return new Vector2(myTransform.position.x, myTransform.position.y + distY);


                    case ResolutionManager.AlignPoints.TopLeftCorner:
                        return new Vector2(myTransform.position.x - distX, myTransform.position.y + distY);


                    case ResolutionManager.AlignPoints.TopRightCorner:
                        return new Vector2(myTransform.position.x + distX, myTransform.position.y + distY);


                    case ResolutionManager.AlignPoints.BottomLeftCorner:
                        return new Vector2(myTransform.position.x - distX, myTransform.position.y - distY);


                    case ResolutionManager.AlignPoints.BottomRightCorner:
                        return new Vector2(myTransform.position.x + distX, myTransform.position.y - distY);


                }
                return new Vector2(0, 0);

            }
        }

        Vector2 EdgeToCentreOffset
        {
            // distance from the centre of the object to it's own edge which is aligned to the screen/canvas edge
            // this is used internally to calculate position
            get
            {
                float distX = width / 2;
                float distY = height / 2;

                switch (AlignedPoint)
                {
                    case ResolutionManager.AlignPoints.Left:
                        return new Vector2(distX, 0);


                    case ResolutionManager.AlignPoints.Bottom:
                        return new Vector2(0, distY);


                    case ResolutionManager.AlignPoints.Right:
                        return new Vector2(-distX, 0);

                    case ResolutionManager.AlignPoints.Top:
                        return new Vector2(0, -distY);


                    case ResolutionManager.AlignPoints.TopLeftCorner:
                        return new Vector2(distX, -distY);


                    case ResolutionManager.AlignPoints.TopRightCorner:
                        return new Vector2(-distX, -distY);


                    case ResolutionManager.AlignPoints.BottomLeftCorner:
                        return new Vector2(distX, distY);


                    case ResolutionManager.AlignPoints.BottomRightCorner:
                        return new Vector2(-distX, distY);
                }

                return Vector2.zero;
            }
        }
        Vector3 GetOffScreenPos(Vector2 onScreenPos)
        {

            // this returns a position to 'hide' the object outside of the visible screen
            // this is used when the object is sent off the screen


            // calculate the height and width by checking the object's renderer or collider
            // for composite objects (e.g. a row of multiple buttons) that don't have their own renderer, adding a box collider 2D enclosing all the buttons ensures the correct size is detected
            // set the collider to a trigger if you don't want it to collide with things

            Vector2 newPos = Vector2.zero;
            float distanceFromLeft = Mathf.Abs(ResolutionManager.Instance.ScreenLeftEdge - onScreenPos.x);
            float distanceFromBottom = Mathf.Abs(ResolutionManager.Instance.ScreenBottomEdge - onScreenPos.y);
            float distanceFromRight = Mathf.Abs(ResolutionManager.Instance.ScreenRightEdge - onScreenPos.x);
            float distanceFromTop = Mathf.Abs(ResolutionManager.Instance.ScreenTopEdge - onScreenPos.y);

            switch (EdgeToMoveOff)
            {
                case ResolutionMagic.ResolutionManager.AlignPoints.Left:
                    newPos = new Vector2(onScreenPos.x - (distanceFromLeft + width), onScreenPos.y);
                    break;

                case ResolutionMagic.ResolutionManager.AlignPoints.Bottom:
                    newPos = new Vector2(onScreenPos.x, onScreenPos.y - (distanceFromBottom + height));
                    break;

                case ResolutionMagic.ResolutionManager.AlignPoints.Right:
                    newPos = new Vector2(onScreenPos.x + distanceFromRight + width, onScreenPos.y);
                    break;

                case ResolutionMagic.ResolutionManager.AlignPoints.Top:
                    newPos = new Vector2(onScreenPos.x, onScreenPos.y + (distanceFromTop + height));
                    break;

                case ResolutionMagic.ResolutionManager.AlignPoints.TopLeftCorner:
                    newPos = new Vector2(onScreenPos.x - (distanceFromLeft + width), onScreenPos.y + (distanceFromTop + height));
                    break;

                case ResolutionMagic.ResolutionManager.AlignPoints.TopRightCorner:
                    newPos = new Vector2(onScreenPos.x + (distanceFromRight + width), onScreenPos.y + (distanceFromTop + height));
                    break;

                case ResolutionMagic.ResolutionManager.AlignPoints.BottomLeftCorner:
                    newPos = new Vector2(onScreenPos.x - (distanceFromLeft + width), onScreenPos.y - (distanceFromBottom + height));
                    break;

                case ResolutionMagic.ResolutionManager.AlignPoints.BottomRightCorner:
                    newPos = new Vector2(onScreenPos.x + (distanceFromRight + width), onScreenPos.y - (distanceFromBottom + height));
                    break;

            }

            return newPos;

        }
        #endregion

        // the scale of this object's transform. This is needed to keep the items at the right scale when the ResolutionManager resizes the screen. Based on the x scale only, as should be considered a 'good enough' estimate of scale
        float MyScale;
        bool inAnimation = false;

        Transform myTransform;
        void Awake()
        {

            myTransform = transform;  // reference to the transform to reduce expense of working on the transform
            origStartCentre = myTransform.position;   // store the starting position to calculate placement  


        }

        void Start()
        {
            if (!IgnoreManager)
            {
                origDistFromEdgeEdge = ResolutionManager.Instance.CanvasEdgeAsVector(AlignedPoint) - AlignedEdgePoint;
                MyScale = transform.localScale.x;

                if (StartOffScreen)
                {

                    HideInstant();
                }

            }
        }



        #region PUBLIC METHODS

        // ShowInstant and HideInstant will put the object on or off the screen instantly, without any animated transition
        public void HideInstant()
        {
            myTransform.position = posWhenOffScreenCentre;
            VisibleOnScreen = false;
        }

        public void ShowInstant()
        {
            myTransform.position = posWhenVisibleCentre;
            VisibleOnScreen = true;
        }

        // HideNow and ShowNow will hide/show the object (using animation settings). These methods ignore the 'Ignore Manager' or 'Don't Move' settings.
        public void HideNow()
        {
            if (VisibleOnScreen)
                StartCoroutine("Hide");
        }

        public void ShowNow()
        {
            if (!VisibleOnScreen)
                StartCoroutine("Show");
        }

        // ShowThis / HideThis - will show or hide the object (using its animation settings) but only if the IgnoreManager and DontMove settings are not enabled
        // These are intended to be called only by the ResolutionManager script
        // call the HideNow/ShowNow methods from your own scripts (or the HideInstant/ShowInstant methods to move instantly)
        public void ShowThis()
        {
            //slide this on the screen (if currently off screen) using its settings for direction and speed
            if (!IgnoreManager && !VisibleOnScreen && !DontMove)
                StartCoroutine("Show");
        }
        public void HideThis()
        {
            // slide this off the screen (if currently on screen) using its settings for direction and speed
            if (!IgnoreManager && VisibleOnScreen && !DontMove)
                StartCoroutine("Hide");
        }

        public void RefreshPosition(float scale)
        {
            // forces the object to refresh its position to the current screen dimensions
            // called by ResolutionManager when the resolution is changed
            if (!IgnoreManager)
            {
                scale = scale * MyScale; // apply any extra scaling required according to this item's original scale
                ReAlign(scale);
                CalculatePositions(scale);
                MoveToCurrentPosition();
            }
        }

 

        public void Bounce()
        {
            // animates the button with a simple 'bounce' pressing effect
            // call this when the button is pressed to give visual feedback
            if (!inAnimation)
                StartCoroutine("BounceMe");
        }
        #endregion

        #region PRIVATE METHODS
        void ReAlign(float scale)
        {
            if (IgnoreManager)
                return;
            // resets back to the original position, from which it can be re-placed to fit the current screen resolution
            myTransform.position = origStartCentre;
            myTransform.localScale = new Vector3(scale, scale, 1);
        }

        void CalculatePositions(float scale)
        {
            posWhenVisibleCentre = ResolutionManager.Instance.ScreenEdgeAsVector(AlignedPoint) - origDistFromEdgeEdge + EdgeToCentreOffset;
            posWhenOffScreenCentre = GetOffScreenPos(posWhenVisibleCentre);
        }

        void MoveToCurrentPosition()
        {
            if (VisibleOnScreen)
                myTransform.position = posWhenVisibleCentre;
            else
                myTransform.position = posWhenOffScreenCentre;
        }
        IEnumerator Hide()
        {
            float startTime = Time.time;
            while (Time.time < startTime + hideTime)
            {
                myTransform.position = Vector3.Lerp(posWhenVisibleCentre, posWhenOffScreenCentre, (Time.time - startTime) / hideTime);
                yield return null;
            }
            myTransform.position = posWhenOffScreenCentre;
            VisibleOnScreen = false;
        }

        IEnumerator Show()
        {
            float startTime = Time.time;
            while (Time.time < startTime + showTime)
            {
                myTransform.position = Vector3.Lerp(posWhenOffScreenCentre, posWhenVisibleCentre, (Time.time - startTime) / hideTime);
                yield return null;
            }
            myTransform.position = posWhenVisibleCentre;
            VisibleOnScreen = true;
        }

        IEnumerator BounceMe()
        {
            inAnimation = true;
            float scale = 0.2f;
            Vector3 originalScale = myTransform.localScale;

            float rate = 1.5f / scale;
            float t = 0.0f;

            float d = 0;

            while (t < 1.0f)
            {
                t += Time.deltaTime * rate;
                myTransform.localScale = originalScale + (originalScale * (scale * Mathf.Sin(d * Mathf.Deg2Rad)));

                d = 180 * t;
                yield return new WaitForEndOfFrame();
            }

            myTransform.localScale = originalScale;
            inAnimation = false;

            yield return new WaitForSeconds(0.2f);

        }
        #endregion

    }

}
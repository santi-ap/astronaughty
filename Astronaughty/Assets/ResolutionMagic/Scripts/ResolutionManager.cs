using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Cinemachine;


namespace ResolutionMagic
{
    public class ResolutionManager : MonoBehaviour
    {
        public CinemachineVirtualCamera virtualCamera;
        #region SETTINGS

        /// USER DEFINED SETTINGS
        [Header("Settings")]
        public ZoomTypes ZoomTo;
        // choose to zoom to the canvas (show canvas at maximum size) or screen (show as much content as possible)
        // restricting the screen to the background will prevent the camera from moving too far that area outside of the background is visible to the player
        // this way you can simply move the camera without worrying about blank space showing to the player. The camera will simply not move in the desired direction if it would show empty space.
        [SerializeField]
        bool RestrictScreenToBackground;

        [SerializeField]
        Camera[] ExtraCameras;

        float cameraSpeed = 0.25f;

        [SerializeField]
        bool autoCheckResolutionChange = true; // by default automatically detect when the resolution changes
        [SerializeField]
        float autoCheckFrequency = 0.5f; // by default check for a resolution change every 0.5 seconds

        [SerializeField] bool canvasSpriteVisibleAtRuntime;

        [Header("UI - Deprecated, use Unity's UI instead")]
        [SerializeField]
        public bool ScaleUI;

        // when true the UI buttons with AlignedObject script are scaled according to the screen area / canvas ratio
        // if enabled, UI buttons will be moved to the appropriate screen edge automatically
        // only disable this if you want your buttons to stay at the place you designed them regardless of the screen resolution
        // only affects objects with the AlignedObject script attached
        [SerializeField]
        bool moveUiButtons = true;

        #endregion

        bool resetUI = false; //used internally to notify that the resolution has changed and the UI needs to be adjusted
        bool useSolidBorder = false;
        bool resetBorders = false; // used internally to notify that the borders needs to move because the screen resolution has changed
        AlignedObject[] UiItemsToMove; // a list of UI items this script moves
        BorderScript[] BorderItems; // a list of border items this script moves
        float zoomAccuracy = 0.01f; // how accurate the camera zoom is. Smaller number = greater accuracy, but increases the work required to zoom the camera
        Transform _canvas;
        Transform _maxArea;


        Transform Canvas
        {
            get
            {
                if (_canvas != null)
                    return _canvas;
                else
                {
                    SetupCanvasAndMaxArea();
                    return _canvas;

                }
            }


        } // the canvas object's transform - VITAL for zooming the camera correctly
        Transform MaxArea
        {

            get
            {
                if (_maxArea != null)
                    return _maxArea;
                else
                {
                    SetupCanvasAndMaxArea();
                    return _maxArea;

                }
            }
        }// the maximum gameplay area used to zoom out as far as possible without showing areas outside the game area
        float resHeight = 0f; // and resWidth - used to store the most recent resolution so we can check if it has changed
        float resWidth = 0f;
        static ResolutionManager myInstance;

        // the developer can choose to use a renderer to define the areas to be shown or use the new method of a box collider (box collider is easier to modify in the Unity editor)
        private bool getSizeFromCollider = false;
        private Renderer renderer;
        private BoxCollider2D collider;
        private Renderer maxRenderer;
        private BoxCollider2D maxCollider;


        public static ResolutionManager Instance
        {
            get { return myInstance; }
        }
        float canvasWidth;
        float canvasHeight;
        public bool ForceRefresh = false;
        void Awake()
        {
            myInstance = this;
        }

        void SetupCanvasAndMaxArea()
        {
            GameObject CanvasHolder = GameObject.FindWithTag("RM_Canvas");
            GameObject BackgroundHolder = GameObject.FindWithTag("RM_Background");
            // get a list of buttons that this script will move automatically
            if (moveUiButtons)
                UiItemsToMove = GameObject.FindObjectsOfType<AlignedObject>();

            BorderItems = GameObject.FindObjectsOfType<BorderScript>();
            if (BorderItems != null)
                useSolidBorder = true;

            try
            {
                _canvas = CanvasHolder.GetComponent<Transform>();
            }
            catch
            {
                Debug.Log("Error finding canvas object. Is the ResolutionMagic prefab in your scene?");

            }
            try
            {
                _maxArea = BackgroundHolder.GetComponent<Transform>();
            }
            catch
            {
                Debug.Log("Error finding background object. Is the ResolutionMagic prefab in your scene?");
            }

            // if a collider exists the user is using the new method of using a collider instead of a renderer to box out the area to be displayed
            if (Canvas.GetComponent<Collider2D>() != null)
            {
                getSizeFromCollider = true;
                collider = Canvas.GetComponent<BoxCollider2D>();
                maxCollider = MaxArea.GetComponent<BoxCollider2D>();
            }
            else
            {
                getSizeFromCollider = false;
                renderer = Canvas.GetComponent<Renderer>();
                maxRenderer = MaxArea.GetComponent<Renderer>();
            }
        }

        void Start()
        {
            SetupCanvasAndMaxArea();
            InvokeRepeating("CheckForResolutionChange", 2, autoCheckFrequency);
            RefreshResolution();
            if (!canvasSpriteVisibleAtRuntime)
            {
                var canvRend = _canvas.GetComponent<SpriteRenderer>();
                if (canvRend != null) canvRend.enabled = false;
            }
        }


        #region PUBLIC METHODS
        // these methods are accessible from anywhere in your game as long as there is an instance of this script available
        // you access them like the following example:
        // ResolutionManager.Instance.MoveCamera(CameraDirections.Up, 0.5f);

        public void MoveCamera(CameraDirections direction, float moveDistance, bool moveSafely = false)
        {
            // move the camera in the specified direction and the specified distance
            // if RestrictScreenToBackground or moveSafely is true the camera will only move if the movement won't cause content outside the background region to show on the screen

            Vector3 newCameraPos = Camera.main.transform.position;

            switch (direction) // to move it two directions (e.g. up and left), call this function  for each direction separately
            {
                case CameraDirections.Up:
                    newCameraPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + moveDistance, -10f);
                    break;
                case CameraDirections.Down:
                    newCameraPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y - moveDistance, -10f);
                    break;
                case CameraDirections.Left:
                    newCameraPos = new Vector3(Camera.main.transform.position.x - moveDistance, Camera.main.transform.position.y, -10f);
                    break;
                case CameraDirections.Right:
                    newCameraPos = new Vector3(Camera.main.transform.position.x + moveDistance, Camera.main.transform.position.y, -10f);
                    break;

            }

            if (RestrictScreenToBackground || moveSafely)
                MoveCameraSafely(newCameraPos, direction); // only move the camera if the movement will not reveal space outside the background area
            else
                MoveCameraUnsafely(newCameraPos); // move the camera regardless

            PlaceUI();
        }

        public void RefreshResolution()
        {
            // force the resolution to be checked/changed as per the script's properties
            // this shouldn't be required unless you turn off the automatic checking

            // choose the correct zoom method based on the ZoomTo property
            if (ZoomTo == ZoomTypes.Canvas)
                ZoomCameraToCanvas();
            else if (ZoomTo == ZoomTypes.MaxSize)
                ZoomCameraToMaxGameArea();

            // store the current screen resolution for use elsewhere
            resHeight = Screen.height;
            resWidth = Screen.width;



            // ensure the UI and borders are refreshed on the next frame update in case the resolution has changed
            resetUI = true;
            resetBorders = true;

            // store the canvas dimensions for later use
            canvasHeight = CanvasHeight;
            canvasWidth = CanvasWidth;
            _scaleFactor = 0;

            foreach (Camera cam in ExtraCameras)
            {
                cam.orthographicSize = Camera.main.orthographicSize;
            }
        }


        private float GetMaxHeight
        {
            get
            {
                if (getSizeFromCollider)
                {
                    return maxCollider.bounds.size.y;
                }
                else
                {
                    return maxRenderer.bounds.size.y;
                }
            }
        }

        private float GetMaxWidth
        {
            get
            {
                if (getSizeFromCollider)
                {
                    return maxCollider.bounds.size.x;
                }
                else
                {
                    return maxRenderer.bounds.size.x;
                }
            }
        }


        public void HideUI()
        {
            // move all UI items (with AlignedObject script attached) off the screen (individual objects can be set to ignore this)
            if (UiItemsToMove != null)
            {
                foreach (AlignedObject uiElement in UiItemsToMove)
                {
                    if (!uiElement.IgnoreManager)
                        uiElement.HideThis();
                }

            }
        }

        public void ShowUI()
        {
            // move all UI items (with AlignedObject script attached) on the screen (individual objects can be set to ignore this)
            if (UiItemsToMove != null)
            {
                foreach (AlignedObject uiElement in UiItemsToMove)
                {
                    if (!uiElement.IgnoreManager)
                        uiElement.ShowThis();
                }

            }

        }


        public void TurnOnBlackBars()
        {
            ToggleBlackBars(true);
        }

        public void TurnOffBlackBars()
        {
            ToggleBlackBars(false);
        }



        #endregion

        #region PRIVATE METHODS
        void MoveCameraUnsafely(Vector3 newCameraPosition)
        {
            // move the camera to the specified position
            // this method will always move the camera regardless of what content will be shown to the player
            // it may reveal 'black bars' or objects outside the game area

            Camera.main.transform.position = newCameraPosition;
            PlaceUI();

        }

        void MoveCameraSafely(Vector3 newCameraPosition, CameraDirections direction)
        {

            // an example of moving the camera only if it doesn't make space outside the background visible
            bool safe = false;

            switch (direction)
            {
                case CameraDirections.Up:
                    safe = !PointIsVisibleToCamera(new Vector2(Camera.main.transform.position.x, MaxTopEdge - cameraSpeed));
                    break;
                case CameraDirections.Down:
                    safe = !PointIsVisibleToCamera(new Vector2(Camera.main.transform.position.x, MaxBottomEdge + cameraSpeed));
                    break;
                case CameraDirections.Left:
                    safe = !PointIsVisibleToCamera(new Vector2(MaxLeftEdge + cameraSpeed, Camera.main.transform.position.y));
                    break;
                case CameraDirections.Right:
                    safe = !PointIsVisibleToCamera(new Vector2(MaxRightEdge - cameraSpeed, Camera.main.transform.position.y));
                    break;

            }

            if (safe)
                Camera.main.transform.position = newCameraPosition;
            PlaceUI();

        }
        void PlaceUI()
        {
            resetUI = false;
            if (UiItemsToMove == null)
                return;

            foreach (AlignedObject uiElement in UiItemsToMove)
            {
                float uiScale = 1f;
                if (ScaleUI)
                    uiScale = ScaleFactor;
                uiElement.RefreshPosition(uiScale);
            }

        }

        void ResetBorders()
        {
            resetBorders = false;
            if (BorderItems == null)
                return;

            if (!useSolidBorder)
            {
                foreach (BorderScript bScr in BorderItems)
                {
                    bScr.gameObject.SetActive(false);
                }
            }

            foreach (BorderScript bScr in BorderItems)
            {
                bScr.MoveEdge();
            }
            resetBorders = false;
        }
        void CheckForResolutionChange()
        {
            if (!autoCheckResolutionChange)
                return;

            if (HasResolutionChanged())
                RefreshResolution();

        }
        bool HasResolutionChanged()
        {
            if (resHeight == 0 || resWidth == 0)
                return false; // no resolution is set yet if these values are zero, so ignore

            if (Math.Abs(resWidth - Screen.width) > 2 || Math.Abs(resHeight - Screen.height) > 2)
                return true;

            return false;
        }
        void LateUpdate()
        {
            if (resetUI)
                PlaceUI();

            if (resetBorders)
                ResetBorders();

            if (ForceRefresh)
            {
                RefreshResolution();
                ForceRefresh = false;
            }
        }



        void ZoomCameraToCanvas()
        {
            // zoom the camera in until the specified object is at the edge of the screen
            CentreCameraOn(Canvas);
            // get the position of the object we're zooming in on in viewport co-ordinates
            Vector2 canvasTopLeft = new Vector2((Canvas.transform.position.x - CanvasWidth / 2), Canvas.transform.position.y - CanvasHeight / 2);

            if (PointIsVisibleToCamera(canvasTopLeft))
            {
                ZoomIn(canvasTopLeft);
            }
            else
            {
                ZoomOut(canvasTopLeft);
            }
        }

        void ZoomCameraToMaxGameArea()
        {

            // pseudo code
            // 1. zoom out until at least one of the middle edges is visible to the camera
            // 2. zoom in until none of the middle edges is visible to the camera



            // centre on the canvas first, as this is where all zooming originates
            CentreCameraOn(Canvas);

            // zoom to canvas first as a starting point
            ZoomCameraToCanvas();

            // store vectors for all the points on the extreme edges
            // we need to ensure that each of these points is not visible to the camera while showing as much game area as possible
            Vector2 maxT = new Vector2(0, MaxTopEdge);
            Vector2 maxB = new Vector2(0, MaxBottomEdge);
            Vector2 maxL = new Vector2(MaxLeftEdge, 0);
            Vector2 maxR = new Vector2(MaxRightEdge, 0);
            // we have zoomed in on the canvas, so we now zoom out as far as we can without showing areas outside the background.            
            // we want to zoom as far as we can without making any of the above points visible to the camera IF POSSIBLE
            // NOTE: if the background is not big enough we can't avoid area outside the background being shown to the player (because we would have to zoom in and some of the canvas would not show)

            while (!PointIsVisibleToCamera(maxT) && !PointIsVisibleToCamera(maxL) && !PointIsVisibleToCamera(maxB) && !PointIsVisibleToCamera(maxR))
            {
                Camera.main.orthographicSize += zoomAccuracy;
                if (Input.GetKeyUp(KeyCode.Z))
                    return;
            }

            resHeight = Screen.height;
            resWidth = Screen.width;

        }

        static bool PointIsVisibleToCamera(Vector2 point)
        {
            if (Camera.main.WorldToViewportPoint(point).x < 0 || Camera.main.WorldToViewportPoint(point).x > 1 || Camera.main.WorldToViewportPoint(point).y > 1 || Camera.main.WorldToViewportPoint(point).y < 0)
                return false;

            return true;


        }

        bool PointIsWithinBackground(Vector2 point)
        {

            if (point.x > MaxTopEdge || point.x < MaxBottomEdge || point.y > MaxRightEdge || point.y < MaxLeftEdge)
                return false;

            return true;


        }

        void ZoomOut(Vector2 point)
        {
            // Zoom out the camera until the specified point is visible
            while (!PointIsVisibleToCamera(point))
            {
                Camera.main.orthographicSize += zoomAccuracy;
                //vcam m_Lens ortho
                virtualCamera.m_Lens.OrthographicSize += zoomAccuracy;
            }
        }

        void ZoomIn(Vector2 point)
        {
            // zoom in the camera until the specified point is just outside the camera's view
            while (PointIsVisibleToCamera(point))
            {
                Camera.main.orthographicSize -= zoomAccuracy;
                //vcam m_Lens ortho
                virtualCamera.m_Lens.OrthographicSize -= zoomAccuracy;
            }
        }

        void CentreCameraOn(Transform objectToCentre)
        {
            // this centres the camera on a specific transform's location
            //Resolution Magic uses this to centre on the canvas before zooming the camera
            Vector3 newCameraPos = new Vector3(objectToCentre.position.x, objectToCentre.position.y, Camera.main.transform.position.z);
            Camera.main.transform.position = newCameraPos;
        }


        void ToggleBlackBars(bool isActive)
        {
            var barsPrefab = GameObject.FindGameObjectWithTag("RM_Black_Bars");
            if (barsPrefab != null)
            {
                if (isActive)
                    barsPrefab.GetComponent<BlackBars>().Enabled = true;
                else
                    barsPrefab.GetComponent<BlackBars>().Enabled = false;
            }
            else
            {
                Debug.Log("Resolution Magic warning: trying to toggle black bars, but black bars prefab not found.");
            }

        }
        #endregion

        #region PROPERTIES


        /// PROPERTIES


        // the EDGE properties return the furthest point on the relevant edge, e.g. the CameraLeftEdge is the leftmost position the camera can see
        // and CanvasTopEdge is the topmost point on the canvas (which will not necessarily be the same as the top of the screen)
        // these values are in regular vector space with (0,0) representing the centre of the screen
        public float ScreenLeftEdge
        {
            get
            {
                Vector2 topLeft = new Vector2(0, 1);
                Vector2 topLeftInScreen = Camera.main.ViewportToWorldPoint(topLeft);
                return topLeftInScreen.x;
            }
        }

        public float ScreenRightEdge
        {
            get
            {
                Vector2 edge = new Vector2(1, 0);
                Vector2 edgeVector = Camera.main.ViewportToWorldPoint(edge);
                return edgeVector.x;
            }
        }

        public float ScreenTopEdge
        {
            get
            {
                Vector2 edge = new Vector2(1, 1);
                Vector2 edgeVector = Camera.main.ViewportToWorldPoint(edge);
                return edgeVector.y;
            }
        }

        public float ScreenBottomEdge
        {
            get
            {
                Vector2 edge = new Vector2(1, 0);
                Vector2 edgeVector = Camera.main.ViewportToWorldPoint(edge);
                return edgeVector.y;
            }
        }

        public Vector2 ScreenTopLeft
        {
            get
            {
                return new Vector2(ScreenLeftEdge, ScreenTopEdge);
            }
        }

        public Vector2 ScreenTopRight
        {
            get
            {
                return new Vector2(ScreenRightEdge, ScreenTopEdge);
            }
        }

        public Vector2 ScreenBottomLeft
        {
            get
            {
                return new Vector2(ScreenLeftEdge, ScreenBottomEdge);
            }
        }

        public Vector2 ScreenBottomRight
        {
            get
            {
                return new Vector2(ScreenRightEdge, ScreenBottomEdge);
            }
        }

        public float CanvasLeftEdge
        {
            get
            {
                return Canvas.position.x - (CanvasWidth / 2);
            }
        }

        public float CanvasRightEdge
        {
            get
            {
                return Canvas.position.x + (CanvasWidth / 2);
            }
        }

        public float CanvasTopEdge
        {
            get
            {
                return Canvas.position.y + (CanvasHeight / 2);
            }
        }

        public float CanvasBottomEdge
        {
            get
            {
                return Canvas.position.y - (CanvasHeight / 2);
            }
        }


        public float CanvasWidth
        {
            get
            {
                if (getSizeFromCollider)
                {
                    return collider.bounds.size.x;
                }
                else
                {
                    return renderer.bounds.size.x;
                }
            }
        }

        public float CanvasHeight
        {
            get
            {
                if (getSizeFromCollider)
                {
                    return collider.bounds.size.y;
                }
                else
                {
                    return renderer.bounds.size.y;
                }
            }
        }

        public float ScreenWidth
        {
            get
            {
                Vector2 topRightCorner = new Vector2(ScreenRightEdge, 0);
                float width = topRightCorner.x * 2;
                return width;
            }
        }

        public float ScreenHeight
        {
            get
            {

                Vector2 topRightCorner = new Vector2(0, ScreenTopEdge);
                float height = topRightCorner.y * 2;
                return height;
            }
        }

        public float MaxLeftEdge
        {
            get
            {
                return MaxArea.position.x - (GetMaxWidth / 2);
            }
        }

        public float MaxRightEdge
        {
            get
            {
                return MaxArea.position.x + (GetMaxWidth / 2);
            }
        }

        public float MaxTopEdge
        {
            get
            {
                return MaxArea.position.y + (GetMaxHeight / 2);
            }
        }

        public float MaxBottomEdge
        {
            get
            {
                return MaxArea.position.y - (GetMaxHeight / 2);
            }
        }

        private float _scaleFactor = 0;
        public float ScaleFactor
        {
            get
            {

                if (_scaleFactor == 0)
                {
                    float ratio;
                    float canvasArea;
                    float screenArea;

                    // canvasArea = Canvas.renderer.bounds.size.x * Canvas.renderer.bounds.size.y;
                    canvasArea = canvasHeight * canvasWidth;

                    float screenX = ScreenRightEdge * 2; // double the distance from the centre to the screen edge
                    float screenY = ScreenTopEdge * 2; // double the distance from the centre to the screen edge
                    screenArea = screenX * screenY;
                    ratio = screenArea / canvasArea;
                    _scaleFactor = Mathf.Sqrt(ratio);
                    return _scaleFactor;
                }
                return _scaleFactor;
            }
        }

        public Vector2 ScreenEdgeAsVector(AlignPoints AlignedEdge)
        {

            switch (AlignedEdge)
            {

                case ResolutionManager.AlignPoints.Centre:
                    return Vector2.zero;

                case ResolutionManager.AlignPoints.Left:
                    return new Vector2(ScreenLeftEdge, ScreenCentre.y);


                case ResolutionManager.AlignPoints.Bottom:
                    return new Vector2(ScreenCentre.x, ScreenBottomEdge);


                case ResolutionManager.AlignPoints.Right:
                    return new Vector2(ScreenRightEdge, ScreenCentre.y);

                case ResolutionManager.AlignPoints.Top:
                    return new Vector2(ScreenCentre.x, ScreenTopEdge);

                case ResolutionManager.AlignPoints.TopLeftCorner:
                    return new Vector2(ScreenLeftEdge, ScreenTopEdge);


                case ResolutionManager.AlignPoints.TopRightCorner:
                    return new Vector2(ScreenRightEdge, ScreenTopEdge);


                case ResolutionManager.AlignPoints.BottomLeftCorner:
                    return new Vector2(ScreenLeftEdge, ScreenBottomEdge);


                case ResolutionManager.AlignPoints.BottomRightCorner:
                    return new Vector2(ScreenRightEdge, ScreenBottomEdge);


            }
            return new Vector2(0, 0);


        }
        public Vector2 ScreenEdgeAsVector(Edges AlignedEdge)
        {

            switch (AlignedEdge)
            {

                case ResolutionManager.Edges.Left:
                    return new Vector2(ScreenLeftEdge, 0);


                case ResolutionManager.Edges.Bottom:
                    return new Vector2(0, ScreenBottomEdge);


                case ResolutionManager.Edges.Right:
                    return new Vector2(ScreenRightEdge, 0);

                case ResolutionManager.Edges.Top:
                    return new Vector2(0, ScreenTopEdge);


            }
            return new Vector2(0, 0);
        }

        public Vector2 ScreenCentre
        {
            // returns the x-coordinate that is the centre of the screen on the x axis regardless of where the camera is
            get
            {
                Vector2 zeroZero = new Vector2(0.5f, 0.5f);

                Vector2 zeroZeroToWorld = Camera.main.ViewportToWorldPoint(zeroZero);


                return zeroZeroToWorld;
            }

        }
        public Vector2 CanvasEdgeAsVector(AlignPoints AlignedEdge)
        {

            switch (AlignedEdge)
            {
                case ResolutionManager.AlignPoints.Left:
                    return new Vector2(CanvasLeftEdge, 0);


                case ResolutionManager.AlignPoints.Bottom:
                    return new Vector2(0, CanvasBottomEdge);


                case ResolutionManager.AlignPoints.Right:
                    return new Vector2(CanvasRightEdge, 0);

                case ResolutionManager.AlignPoints.Top:
                    return new Vector2(0, CanvasTopEdge);

                case ResolutionManager.AlignPoints.TopLeftCorner:
                    return new Vector2(CanvasLeftEdge, CanvasTopEdge);


                case ResolutionManager.AlignPoints.TopRightCorner:
                    return new Vector2(CanvasRightEdge, CanvasTopEdge);


                case ResolutionManager.AlignPoints.BottomLeftCorner:
                    return new Vector2(CanvasLeftEdge, CanvasBottomEdge);


                case ResolutionManager.AlignPoints.BottomRightCorner:
                    return new Vector2(CanvasRightEdge, CanvasBottomEdge);


            }
            return new Vector2(0, 0);// myTransform.position;


        }

        public Vector2 CanvasEdgeAsVector(Edges AlignedEdge)
        {
            // overload to only allow edges (i.e. not corners) for placing the border around the screen/canvas
            switch (AlignedEdge)
            {
                case ResolutionManager.Edges.Left:
                    return new Vector2(CanvasLeftEdge, 0);


                case ResolutionManager.Edges.Bottom:
                    return new Vector2(0, CanvasBottomEdge);


                case ResolutionManager.Edges.Right:
                    return new Vector2(CanvasRightEdge, 0);

                case ResolutionManager.Edges.Top:
                    return new Vector2(0, CanvasTopEdge);

            }
            return new Vector2(0, 0);// myTransform.position;


        }

        public string ScreenAspectRatio()
        {

            // Resolution Magic doesn't use this, but you can use it to get the current screen ratio
            // you can add/remove resolutions to suit your project

            // needs to be fleshed out
            float ratio;
            string orientation;
            if (Screen.width > Screen.height)
            {
                ratio = (float)Screen.width / Screen.height;
                orientation = "landscape";
            }
            else
            {
                ratio = (float)Screen.height / Screen.width;
                orientation = "portrait";
            }

            // NOTE: because screen sizes can vary slightly
            // we need to use fuzzy logic to get the closest match rather than checking for the exact ratio
            // e.g. a screen might have an actual ratio of 1.59999 instead of 1.6

            if (ratio < 1.38f)
                return "4x3" + orientation;

            if (ratio < 1.59f)
                return "3x2" + orientation;

            if (ratio < 1.63f)
                return "16x10" + orientation;

            if (ratio < 1.7f)
                return "5x3" + orientation;

            if (ratio < 1.82f)
                return "16x9" + orientation;

            return "not_detected"; //fallback for very narrow or weird screens

        }
        # endregion

        #region ENUMS

        public enum ZoomTypes
        {
            Canvas,
            MaxSize

        };

        public enum AlignPoints
        {
            Centre,
            Top,
            Bottom,
            Left,
            Right,
            TopLeftCorner,
            TopRightCorner,
            BottomLeftCorner,
            BottomRightCorner

        }

        public enum CameraDirections
        {
            Up,
            Down,
            Left,
            Right

        }
        public enum Edges
        {
            Left,
            Top,
            Right,
            Bottom

        }

        public enum AlignObjects
        {
            Screen,
            Canvas
        }

        #endregion

    }
}

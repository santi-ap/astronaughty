using UnityEngine;
using System.Collections;
using ResolutionMagic;

public class BlackBars : MonoBehaviour
{

    [SerializeField] private GameObject leftBar;
    [SerializeField]
    private GameObject rightBar;
    [SerializeField]
    private GameObject topBar;
    [SerializeField]
    private GameObject bottomBar;
    
    Transform myTransform; // reference to the transform
    [SerializeField]
    bool _enabled = true;

    public bool Enabled
    {
        get { return _enabled; }
        set
        {
            _enabled = value;
            if(value)
                EnableBlackBars();
            else
            {
                DisableBlackBars();
            }
        }
    }
	// Use this for initialization
	void Start ()
    {
	    myTransform = transform;
        if(Enabled)
            EnableBlackBars();
    }

    
    void EnableBlackBars()
    {
        Camera.main.transform.position = new Vector3(GameObject.FindGameObjectWithTag("RM_Canvas").transform.position.x, GameObject.FindGameObjectWithTag("RM_Canvas").transform.position.y, Camera.main.transform.position.z);
        ToggleBarSprites(true);
        ScaleBars();
        PlaceBars();
    }


    void DisableBlackBars()
    {
        ToggleBarSprites(false);
    }

    void ToggleBarSprites(bool isOn)
    {
        leftBar.GetComponent<SpriteRenderer>().enabled = isOn;
        rightBar.GetComponent<SpriteRenderer>().enabled = isOn;
        topBar.GetComponent<SpriteRenderer>().enabled = isOn;
        bottomBar.GetComponent<SpriteRenderer>().enabled = isOn;
    }

    void SetBarSize(Transform bar, bool isVertical)
    {
         float mySize = 0;
         bar.localScale = new Vector3(1, 1, 0);
         mySize = bar.GetComponent<Renderer>().bounds.size.x;

        float scalex = 30f;
        float scaley = 30f;
        if (isVertical)
        {
            scalex = (ResolutionManager.Instance.CanvasWidth/mySize) * 1.5f;
        }
        else
        {
             scaley = (ResolutionManager.Instance.CanvasHeight/mySize) * 1.5f;
        }
        //else
                 //    scale = ResolutionManager.Instance.ScreenWidth / mySize;

         bar.localScale = new Vector3(scalex, scaley , 0);
         

         
    }

    void SetBarPosition(Transform bar, ResolutionManager.AlignPoints alignment)
    {
        Vector2 adjustment = new Vector2(0,0);
        switch (alignment)
        {
            case ResolutionManager.AlignPoints.Bottom:
                adjustment.y -= (bar.GetComponent<Renderer>().bounds.size.y/2);
                break;
            case ResolutionManager.AlignPoints.Top:
                adjustment.y += (bar.GetComponent<Renderer>().bounds.size.y/2);
                break;
            case ResolutionManager.AlignPoints.Left:
                adjustment.x -= (bar.GetComponent<Renderer>().bounds.size.x/2);
                break;
            case ResolutionManager.AlignPoints.Right:
                adjustment.x += (bar.GetComponent<Renderer>().bounds.size.x/2);
                break;
     
        }

        bar.position = ResolutionManager.Instance.CanvasEdgeAsVector(alignment) + adjustment;

    }

    void ScaleBars()
    {
        SetBarSize(leftBar.transform, false);
        SetBarSize(rightBar.transform, false);
        SetBarSize(topBar.transform, true);
        SetBarSize(bottomBar.transform, true);
    }

    void PlaceBars()
    {
        SetBarPosition(leftBar.transform, ResolutionManager.AlignPoints.Left);
        SetBarPosition(rightBar.transform, ResolutionManager.AlignPoints.Right);
        SetBarPosition(topBar.transform, ResolutionManager.AlignPoints.Top);
        SetBarPosition(bottomBar.transform, ResolutionManager.AlignPoints.Bottom);
    }

}

	
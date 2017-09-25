using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PupilCalibMarker : MonoBehaviour {

    public ExperimentState state;
	RectTransform _transform;
	Image _image;
	bool _started=false;
    public bool _finished = false;
	float x,y;

	// Use this for initialization
	void Start () {
		_transform = GetComponent<RectTransform> ();
		_image = GetComponent<Image> ();
		_image.enabled = false;

		PupilGazeTracker.Instance.OnCalibrationStarted += OnCalibrationStarted;
		PupilGazeTracker.Instance.OnCalibrationDone += OnCalibrationDone;
		PupilGazeTracker.Instance.OnCalibrationData += OnCalibrationData;
	}

	void OnCalibrationStarted(PupilGazeTracker m)
	{
		_started = true;
	}

	void OnCalibrationDone(PupilGazeTracker m)
	{
		_started = false;
        _finished = true;
	}

	void OnCalibrationData(PupilGazeTracker m,float x,float y)
	{
		this.x = x;
		this.y = y;
	}

	void _SetLocation(float x,float y)
	{
		Canvas c = _transform.GetComponentInParent<Canvas> ();
		if (c == null) return;

        _transform.localPosition = new Vector3((x - 0.5f) * c.pixelRect.width, (y - 0.5f) * c.pixelRect.height, 0);
	}
	
    public void startCalibrate()
    {
        PupilGazeTracker.Instance.StartCalibration();
    }
    public void stopCalibrate()
    {
        PupilGazeTracker.Instance.StopCalibration();
    }


	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.H))
            PupilGazeTracker.Instance.StartCalibration();
        //if (Input.GetKeyDown (KeyCode.J))
        //	PupilGazeTracker.Instance.StopCalibration ();

        _image.enabled = _started;

        if (_started)
			_SetLocation (x, y);
	}
}

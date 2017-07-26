
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class EyeGazeRenderer : MonoBehaviour
{
	public RectTransform gaze;
	public PupilGazeTracker.GazeSource Gaze;
    Canvas c;
    Vector2 g;

    void Start()
    {
		if (gaze == null) gaze = this.GetComponent<RectTransform>();
	}

	void Update()
    {
		if (gaze == null) return;

        if(PupilGazeTracker.Instance.getStatus() == PupilGazeTracker.EStatus.Calibration)
        {
            gaze.localScale = Vector3.zero;
            PupilGazeTracker.Instance.DisplayCalibrationStatus();
        }
        else
        {
            c = gaze.GetComponentInParent<Canvas>();
            g = PupilGazeTracker.Instance.GetEyeGaze(Gaze);
            gaze.localPosition = new Vector3((g.x - 0.5f) * c.pixelRect.width, (g.y - 0.5f) * c.pixelRect.height, c.planeDistance);

            if
            (
                (PupilGazeTracker.Instance.getGazeDisplayMode() == PupilGazeTracker.GazeDisplayMode.All)
                || (PupilGazeTracker.Instance.getGazeDisplayMode() == PupilGazeTracker.GazeDisplayMode.LeftEye && Gaze == PupilGazeTracker.GazeSource.LeftEye)
                || (PupilGazeTracker.Instance.getGazeDisplayMode() == PupilGazeTracker.GazeDisplayMode.RightEye && Gaze == PupilGazeTracker.GazeSource.RightEye)
                || (PupilGazeTracker.Instance.getGazeDisplayMode() == PupilGazeTracker.GazeDisplayMode.CenterEye && Gaze == PupilGazeTracker.GazeSource.CenterEye)
            )
            {
                gaze.localScale = new Vector3(0.35f, 0.35f, 0.35f);
            }
            else
            {
                gaze.localScale = Vector3.zero;
            }

            if ((PupilGazeTracker.Instance.getGazeDisplayMode() == PupilGazeTracker.GazeDisplayMode.All) || (PupilGazeTracker.Instance.getGazeDisplayMode() == PupilGazeTracker.GazeDisplayMode.None))
            {
                if (Gaze == PupilGazeTracker.GazeSource.CenterEye)
                {
                    PupilGazeTracker.Instance.checkEyeTrackingPoint(transform.position);
                }
            }
            else
            {
                if
                (
                    (PupilGazeTracker.Instance.getGazeDisplayMode() == PupilGazeTracker.GazeDisplayMode.LeftEye && Gaze == PupilGazeTracker.GazeSource.LeftEye)
                    || (PupilGazeTracker.Instance.getGazeDisplayMode() == PupilGazeTracker.GazeDisplayMode.RightEye && Gaze == PupilGazeTracker.GazeSource.RightEye)
                    || (PupilGazeTracker.Instance.getGazeDisplayMode() == PupilGazeTracker.GazeDisplayMode.CenterEye && Gaze == PupilGazeTracker.GazeSource.CenterEye)
                )
                {
                    PupilGazeTracker.Instance.checkEyeTrackingPoint(transform.position);
                }
            }
        }
    }
}
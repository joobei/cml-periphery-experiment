using System.Runtime.InteropServices;
using UnityEngine;

public class Calibration : MonoBehaviour {

    [DllImport("Touchy")]
    public static extern void getEEPosition(double[] position);

    [DllImport("Touchy")]
    public static extern void disableSphere();

    [DllImport("Touchy")]
    public static extern void enableSphere();

    [DllImport("Touchy")]
    public static extern void stopCenterCallback();

    [DllImport("Touchy")]
    public static extern void addSphere(double radius, double x, double y, double z);

    public Transform optiCursor;

    public Transform hapticTarget;

    public Vector3 optiPosZero;
    private bool isCalibrationDone = false;

	void Update () {
        double[] tempPosition = new double[3];
        getEEPosition(tempPosition);
        Vector3 tempVector;
        tempVector.x = (float)tempPosition[0] / 1000; //convert from mm to m
        tempVector.y = (float)tempPosition[1] / 1000;
        tempVector.z = (float)-tempPosition[2] / 1000;

        if (Input.GetKeyDown(KeyCode.Space) && isCalibrationDone)
        {
            addSphere(30, 0, 0, 0);
        }

        if (!isCalibrationDone)
        {
            disableSphere();
            Debug.Log(tempVector.magnitude);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                optiPosZero = optiCursor.position;
                hapticTarget.position += optiPosZero;
                enableSphere();
                isCalibrationDone = true;
                stopCenterCallback();
            }
        }
    }
}

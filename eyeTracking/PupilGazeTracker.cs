// Pupil Gaze Tracker service
// Written by MHD Yamen Saraiji
// https://github.com/mrayy

using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using NetMQ;
using NetMQ.Sockets;
using System;
using System.IO;
using MsgPack.Serialization;

namespace Pupil
{
	//Pupil data types based on Yuta Itoh sample hosted in https://github.com/pupil-labs/hmd-eyes
	[Serializable]
	public class ProjectedSphere
	{
		public double[] axes = new double[] {0,0};
		public double angle;
		public double[] center = new double[] {0,0};
	}
	[Serializable]
	public class Sphere
	{
		public double radius;
		public double[] center = new double[] {0,0,0};
	}
	[Serializable]
	public class Circle3d
	{
		public double radius;
		public double[] center = new double[] {0,0,0};
		public double[] normal = new double[] {0,0,0};
	}
	[Serializable]
	public class Ellipse
	{
		public double[] axes = new double[] {0,0};
		public double angle;
		public double[] center = new double[] {0,0};
	}
	[Serializable]
	public class PupilData3D
	{
		public double diameter;
		public double confidence;
		public ProjectedSphere projected_sphere = new ProjectedSphere();
		public double theta;
		public int model_id;
		public double timestamp;
		public double model_confidence;
		public string method;
		public double phi;
		public Sphere sphere = new Sphere();
		public double diameter_3d;
		public double[] norm_pos = new double[] { 0, 0, 0 };
		public int id;
		public double model_birth_timestamp;
		public Circle3d circle_3d = new Circle3d();
		public Ellipse ellipese = new Ellipse();
	}
}

public class PupilGazeTracker : MonoBehaviour
{

    static PupilGazeTracker _Instance;
	public static PupilGazeTracker Instance
	{
		get
        {
			if (_Instance == null)
            {
				_Instance = new GameObject("PupilGazeTracker").AddComponent<PupilGazeTracker> ();
			}
			return _Instance;
		}
	}

	class MovingAverage
	{
		List<float> samples=new List<float>();
		int length;
        float s;

        public MovingAverage(int len)
		{
			length=len;
		}

        public float AddSample(float v)
		{
			samples.Add (v);
			while (samples.Count > length)
            {
				samples.RemoveAt (0);
			}

            s = 0;
			for (int i = 0; i < samples.Count; ++i)
				s += samples [i];

			return s / (float)samples.Count;
		}
	}

	class EyeData
	{
		MovingAverage xavg;
		MovingAverage yavg;

		public EyeData(int samplesCount)
		{
			 xavg=new MovingAverage(samplesCount);
			 yavg=new MovingAverage(samplesCount);
		}
		public Vector2 gaze=new Vector2();
		public Vector2 AddGaze(float x,float y)
		{
			gaze.x = xavg.AddSample (x);
			gaze.y = yavg.AddSample (y);
			return gaze;
		}
	}

    public enum EStatus
    {
        Idle,
        ProcessingGaze,
        Calibration
    }

    public enum GazeSource
    {
        LeftEye,
        RightEye,
        CenterEye
    }

    public enum GazeDisplayMode
    {
        LeftEye,
        RightEye,
        CenterEye,
        All,
        None
    }
    
    EyeData leftEye;
    EyeData rightEye;
    public Vector2 centerEye;
    public Vector2 fixationPoint;

    public delegate void OnCalibrationStartedDeleg(PupilGazeTracker manager);
	public delegate void OnCalibrationDoneDeleg(PupilGazeTracker manager);
	public delegate void OnCalibrationDataDeleg(PupilGazeTracker manager,float x,float y);

    public event OnCalibrationStartedDeleg OnCalibrationStarted;
	public event OnCalibrationDoneDeleg OnCalibrationDone;
	public event OnCalibrationDataDeleg OnCalibrationData;

    Thread _serviceThread;
    object _dataLock;
    int _gazeFPS = 0;
    int _currentFps = 0;
    DateTime _lastT, ct;
    bool _isDone = false;
    bool _isconnected =false;
	RequestSocket _requestSocket;
    Pupil.PupilData3D _pupilData;
    int _currCalibPoint=0;
	int _currCalibSamples=0;
    List<Dictionary<string, object>> _calibrationData = new List<Dictionary<string, object>>();
    Vector2[] _calibPoints = new Vector2[] { //calibrate using default 9 points
            new Vector2 (0.5f, 0.5f), new Vector2 (0.4f, 0.4f), new Vector2 (0.4f, 0.5f),
            new Vector2 (0.4f, 0.6f), new Vector2 (0.5f, 0.6f), new Vector2 (0.6f, 0.6f),
            new Vector2 (0.6f, 0.5f), new Vector2 (0.6f, 0.4f), new Vector2 (0.5f, 0.4f)};
    EStatus m_status = EStatus.Idle;

    public string ServerIP="";
	public int ServicePort=50020;
	public int DefaultCalibrationCount=120; // Number of samples per calibration target
    public int GazeSmoothingSamplesCount=5;
    public float TrackingConfidenceThreshold=0.5f;

    public GazeDisplayMode gazeDisplayMode = GazeDisplayMode.CenterEye;

    public void calibrateFixation()
    {
        fixationPoint = centerEye;
    }

    public GazeDisplayMode getGazeDisplayMode()
    {
        return gazeDisplayMode;
    }

    public bool checkEyeTrackingThreshold(float threshold)
    {
        float distanceToCenter = Vector2.Distance(centerEye, fixationPoint);
        return (distanceToCenter <= threshold);
    }

    public Vector2 GetEyeGaze(GazeSource s)
	{
		if (s == GazeSource.RightEye)
			return rightEye.gaze;
		else if (s == GazeSource.LeftEye)
			return leftEye.gaze;
		else
            return centerEye;
	}

	public PupilGazeTracker()
	{
		_Instance = this;
	}

    LineRenderer lineRenderer1;

    void Start()
	{
        lineRenderer1 = gameObject.AddComponent<LineRenderer>();
        lineRenderer1.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer1.widthMultiplier = 0.02f;
        lineRenderer1.positionCount = 2;
        lineRenderer1.startColor = Color.yellow;
        lineRenderer1.endColor = Color.red;

        if (PupilGazeTracker._Instance == null) PupilGazeTracker._Instance = this;

        leftEye = new EyeData (GazeSmoothingSamplesCount);
		rightEye= new EyeData (GazeSmoothingSamplesCount);

		_dataLock = new object ();
		_serviceThread = new Thread(NetMQClient);
		_serviceThread.Start();

	}
	void OnDestroy()
	{
		if (m_status == EStatus.Calibration) StopCalibration ();
		_isDone = true;
		_serviceThread.Join();
	}

	void NetMQClient()
	{
		//thanks for Yuta Itoh sample code to connect via NetMQ with Pupil Service
		string IPHeader = ">tcp://" + ServerIP + ":";
		var timeout = new System.TimeSpan(0, 0, 1); // 1 sec

		// Necessary to handle this NetMQ issue on Unity editor
		// https://github.com/zeromq/netmq/issues/526
		AsyncIO.ForceDotNet.Force();
		NetMQConfig.ManualTerminationTakeOver();
		NetMQConfig.ContextCreate(true);

		Debug.Log("Connect to the server: "+ IPHeader + ServicePort + ".");
		_requestSocket = new RequestSocket(IPHeader + ServicePort);

        string subport = "";
        _requestSocket.SendFrame("SUB_PORT");
		_isconnected = _requestSocket.TryReceiveFrameString(timeout, out subport);

		_lastT = DateTime.Now;

		if (_isconnected)
		{
			StartProcess ();
			var subscriberSocket = new SubscriberSocket( IPHeader + subport);
			subscriberSocket.Subscribe("gaze"); //subscribe for gaze data
			subscriberSocket.Subscribe("notify."); //subscribe for all notifications
			_setStatus(EStatus.ProcessingGaze);
			var msg = new NetMQMessage();
			while ( _isDone == false)
			{
				_isconnected = subscriberSocket.TryReceiveMultipartMessage(timeout,ref(msg));
				if (_isconnected)
				{
					try
					{
						string msgType=msg[0].ConvertToString();
						//Debug.Log(msgType);
						if(msgType=="gaze")
						{
							var message = MsgPack.Unpacking.UnpackObject(msg[1].ToByteArray());
							MsgPack.MessagePackObject mmap = message.Value;
							lock (_dataLock)
							{
								_pupilData = JsonUtility.FromJson<Pupil.PupilData3D>(mmap.ToString());
								if(_pupilData.confidence>TrackingConfidenceThreshold)
								{
									OnPacket(_pupilData);
								}
							}
						}
						//Debug.Log(message);
					}
					catch
					{
					//	Debug.Log("Failed to unpack.");
					}
				}
				else
				{
					Debug.LogWarning("Failed to receive a message.");
					Thread.Sleep(50);
				}
			}

			StopProcess ();

			subscriberSocket.Close();
		}
		else
		{
			Debug.Log("Failed to connect the server.");
		}

		_requestSocket.Close();
		// Necessary to handle this NetMQ issue on Unity editor
		// https://github.com/zeromq/netmq/issues/526
		Debug.Log("ContextTerminate.");
		NetMQConfig.ContextTerminate();
	}

	void _setStatus(EStatus st)
	{
		if(st==EStatus.Calibration)
		{
			_calibrationData.Clear ();
			_currCalibPoint = 0;
			_currCalibSamples = 0;
		}

		m_status = st;
	}

    public EStatus getStatus()
    {
        return m_status;
    }

    NetMQMessage _sendRequestMessage(Dictionary<string, object> data)
    {
        NetMQMessage m = new NetMQMessage();
        m.Append("notify." + data["subject"]);

        using (var byteStream = new MemoryStream())
        {
            var ctx = new SerializationContext();
            ctx.CompatibilityOptions.PackerCompatibilityOptions = MsgPack.PackerCompatibilityOptions.None;
            var ser = MessagePackSerializer.Get<object>(ctx);
            ser.Pack(byteStream, data);
            m.Append(byteStream.ToArray());
        }

        _requestSocket.SendMultipartMessage(m);

        NetMQMessage recievedMsg;
        recievedMsg = _requestSocket.ReceiveMultipartMessage();

        return recievedMsg;
    }

    public void StartProcess()
	{
		_sendRequestMessage (new Dictionary<string,object> {{"subject","eye_process.should_start.0"},{"eye_id",0}});
		_sendRequestMessage ( new Dictionary<string,object> {{"subject","eye_process.should_start.1"},{"eye_id",1}});
	}
	public void StopProcess()
	{
		_sendRequestMessage ( new Dictionary<string,object> {{"subject","eye_process.should_stop"},{"eye_id",0}});
		_sendRequestMessage ( new Dictionary<string,object> {{"subject","eye_process.should_stop"},{"eye_id",1}});
	}

	public void StartCalibration()
	{
        StartCalibration (_calibPoints, DefaultCalibrationCount);
	}
	private void StartCalibration(Vector2[] calibPoints,int samples)
	{
		_sendRequestMessage ( new Dictionary<string,object> {{"subject","start_plugin"},{"name","HMD_Calibration"}});
		_sendRequestMessage ( new Dictionary<string,object> {{"subject","calibration.should_start"},{"hmd_video_frame_size",new float[]{1000,1000}},{"outlier_threshold",35}});
		_setStatus (EStatus.Calibration);

		if (OnCalibrationStarted != null) OnCalibrationStarted (this);
	}

    public void StopCalibration()
	{
        _sendRequestMessage ( new Dictionary<string,object> {{"subject","calibration.should_stop"}});
		if (OnCalibrationDone != null) OnCalibrationDone(this);
		_setStatus (EStatus.ProcessingGaze);
        gazeDisplayMode = GazeDisplayMode.CenterEye;
        
    }

	void OnPacket(Pupil.PupilData3D data)
	{
        float x, y;
        _gazeFPS++;
		ct=DateTime.Now;
		if((ct-_lastT).TotalSeconds>1)
		{
			_lastT=ct;
			_currentFps=_gazeFPS;
			_gazeFPS=0;
		}

		if (m_status == EStatus.ProcessingGaze) //gaze processing stage
        {
			x = (float)data.norm_pos [0];
			y = (float)data.norm_pos [1];

            if (data.id == 0)
            {
				leftEye.AddGaze (x, y);
			}
            else if (data.id == 1)
            {
				rightEye.AddGaze (x, y);
			}

            centerEye.x = (leftEye.gaze.x + rightEye.gaze.x) * 0.5f;
            centerEye.y = (leftEye.gaze.y + rightEye.gaze.y) * 0.5f;
        }
        else if (m_status == EStatus.Calibration) //gaze calibration stage
        {
			// Get Pupil Timestamp
            _requestSocket.SendFrame("t");
            NetMQMessage recievedMsg = _requestSocket.ReceiveMultipartMessage();
            float t = float.Parse(recievedMsg[0].ConvertToString());

            var ref0=new Dictionary<string,object>(){{"norm_pos",new float[]{_calibPoints[_currCalibPoint].x,_calibPoints[_currCalibPoint].y}},{"timestamp",t},{"id",0}};
			var ref1=new Dictionary<string,object>(){{"norm_pos",new float[]{_calibPoints[_currCalibPoint].x,_calibPoints[_currCalibPoint].y}},{"timestamp",t},{"id",1}};

            if (OnCalibrationData != null) OnCalibrationData(this, _calibPoints [_currCalibPoint].x, _calibPoints [_currCalibPoint].y);

			_calibrationData.Add (ref0);
			_calibrationData.Add (ref1);
			_currCalibSamples++;
			Thread.Sleep (1000 / 60);

			if (_currCalibSamples >= DefaultCalibrationCount)
            {
				_currCalibSamples = 0;
				_currCalibPoint++;
                _sendRequestMessage(new Dictionary<string,object> {{"subject","calibration.add_ref_data"},{"ref_data", _calibrationData.ToArray()}});
				_calibrationData.Clear ();
				if (_currCalibPoint >= _calibPoints.Length)
                {
					StopCalibration ();
				}
			}
		}
	}
    
    public UnityEngine.UI.Text legend;
    public bool displayUILog = !false;
    private RaycastHit hit;
    //private String selection;
    public void checkEyeTrackingPoint(Vector3 targetPosition)
    {
        /*
        Vector3 origin = Camera.main.transform.position; origin.y = origin.y - 0.2f;

        //lineRenderer1.SetPosition(0, startPosition);
        //lineRenderer1.SetPosition(1, targetPosition);

        Vector3 direction = (targetPosition - origin);
        Ray ray = new Ray(targetPosition, direction);
        if (Physics.Raycast(ray, out hit))
        {
            lineRenderer1.SetPosition(0, origin);
            lineRenderer1.SetPosition(1, hit.point);
            selection = hit.collider.gameObject.name;
        }
        else
        {*/
            lineRenderer1.SetPosition(0, Vector3.zero);
            lineRenderer1.SetPosition(1, Vector3.zero);
            //selection = String.Empty;
        /*}
        
        if (displayUILog)
        {
            string str = "Capture Rate=" + _currentFps;
            str += "\nSelection:" + selection;
            legend.text = str;
        }
        else
        {
            legend.text = String.Empty;
        }
        */
    }
    public void DisplayCalibrationStatus()
    {
        /*
        if (displayUILog)
        {
            string str = "Capture Rate=" + _currentFps;
            str += "\nRemaining calibration points:" + (DefaultCalibrationCount - _currCalibSamples);
            legend.text = str;
        }
        else
        {
            legend.text = String.Empty;
        }
        */
    }

    void OnGUI()
	{
        if (Input.GetKeyDown(KeyCode.K)) displayUILog = true;
        if (Input.GetKeyDown(KeyCode.L)) displayUILog = false;
        if (Input.GetKeyDown(KeyCode.Alpha1)) gazeDisplayMode = GazeDisplayMode.LeftEye;
        if (Input.GetKeyDown(KeyCode.Alpha2)) gazeDisplayMode = GazeDisplayMode.RightEye;
        if (Input.GetKeyDown(KeyCode.Alpha3)) gazeDisplayMode = GazeDisplayMode.CenterEye;
        if (Input.GetKeyDown(KeyCode.Alpha4)) gazeDisplayMode = GazeDisplayMode.All;
        if (Input.GetKeyDown(KeyCode.Alpha5)) gazeDisplayMode = GazeDisplayMode.None;

        String str = "";
        str += "Capture Rate=" + _currentFps;
        str += "\nLeft Eye (Red):" + leftEye.gaze.ToString();
        str += "\nRight Eye (Blue):" + rightEye.gaze.ToString();
        str += "\nCenter Eye (Green):" + centerEye.ToString();
        str += "\nRemaining calibration points:" + (DefaultCalibrationCount - _currCalibSamples);
		//str += "\nLooking at fixation point?:" + (checkEyeTrackingThreshold()? " YES" : " NO");
        GUI.TextArea (new Rect (0, 0, 200, 6*17), str);
    }
}

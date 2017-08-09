using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExperimentState : MonoBehaviour
{
    AudioSource audioSource;
    protected float timeRepeat = 1f; //to stop sounds from playing too fast - needs fixing
    protected float triggerTime = -1f;

    public ExperimentState nextState;

    protected float armLength;

    protected String stateName;

    //private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private SteamVR_Controller.Device device;
    private SteamVR_TrackedObject trackedObject;

    //objects this state is going to use 
    //and therefore needs to activate (in Activate method)
    public GameObject[] neededObjects;

    protected virtual void Update()
    {
        timeRepeat -= Time.deltaTime;
        var controllerObject = GameObject.Find("RightController");

        //HACK For trigger!!!
        bool triggerClicked = false;
        if (timeRepeat < 0)
        {
            try
            {

                trackedObject = controllerObject.GetComponent<SteamVR_TrackedObject>();
                device = SteamVR_Controller.Input((int)trackedObject.index);
                triggerClicked = device.GetHairTriggerDown();
                //Vector2 triggerPosition = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger);
                //triggerClicked = triggerPosition.x > 0.999f; // trigger treshold seems to be 0.9f
            }
            catch (NullReferenceException e) { Debug.LogWarning(e.Message); }
        }

        if (triggerClicked && triggerTime < 0)
        {
            triggerPressed();
            triggerTime = 1f;
            triggerClicked = false;
        }

        //reset button timeout if trigger button is up
        if (SteamVR_Controller.Input(SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.FarthestRight)).GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            triggerTime = -1;
        }
    }

    protected abstract void triggerPressed();

    //Disable all objects other
    //than the ones the state needs
    public virtual void OnEnable()
    {
        //disable all objects tagged "useful"
        //this is so that we don't disable everything
        //gameobjects holding networking scripts, OpenVR, HMD's etc.
        //"useful" was chosen for lack of a better tag.
        GameObject[] objects = GameObject.FindGameObjectsWithTag("useful");
        foreach (GameObject gameObject in objects)
        {
            gameObject.SetActive(false);
        }

        //enable only Objects needed by current state
        foreach (GameObject gameObject in neededObjects)
        {
            gameObject.SetActive(true);
        }

        audioSource = GetComponent<AudioSource>();
         AudioClip clip = (AudioClip)Resources.Load("Start");
        audioSource.PlayOneShot(clip);
        //Debug.Log("State: " + this.stateName);
    }

    public virtual void advanceState()
    {
        //disable this state (i.e. update will stop being called)
        enabled = false;

        //tried to enable next state in list but did not work because it was not enabled
        //ExperimentState[] states = GetComponents<ExperimentState>(true);
        //for (int i = 0; i < states.Length; i++)
        //{
        //    if (states[i].Equals(this))
        //    {
        //        this.enabled = false;
        //        if (states[i + 1] != null)
        //            states[i + 1].enabled = true;
        //    }
        //    break;
        //}

        //enable the next one
        if (nextState != null)
            nextState.enabled = true;
    }

    //public virtual void regressState()
    //{
    //    enabled = false;
    //    previousState.enabled = true;
    //}

    protected void playSound(String filename)
    {
        AudioClip clip = (AudioClip)Resources.Load(filename);
        if (!audioSource.isPlaying)
        {
            if (timeRepeat < 0)
            {
                audioSource.PlayOneShot(clip);
                timeRepeat = .5f;
            }
        }
    }

}

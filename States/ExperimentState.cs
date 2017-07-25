using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExperimentState : MonoBehaviour
{
    AudioSource audioSource;
    float timeRepeat = -1f; //to stop sounds from playing too fast - needs fixing

    public ExperimentState previousState;
    public ExperimentState nextState;

    protected String stateName;

    //objects this state is going to use 
    //and therefore needs to activate (in Activate method)
    public GameObject[] neededObjects;

    protected virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //a state is not enabled by default
        //the experiment controller sets the starting state
        //to enabled when the program launches.
        enabled = false;
    }

    protected virtual void Update()
    {
        timeRepeat -= Time.deltaTime;
        if (Input.GetMouseButtonDown(1))
        {
            regressState();
        }
    }

    //Disable all objects other
    //than the ones the state needs
    public virtual void Activate()
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

        //enable self
        enabled = true;

        playSound("Start");

        Debug.Log("State: " + this.stateName);
    }

    public virtual void advanceState()
    {
        //disable this state (i.e. update will stop being called)
        enabled = false;

        //enable the next one
        nextState.Activate();
    }

    public virtual void regressState()
    {
        enabled = false;
        previousState.Activate();
    }

    protected void playSound(String filename)
    {
        AudioClip clip = (AudioClip)Resources.Load(filename);
        if (!audioSource.isPlaying)
        {
            if (timeRepeat < 0)
            {
                audioSource.PlayOneShot(clip);
                timeRepeat = 5f;
            }
        }
    }

}

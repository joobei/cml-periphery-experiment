/*  This file is part of Joutai.

    Joutai is free software: you can redistribute it and/or modify
	it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
	(at your option) any later version.

	Joutai is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

	You should have received a copy of the GNU General Public License
	along with Joutai.  If not, see <http://www.gnu.org/licenses/>. 
*/


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Joutai {
    
public abstract class State : MonoBehaviour
{
    //Whether or not this state will be used
    //this is checked by onEnable()
	[HideInInspector]
    public bool use=true;

	//List of objects this state is going to use 
	//and therefore needs to activate (in Activate method)
    public GameObject[] neededObjects;

    //Disable all objects other
    //than the ones the state needs
    public virtual void OnEnable()
    {
       if (use) //toggled through the inspector interface
        {
            //disable all objects except those tagged "permanent"
            //this is so that we don't disable 
            //gameobjects holding networking scripts, OpenVR, HMD's etc.
            List<GameObject> permanentObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("permanent"));

            GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject go in rootObjects)
            {
                //if it's not in the permanent objects array
                //disable it
                //TODO: check what happens with MainCamera in VR
                if(!(permanentObjects.Contains(go) || go.name.Contains("Camera") || go == gameObject))
                {
                    go.SetActive(false);    
                }

            }

            //enable only Objects needed by current state
            SetActiveNeededObjects(true);
        }
        else
        {
            AdvanceState();
        }
    }

    public void SetActiveNeededObjects(bool value)
    {
        foreach (GameObject gO in neededObjects)
        {
            gO.SetActive(value);
            foreach (Transform child in gO.GetComponentsInChildren<Transform>())
            {
                child.gameObject.SetActive(value);
            }
        }
    }


    //This function should be called whenever you want to move to the next state
    public virtual void AdvanceState()
    {
        //disable this state
        this.enabled = false;

		State[] states = GetComponentsInParent<State>(true);

		for (int i = 0; i < states.Length; i++)
		{
		    if (states[i].Equals(this))
		    {

                //if there IS a next state, enable it.
                if (i+1 < states.Length)
                {
                    states[i + 1].enabled = true;    
					break;
                }
		    }
		    
		}
	}

    //This function should be called whenever you want to move to the next state
    public virtual void RegressState()
    {
        State[] states = GetComponentsInParent<State>(true);

        for (int i = 0; i < states.Length; i++)
        {
            if (states[i].Equals(this))
            {
                //disable this state
                this.enabled = false;
                //if there IS a previous state, enable it.
                if (i - 1 >= 0)
                {
                    states[i - 1].enabled = true;
                    break;
                }
            }

        }
    }

}
}
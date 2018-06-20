using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonInteract : MonoBehaviour {

    public enum Decision
    {
        Smaller, Bigger
    }

    //Event for ui button
    public delegate void DecisionAction(Decision decision);
    public static event DecisionAction OnUIButton;

    private EventSystem myEventSystem;

    private void OnEnable()
    {
        HapticManager.OnStylusButton += OnStylusButton; //Subscribe
        if (myEventSystem == null)
            myEventSystem = GameObject.Find("EventSystem").GetComponent<UnityEngine.EventSystems.EventSystem>();
    }

    private void OnDisable()
    {
        HapticManager.OnStylusButton -= OnStylusButton; //Unsubscribe
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "uiCursor")
            GetComponent<Button>().Select();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "uiCursor")
            myEventSystem.SetSelectedGameObject(null);
    }

    private void OnStylusButton()
    {
        if (myEventSystem.currentSelectedGameObject == gameObject)
        {
            string text = GetComponentInChildren<Text>().text;
            Debug.Log(text);
            if (text == "Felt\nsmaller")
            {
                OnUIButton(Decision.Smaller);
            }
            else if (text == "Felt\nbigger")
            {
                OnUIButton(Decision.Bigger);
            }
        }

        //ExecuteEvents.Execute<IPointerClickHandler>(gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
        //GetComponent<Button>().animationTriggers
    }
}

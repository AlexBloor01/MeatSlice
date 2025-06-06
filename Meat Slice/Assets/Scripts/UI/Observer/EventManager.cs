using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    public delegate void ExampleDelegate(); //Defines the delegate. Remember capitals as it is a function type.
    public ExampleDelegate exampleDelegate; //Instance of the delegate.

    void OnEnable()
    {
        //Subscribing to delegate.
        exampleDelegate += MyFunciton;
        exampleDelegate += MyOtherFunciton;
    }

    void OnDisable()
    {
        //Unsubscribing to delegate.
        exampleDelegate -= MyFunciton;
        exampleDelegate -= MyOtherFunciton;
    }

    void MyFunciton()
    {
        Debug.Log("Annoying Right?");
    }

    void MyOtherFunciton()
    {
        Debug.Log("Super Annoying.");
    }
}

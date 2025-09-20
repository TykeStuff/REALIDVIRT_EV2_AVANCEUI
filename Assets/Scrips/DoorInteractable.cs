using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DoorInteractable : XRGrabInteractable
{

    [SerializeField] Transform doorTransform;
    [SerializeField] XRSocketInteractor keySocket;
    [SerializeField] bool isLocked;
    [SerializeField] HingeJoint hinge;
    private Transform parentTransform;
    private const string defaultLayer = "Default";
    private const string grabLayer = "Grab";
    private bool isGrabbed;


    void Start()
    {
        if (keySocket != null)
        {
            keySocket.selectEntered.AddListener(OnDoorUnlocked);
            keySocket.selectExited.AddListener(OnDoorLocked);
        }

        parentTransform = transform.parent.transform;
    }

    private void OnDoorLocked(SelectExitEventArgs arg0)
    {
        isLocked = true;
        Debug.Log("Door Locked");
    }

    private void OnDoorUnlocked(SelectEnterEventArgs arg0)
    {
        isLocked = false;
        Debug.Log("Door Unlocked");
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if (!isLocked)
        {
            transform.SetParent(parentTransform);
            isGrabbed = true;
        }
        else
        {
            interactionLayers = InteractionLayerMask.GetMask(defaultLayer);
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        interactionLayers = InteractionLayerMask.GetMask(grabLayer);
        isGrabbed = false;
    }

    private void UpdateLockState()
    {
        if (hinge != null)
        {
            if (isLocked)
            {
                // La puerta no se mueve
                JointLimits limits = new JointLimits { min = 0, max = 0 };
                hinge.limits = limits;
                hinge.useLimits = true;
            }
            else
            {
                // La puerta puede abrir hasta 90°
                JointLimits limits = new JointLimits { min = 0, max = 90 };
                hinge.limits = limits;
                hinge.useLimits = true;
            }
        }

        // Capas de interacción XR
        interactionLayers = isLocked
            ? InteractionLayerMask.GetMask(defaultLayer) // No se agarra
            : InteractionLayerMask.GetMask(grabLayer);   // Se puede agarrar
    }
    

}
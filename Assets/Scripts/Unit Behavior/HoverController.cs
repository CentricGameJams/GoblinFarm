using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverController : MonoBehaviour, IVisualFeedbackController
{
    [SerializeField] private GameObject _hoverFeedbackObject;


    public void SetHoverFeedback(bool newState)
    {
        if (_hoverFeedbackObject != null)
        {
            _hoverFeedbackObject.SetActive(newState);
        }
    }
}

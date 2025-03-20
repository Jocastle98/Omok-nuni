using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TestNetworkManager : MonoBehaviour
{
    [FormerlySerializedAs("SignupPanel")] public GameObject mSignupPanel;
    [FormerlySerializedAs("SigninPanel")] public GameObject mSigninPanel;
    private Canvas mCanvas;
    
    void Start()
    {
        mCanvas = GameObject.FindObjectOfType<Canvas>();
        if (mCanvas != null)
        {
            var signinPanelObj = Instantiate(mSigninPanel, mCanvas.transform);
            signinPanelObj.GetComponent<PanelController>().Show();
        }
    }

    void Update()
    {
        
    }

    public void OnClickSignupButton()
    {
        if (mCanvas != null)
        {
            var signupPanelObj = Instantiate(mSignupPanel, mCanvas.transform);
            signupPanelObj.GetComponent<PanelController>().Show();
        }
    }
}

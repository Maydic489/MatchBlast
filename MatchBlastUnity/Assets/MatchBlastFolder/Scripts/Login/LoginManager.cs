using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using TMPro;

public class LoginManager : MonoBehaviour
{
    FirebaseAuth auth;
    [SerializeField] TMP_Text statusText;

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    void SetStatusText(string newText)
    {
        statusText.text = newText;
    }

    public void SignInAnonymousely()
    {
        auth.SignInAnonymouslyAsync().ContinueWith(task => {
            if (task.IsCanceled)
            {
                SetStatusText("SignInAnonymouslyAsync was canceled.");
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                SetStatusText("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            AuthResult result = task.Result;
            SetStatusText("User signed in successfully: " + result.User.DisplayName + " (" + result.User.UserId + ")");
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
        });
    }
}

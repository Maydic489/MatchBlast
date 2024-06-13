using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using Firebase.Extensions;
using Firebase;

public class LoginManager : MonoBehaviour
{
    FirebaseAuth auth;
    DatabaseReference firebaseDB;

    [SerializeField] TMP_Text statusText;
    [SerializeField] TMP_InputField userNameInputField;

    private void Start()
    {
        InitFirebase();
    }

    void SetStatusText(string newText)
    {
        statusText.text = newText;
    }

    void InitFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted && task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                firebaseDB = FirebaseDatabase.DefaultInstance.RootReference;
                auth = FirebaseAuth.DefaultInstance; // Ensure FirebaseAuth is initialized here as well
                Debug.Log("FB init success");
            }
        });
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

            GetUserNameFromDB();
        });
    }

    public void SetUserNameToDB()
    {
        var userId = auth.CurrentUser.UserId;

        User user = new User(userNameInputField.text);
        string json = JsonUtility.ToJson(user);
        firebaseDB.Child("users").Child(userId).SetRawJsonValueAsync(json);
    }

    void GetUserNameFromDB()
    {
        FirebaseDatabase.DefaultInstance.GetReference("users").Child(auth.CurrentUser.UserId)
      .GetValueAsync().ContinueWithOnMainThread(task => {
          if (task.IsFaulted)
          {
              // Handle the error...
          }
          else if (task.IsCompleted)
          {
              DataSnapshot snapshot = task.Result;
              string username = snapshot.Child("username").Value.ToString();
              Debug.Log("Username: " + username);
              SetStatusText("Username: " + username);
          }
      });
    }
}

public class User
{
    public string username;

    public User() { }

    public User(string username)
    {
        this.username = username;
    }
}

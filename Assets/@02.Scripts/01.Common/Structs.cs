using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UserDataStructs
{
    public struct SignupData
    {
        public string username;
        public string nickname;
        public string password;
    }
    
    public struct SigninData
    {
        public string username;
        public string password;

        public SigninData(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
    
    public struct SigninResult
    {
        public int result;
        public string nickname;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    private static T m_instance;

    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<T>();
                if (m_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    m_instance = obj.AddComponent<T>();
                }
            }
            
            return m_instance;
        }
    }
    
    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this as T;
            DontDestroyOnLoad(gameObject);
            
            // 씬 전환시 호출되는 액션 메서드 할당
            
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    protected abstract void OnSceneLoaded(Scene scene, LoadSceneMode mode);
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSceneChanger : MonoBehaviour
{
    [SerializeField] int sceneToLoadTo = 2;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            SceneLoader.LoadSceneByIndex(sceneToLoadTo);
        }
    }
}

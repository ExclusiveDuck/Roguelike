using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestFoor : MonoBehaviour
{
    GameManager manager;
    [SerializeField]
    SingleValuesSO testSo;


    private void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            testSo.Value++;

            SceneManager.LoadScene(manager.randomScene);
        }
    }
}
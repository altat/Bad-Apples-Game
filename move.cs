using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class move : MonoBehaviour {

    public Transform home;
    public GameObject cam;
    public GameObject panel;

    public void moving()
    {
        cam.transform.position = home.position;
    }

    public void yes()
    {
        SceneManager.LoadScene("Choose");
    }

    public void no()
    {
        panel.SetActive(false);
    }

    public void courtconfirm()
    {
        panel.SetActive(true);
    }

}

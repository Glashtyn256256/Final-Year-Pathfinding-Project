using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void PathfindingMapOneOnClick()
    {
        SceneManager.LoadScene("PathfindingMapOne");
    }

    public void PathfindingMapTwoOnClick()
    {
        SceneManager.LoadScene("PathfindingMapTwo");
    }

    public void PathfindingMapThreeOnClick()
    {
        SceneManager.LoadScene("PathfindingMapThree");
    }

    public void PathfindingMapCustomOnClick()
    {
        SceneManager.LoadScene("PathfindingMapCustom");
    }

    public void MinHeapMapOneOnClick()
    {
        SceneManager.LoadScene("MinHeapMapOne");
    }

    public void MinHeapMapTwoOnClick()
    {
        SceneManager.LoadScene("MinHeapMapTwo");
    }

    public void MinHeapMapThreeOnClick()
    {
        SceneManager.LoadScene("MinHeapMapThree");
    }

    public void MinHeapMapCustomOnClick()
    {
        SceneManager.LoadScene("MinHeapMapCustom");
    }

    public void FlowfieldMapOneOnClick()
    {
        SceneManager.LoadScene("FlowfieldMapOne");
    }

    public void FlowfieldMapTwoOnClick()
    {
        SceneManager.LoadScene("FlowfieldMapTwo");
    }

    public void FlowfieldMapThreeOnClick()
    {
        SceneManager.LoadScene("FlowfieldMapThree");
    }

    public void FlowfieldMapCustomOnClick()
    {
        SceneManager.LoadScene("FlowfieldMapCustom");
    }
}

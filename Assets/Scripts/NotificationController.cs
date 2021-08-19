using System;
using UnityEngine;
using UnityEngine.UI;

public class NotificationController : MonoBehaviour
{
    public Camera camera;
    public GameObject ExclamationPointPrefab;
    public GameObject QuestionMarkPointPrefab;
    public GameObject PresentPointPrefab;

    private static NotificationController _instance;

    public static NotificationController Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public ExclamationPoint CreateExclamation(Vector3 target)
    {
        var inst = Instantiate(ExclamationPointPrefab, target, Quaternion.identity);
        var exc = inst.GetComponent<ExclamationPoint>();
        inst.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        inst.transform.SetSiblingIndex(0);
        exc.SetParams(target, camera);
        inst.SetActive(true);
        return exc;
    }

    public ExclamationPoint CreateQuestion(Vector3 target)
    {
        var inst = Instantiate(QuestionMarkPointPrefab, target, Quaternion.identity);
        var exc = inst.GetComponent<ExclamationPoint>();
        inst.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        inst.transform.SetSiblingIndex(0);
        exc.SetParams(target, camera);
        inst.SetActive(true);
        return exc;
    }

    public ExclamationPoint CreatePresent(Vector3 target)
    {
        var inst = Instantiate(PresentPointPrefab, target, Quaternion.identity);
        var exc = inst.GetComponent<ExclamationPoint>();
        inst.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform, false);
        inst.transform.SetSiblingIndex(0);
        exc.SetParams(target, camera);
        inst.SetActive(true);
        return exc;
    }
}

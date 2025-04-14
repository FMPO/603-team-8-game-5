using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BugCollection : MonoBehaviour
{
    public static BugCollection instance;
    public TextMeshProUGUI bugsCollected;
    int bugCount = 0;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        bugsCollected.text = "Bugs Count: " + bugCount.ToString();
    }

    // Adds to bug count
    public void AddBug()
    {
        bugCount += 1;
        bugsCollected.text = "Bugs Count: " + bugCount.ToString();
    }
}

using UnityEngine;
using DG.Tweening;

public class BananaControl : MonoBehaviour
{
    private Transform monkeyTransform;
    private Rigidbody rb;
    void Start()
    {
        monkeyTransform = GameObject.FindGameObjectWithTag("Monkey").transform;
        rb = GetComponent<Rigidbody>();
        transform.DOMove(monkeyTransform.position, .3f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monkey"))
        {
            Destroy(gameObject);
            MonkeyManager.Instance.GetShot();
        }
    }

}




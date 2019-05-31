using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTest : MonoBehaviour
{
    public Transform target;
    Transform gunBarrel;

    Quaternion rootRot;
    Quaternion gunRot;

    float angle = 0;

    Quaternion lerpRot;
    Quaternion slerpRot;

    float t;

    // Start is called before the first frame update
    void Start()
    {
        gunBarrel = transform.Find("GunBase");
        //transform.rotation = Quaternion.Euler(0, 30, 0);
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime / 10;

        lerpRot = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(0, 180, 0), t);
        slerpRot = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(0, 180, 0), t);

        //Debug.Log(lerpRot.eulerAngles.y + "," + slerpRot.eulerAngles.y);

        if(Input.GetKeyDown(KeyCode.R))
        {
            transform.rotation = transform.rotation * Quaternion.Euler(0, 10, 0);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Vector3 originRight = transform.right;
            Vector3 to = Quaternion.AngleAxis(-30, transform.right) * transform.forward;

            Vector3 up = Vector3.Cross(to, transform.right);

            Quaternion rot = Quaternion.LookRotation(to, up);

            transform.rotation = rot;
        }

        if(Input.GetKeyDown(KeyCode.Y))
        {
            Vector3 aimDirection = (target.position - transform.position).normalized;
            Quaternion aimRot = Quaternion.FromToRotation(gunBarrel.forward, aimDirection);
            transform.rotation = aimRot * transform.rotation;

            GameObject bullet = Instantiate(Resources.Load<GameObject>("Bullet"), gunBarrel.position + gunBarrel.forward * 2, Quaternion.identity);
            bullet.GetComponent<Rigidbody>().velocity = gunBarrel.forward * 10;
        }

        if(Input.GetKeyDown(KeyCode.U))
        {
            SetTarget();
        }

        if(Input.GetKeyDown(KeyCode.I))
        {
            StartCoroutine(RotateTimer());
        }

        //AimAtTarget();
    }

    void SetTarget()
    {
        Vector3 aimDirection = (target.position - transform.position).normalized;

        Vector3 rootDirection = new Vector3(aimDirection.x, 0, aimDirection.z);
        rootRot = Quaternion.LookRotation(rootDirection, Vector3.up);


        Vector3 gunDirection = (target.position - gunBarrel.position).normalized;
        gunRot = Quaternion.LookRotation(gunDirection, Vector3.up);

        StartCoroutine(AimTargetCoroutine());
    }

    void AimAtTarget()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rootRot, 90 * Time.deltaTime);
        gunBarrel.rotation = Quaternion.RotateTowards(gunBarrel.rotation, gunRot, 45 * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        Vector3 lerpVec = lerpRot * Vector3.right;
        Vector3 slerpVec = slerpRot * Vector3.right;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(Vector3.zero, lerpVec * 10);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(Vector3.zero, slerpVec * 10);


        //Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(transform.position, 0.2f);

        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(transform.position - new Vector3(1, 1, 0) * 50, transform.position + new Vector3(1, 1, 0) * 50);
    }

    IEnumerator AimTargetCoroutine()
    {
        while(true)
        {
            AimAtTarget();

            if(transform.rotation == rootRot && gunBarrel.rotation == gunRot)
            {
                Debug.Log("已到达瞄准位置，跳出循环");
                break;
            }

            yield return null;
        }
    }

    IEnumerator RotateTimer()
    {
        float t = 0;

        while(t < 5)
        {
            transform.Rotate(0, 180 * Time.deltaTime, 0);
            t += Time.deltaTime;
            yield return null;
        }
    }
}

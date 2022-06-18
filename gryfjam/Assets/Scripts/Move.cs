using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Move : MonoBehaviour
{
    [SerializeField] private Transform pointer;
    [SerializeField] private float speedMove;
    [SerializeField] private GameObject camera;
    [SerializeField] private float shakeCam; 
    [SerializeField] private float durationShake; 
    private LineRenderer dirLine;
    private Vector2 pos;
    private Rigidbody2D rb;
    private bool stop;

    private Vector2 dir;
    private Vector2 temp;
    private float dist;


    float smooth = 5.0f;
    float tiltAngle = 60.0f;


    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        dirLine = gameObject.GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (Input.anyKey)
        {
            float tiltAroundZ = Input.GetAxis("Horizontal") * tiltAngle;
            float tiltAroundX = Input.GetAxis("Vertical") * tiltAngle;
            Quaternion target = Quaternion.Euler(tiltAroundX, 0, tiltAroundZ);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);
        }


        float px = 0, py = 0;

        px = Input.GetAxisRaw("Horizontal") * Time.deltaTime * speedMove;
        py = Input.GetAxisRaw("Vertical") * Time.deltaTime * speedMove;
        if (px == 0 && py == 0)
        {
            if (stop)
            {
                stop = false;
                ThrowBall(gameObject.transform.position, pointer.position);
            }
            pos = gameObject.transform.position;
        }
        else stop = true;

        pos.x += px * 2;
        pos.y += py * 2;
        pos.x = Mathf.Clamp(pos.x, transform.position.x - 3, transform.position.x + 3);
        pos.y = Mathf.Clamp(pos.y, transform.position.y - 3, transform.position.y + 3);
        dir = (pos - (Vector2)gameObject.transform.position).normalized;
        dist = Vector2.Distance(gameObject.transform.position, pos);

        if (dist < 3) pointer.position = (Vector3)dir * dist + gameObject.transform.position;
        else pointer.position = (Vector3)dir * 3 + gameObject.transform.position;

        dirLine.SetPosition(0, gameObject.transform.position);
        dirLine.SetPosition(1, pointer.position);
        dirLine.SetColors(new Color(dist, 0, 0, 255), new Color(dist * 4, 0, 0, 0));
        camera.transform.position = new Vector3(pointer.position.x, pointer.position.y, -10);

 
    }

    private void ThrowBall(Vector2 s, Vector2 e)
    {
        rb.AddForce((s - e).normalized * (s-e).magnitude * 600, ForceMode2D.Force);
        StartCoroutine(ShakeCamera());
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if(col.collider.tag == "Bullet")
        {
            SceneManager.LoadScene(1);
        }

    }
    public IEnumerator ShakeCamera()
    {
        
        float coef = 1;
        while(coef > 0.1f)
        {
            coef = Mathf.Lerp(coef, 0, durationShake);
            float x = Random.Range(-shakeCam,shakeCam);
            float y = Random.Range(-shakeCam,shakeCam);
            Vector3 posCamera = camera.transform.position;

            camera.transform.position = posCamera + (Vector3.right * x + Vector3.up * y + Vector3.forward * - 10);
            yield return null;
        }
       
    }
}
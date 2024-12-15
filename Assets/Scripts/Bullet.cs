using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10;
    public Player shotBy;
    public GameObject particles;
    private Rigidbody2D rb;
    public void Shoot(Player p)
    {
        rb = GetComponent<Rigidbody2D>();
        shotBy = p;
        Vector3 delta = Vector3.zero;
        int theta = 0;
        switch(p.direction) {
            case 'N':
                delta = Vector3.up;
                theta = 0;
                break;
            case 'W':
                delta = Vector3.left;
                theta = 90;
                break;
            case 'S':
                delta = Vector3.down;
                theta = 180;
                break;
            case 'E':
                delta = Vector3.right;
                theta = 270;
                break;
        }
        transform.position = p.transform.position + delta;
        transform.eulerAngles = new Vector3(0, 0, theta);
        rb.velocity = transform.up * speed;
    }

    private void Shatter()
    {
        Instantiate(particles, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }

    private void Reflect()
    {
        transform.eulerAngles += new Vector3(0, 0, 180);
        rb.velocity = transform.up * speed;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.gameObject.TryGetComponent<ObjProperties>(out ObjProperties props)) {
            if(props.reflective) {
                Reflect();
            } else {
                Shatter();
            }
        }
        else {
            Shatter();
        }
    }
}

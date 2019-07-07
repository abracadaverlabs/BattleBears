using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTeddy : MonoBehaviour
{
    public Laser laserPrefab;
    public int team;
    public int health;
    public int attackPower;
    public float fieldOfView;

    protected Animator anim;

    private Eye[] eyes;

    //We make this function protected (= accessible to inheriting classes, but not to others)
    //We make this function virtual (= can be overridden)
    protected virtual void Start()
    {
        anim = GetComponentInChildren<Animator>();

        eyes = GetComponentsInChildren<Eye>();

        //We use team as an index in the colors array
        //This way, if we are team 0, we get color at index 0, etc.
        //We can access the GameManager SINGLETON by simply writing GameManager.instance
        Color teamColor = GameManager.instance.teamColors[team];

        //transform.Find is a BAD and SLOW way to access another GameObject
        //Just showing it as an example
        //Better to avoid it, use public references instead.
        transform.Find("Teddy/Teddy_Body").GetComponent<SkinnedMeshRenderer>().material.color = teamColor;
    }

    protected bool CanSee (Transform hitObj, Vector3 hitPos)
    {
        foreach (Eye eye in eyes)
        {
            //We calculate the direction from object A to B by using dir = B - A;
            Vector3 dir = hitPos - eye.transform.position;

            //We calculate the angle to see if we the teddy is in front of us or not
            float angleToEnemy = Vector3.Angle(transform.forward, dir);
            if (angleToEnemy > fieldOfView)
            {
                //This breaks out of this cycle of the foreach loop, and continues with the next
                continue;
            }
            
            //We create a ray the starts at the eye an going toward hitPos
            Ray eyeRay = new Ray(eye.transform.position, dir);

            RaycastHit hitInfo;

            //If one of the eyes sees the target, we return true
            if (Physics.Raycast(eyeRay, out hitInfo))
            {
                if (hitInfo.transform == hitObj)
                {
                    return true;
                }
            }
        }

        //If neither eye could see the target (= raycast toward the hitObj), we return false
        return false;
    }

    protected void ShootLasers (Transform hitObj, Vector3 hitPos)
    {
        //Display the lasers
        foreach(Eye eye in eyes)
        {
            Laser laserClone = Instantiate(laserPrefab);
            laserClone.Init(eye.transform.position, hitPos);
        }

        //This tries to get the BaseTeddy component from the GameObject we hit
        BaseTeddy teddy = hitObj.GetComponent<BaseTeddy>();
        if (teddy != null)
        {
            teddy.OnHit(attackPower);
        }
    }

    public void OnHit(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void Die ()
    {
        anim.SetBool("IsDead", true);
    }
}

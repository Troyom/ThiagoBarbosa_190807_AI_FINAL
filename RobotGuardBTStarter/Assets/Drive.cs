using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Drive : MonoBehaviour {

    //acerta a velociadde
	float speed = 20.0F;

    //Acerta a rotaçao
    float rotationSpeed = 120.0F;

    //Pega o objeto bala
    public GameObject bulletPrefab;

    //Cria a vida
    public float health = 100f;

    //Cria a barra de vida
    public Slider healthBar;

    //Cria o objeto bala
    public Transform bulletSpawn;

    public Transform respawn;

    void Update() {

        //movimento com o botao vertical
        float translation = Input.GetAxis("Vertical") * speed;

        //movimento com o botao horizontalmente
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;

        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;
        transform.Translate(0, 0, translation);
        transform.Rotate(0, rotation, 0);

        //Pression para atirar
        if(Input.GetKeyDown("space"))
        {
            GameObject bullet = GameObject.Instantiate(bulletPrefab, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
            bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.forward*2000);
        }

        //Faz a barra de vida ficar acima do player
        Vector3 healthBarPos = Camera.main.WorldToScreenPoint(this.transform.position);
        healthBar.value = (int)health;
        healthBar.transform.position = healthBarPos + new Vector3(0, 60, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("bullet"))
        {
            health -= 10;
            if (health <= 0)
            {
                transform.position = respawn.position;
                health = 100;
            }
        }
        if (collision.collider.CompareTag("enemy"))
        {
            health -= 20;

            if (health <= 0)
            {
                transform.position = respawn.position;
                health = 100;
            }
        }

    }
}

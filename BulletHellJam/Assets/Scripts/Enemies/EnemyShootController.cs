using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShootController : Enemy
{
    [SerializeField] GameObject center;
    [SerializeField] GameObject BulletEnemyShoot;

    [SerializeField] float DistanceToStop = 3.5f;
    private float timerToShoot = 0f;
    [SerializeField] float timeToShoot = 1f;

    [SerializeField] private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        center = GameObject.FindGameObjectWithTag("base");
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        move = (center.transform.position - transform.position).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        if((center.transform.position - transform.position).magnitude >= DistanceToStop){
            transform.position += new Vector3(move.x * speed * Time.deltaTime, move.y * speed * Time.deltaTime, 0f);
        }else{
            timerToShoot += Time.deltaTime;
            if(timeToShoot <= timerToShoot){
                timerToShoot = 0f;
                ShootController();
            }
        }
        if (spriteRenderer.color != originalColor)
        {
            manageColorDamage();
        }
    }

    void OnCollisionEnter2D(Collision2D collision){
        if (collision.gameObject.tag == "Bullet")
        {
            Damage();
        }
    }

    void ShootController(){
        float rand = UnityEngine.Random.Range(0f, 100f);
        if(rand <= 50f){
            animator.SetBool("shooting", true);
            GameObject bullet = Instantiate(BulletEnemyShoot, transform.position, Quaternion.identity);
            
            AudioManager.instance.PlaySFX(AudioManager.instance.enemyProjectile);

            bullet.GetComponent<BulletEnemyController>().setDirection(move);
        }
    }

}

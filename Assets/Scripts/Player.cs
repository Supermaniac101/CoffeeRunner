using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MovingObject {

    public int pointsPerSmall = 10;
    public int pointsPerBig = 20;
    public Text energyText;

    public float restartLevelDelay = 1f;
    private Animator animator;
    private int energy;
    private Vector2 touchOrigin = Vector2.one;



    protected override void Start()
    {
        animator = GetComponent<Animator>();
        energy = GameManager.instance.playerEnergyPoints;
        energyText.text = "ENERGY: " + energy;
        base.Start();
    }

    private void OnDisable()
    {
        GameManager.instance.playerEnergyPoints = energy;
    }
    
    private void Update()
    {
        if (!GameManager.instance.playersTurn) return;
        int horizontal = 0;
        int vertical = 0;


#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");
        
        if (horizontal != 0)
            vertical = 0;

#else
		if (Input.touchCount > 0)
		{
			Touch myTouch = Input.touches[0];

			if (myTouch.phase == TouchPhase.Began)
			{
				touchOrigin = myTouch.position;
			} else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
			{
				Vector2 touchEnd = myTouch.position;
				float x = touchEnd.x - touchOrigin.x;
				float y = touchEnd.y - touchOrigin.y;
				touchOrigin.x = -1;
				if (Mathf.Abs(x) > Mathf.Abs(y))
					horizontal = x > 0 ? 1 : -1;
				else
					vertical = y > 0 ? 1 : -1;
			}
		}

#endif


        if (horizontal != 0 || vertical != 0)
        {
            AttemptMove<Enemy>(horizontal, vertical);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if(other.tag == "Food")
        {
            energy += pointsPerSmall;
            animator.SetTrigger("playerDrink");
            energyText.text = "+" + pointsPerSmall + " ENERGY: " + energy;
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Soda")
        {
            energy += pointsPerBig;
            animator.SetTrigger("playerDrink");
            energyText.text = "+" + pointsPerBig + " ENERGY: " + energy;
            other.gameObject.SetActive(false);
        }
    }

    public void LoseEnergy(int loss)
    {
        animator.SetTrigger("playerHit");
        energy -= loss;
        energyText.text = "-" + loss + " ENERGY: " + energy;
        CheckIfGameOver();
    }

    protected override void OnCantMove<T>(T component)
    {
        //TODO: Add logic
        //Wall hitWall = component as Wall;
    }

    private void Restart()
    {
        SceneManager.LoadScene("GameScene");
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        energy--;
        energyText.text = "ENERGY: " + energy;
        base.AttemptMove<T>(xDir, yDir);
        RaycastHit2D hit;
        CheckIfGameOver();
        GameManager.instance.playersTurn = false;
    }

    private void CheckIfGameOver()
    {
        if(energy <= 0)
        {
            GameManager.instance.GameOver();
        }
    }

}

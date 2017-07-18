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
        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
            vertical = 0;

        if (horizontal != 0 || vertical != 0)
        {
            //TODO: Add logic
            
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

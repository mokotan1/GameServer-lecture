﻿using System;
using System.Threading;

class SpaceShooterGame
{
    static bool isRunning = true;       // Game running status
    static int playerPosition = 10;     // Player position
    static int enemyPosition = 30;      // Enemy position
    static int playerHealth = 100;      // Player health
    static int enemyHealth = 100;       // Enemy health
    static int enemy2Health = 100;
    static int enemy2Position = 0;
    static object lockObject = new object(); // Lock object for thread safety

    static void Main(string[] args)
    {  
        Console.WriteLine("Welcome to Space Shooter!");
        Console.WriteLine("Press 'A' to move left, 'D' to move right, 'F' to fire. Press 'Q' to quit.");
        Console.Write("Game starts within ");
        for(int i =3; i>0; i--)
        {            
            Console.Write(i + " ");
            Thread.Sleep(500);
        }
        Console.WriteLine(" Go~~~!");

        Thread enemyThread = new Thread(new ThreadStart(EnemyBehavior));
        // 에너미 스레드 추가
        Thread enemyThread2 = new Thread(new ThreadStart(Enemy2Behavior));
        enemyThread.Start();  // Start enemy behavior thread
        enemyThread2.Start();

        while (isRunning && playerHealth > 0 && enemyHealth > 0)
        {
            // Player controls
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                lock (lockObject)
                {
                    switch (key)
                    {
                        case ConsoleKey.A:
                            MovePlayerLeft();
                            break;
                        case ConsoleKey.D:
                            MovePlayerRight();
                            break;
                        case ConsoleKey.F:
                            FireWeapon();
                            break;
                        case ConsoleKey.Q:
                            isRunning = false;
                            break;
                    }
                }
            }

            // Simulate game frame update
            Thread.Sleep(500);
            UpdateGame();
        }

        isRunning = false;
        enemyThread.Join();  // Ensure enemy thread stops before ending game
        enemyThread2.Join();
        EndGame();
    }

    // Moves the player left
    static void MovePlayerLeft()
    {
        if (playerPosition > 0)
        {
            playerPosition--;
            Console.WriteLine(">>> Player moved left to position " + playerPosition);
        }
    }

    // Moves the player right
    static void MovePlayerRight()
    {
        if (playerPosition < 20)
        {
            playerPosition++;
            Console.WriteLine(">>> Player moved right to position " + playerPosition);
        }
    }

    // Fires player's weapon
    static void FireWeapon()
    {
        Console.WriteLine("Player fired!");
        if (Math.Abs(playerPosition - enemyPosition) < 5)
        {
            lock (lockObject)
            {
                enemyHealth -= 10;
                Console.WriteLine("Hit! Enemy health: " + enemyHealth);
            }
        }
        else
        {
            Console.WriteLine("Missed! Enemy is too far.");
        }
    }

    // Enemy behavior thread: Moves enemy and attacks player
    static void EnemyBehavior()
    {
        Random random = new Random();
        while (isRunning && enemyHealth > 0)
        {
            lock (lockObject)
            {
                // Random enemy movement
                int moveDirection = random.Next(0, 3);
                if (moveDirection == 0 && enemyPosition > 0)
                {
                    enemy2Position--;
                }
                else if (moveDirection == 1 && enemy2Position < 20)
                {
                    enemy2Position++;
                }

                Console.WriteLine("> Enemy moved to position " + enemy2Position);

                // Enemy attack if close to the player
                if (Math.Abs(playerPosition - enemyPosition) < 5)
                {
                    playerHealth -= 10;
                    Console.WriteLine("Enemy attacked! Player health: " + playerHealth);
                }
            }

            Thread.Sleep(500); // Enemy action delay
        }
    }

//에너미 2를 제어할 함수    
    static void Enemy2Behavior()
    {
        Random random = new Random();
        while (isRunning && enemy2Health > 0)
        {
            lock (lockObject)
            {
                // Random enemy movement
                int moveDirection = random.Next(0, 3);
                if (moveDirection == 0 && enemy2Position > 0)
                {
                    enemy2Position--;
                }
                else if (moveDirection == 1 && enemy2Position < 20)
                {
                    enemy2Position++;
                }

                Console.WriteLine("> Enemy2 moved to position " + enemy2Position);

                // Enemy attack if close to the player
                if (Math.Abs(playerPosition - enemy2Position) < 5)
                {
                    playerHealth -= 10;
                    Console.WriteLine("Enemy2 attacked! Player health: " + playerHealth);
                }
            }

            Thread.Sleep(500); // Enemy action delay
        }
    }

    // Game update loop
    static void UpdateGame()
    {
        Console.WriteLine("Player Health: " + playerHealth + " | Enemy Health: " + enemyHealth + " | Enemy2 Health: " + enemy2Health);
    }

    // End game logic
    static void EndGame()
    {
        if (playerHealth <= 0)
        {
            Console.WriteLine("Game Over! You were defeated.");
        }
        else if (enemyHealth <= 0 && enemy2Health <= 0)
        {
            Console.WriteLine("Victory! You defeated the enemy.");
        }
        else
        {
            Console.WriteLine("Game exited.");
        }
    }
}


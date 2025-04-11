using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// This is a Game Manager Script Written by Patrick Emmons for the game "Pin Brawl" in 2024
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject[] players;
    public int livePlayers;
    public static AudioClips audioManager;
    public List<string> levels = new List<string> { "Gameplay" }; // Add your level names here
    public float screenShakeIntensity = 5f;
    public List<Texture2D> colorPalettes;
    public List<Texture2D> unusedPalettes; // Palettes aren't in use by any of the players


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        unusedPalettes = new List<Texture2D>();
        foreach(Texture2D palette in this.colorPalettes)
        {
            unusedPalettes.Add(palette);
        }
    }

    void Start()
    {
        audioManager = this.GetComponent<AudioClips>();

    }

    void FixedUpdate()
    {
        //if (SceneManager.GetActiveScene().name != "Gameplay") //used to be main menu scene
        //{
        //    gameObject.GetComponent<PlayerInputManager>().DisableJoining();
        //}
        //else
        //{
        //    gameObject.GetComponent<PlayerInputManager>().EnableJoining();
        //}
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GetPlayerIds()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
        }
    }


    public void ReloadLevel()
    {
        ClearPlayers();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void CompleteGame()
    {
        SceneManager.LoadScene("Credits");
    }

    //public void UpdateUnusedPalettes()
    //{
    //    unusedPalettes.Clear();
    //    for (int i = 0; i < colorPalettes.Count; i++)
    //    {
    //        bool isUsed = false;
    //        for (int j = 0; j < players.Length; j++)
    //        {
    //            if (players[j].GetComponent<Player>().GetComponent<SpriteRenderer>().material.GetTexture("_PaletteTex") == colorPalettes[i])
    //            {
    //                isUsed = true;
    //                break;
    //            }
    //        }
    //        if (!isUsed)
    //        {
    //            unusedPalettes.Add(colorPalettes[i]);
    //        }
    //    }
    //}

    //restart the game
    public void Restart()
    {
        //ClearPlayers();

        //// Clear unused palettes
        //unusedPalettes = new List<Texture2D>();
        //foreach (Texture2D palette in this.colorPalettes)
        //{
        //    unusedPalettes.Add(palette);
        //}
        SceneManager.LoadScene("MainMenu");
    }

    private void ClearPlayers()
    {
        foreach (var player in players)
        {
            Destroy(player);
        }

        players = Array.Empty<GameObject>();
    }

}

using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Security.Cryptography;
using System.Text;

using UnityEngine;
using UnityEngine.UI;

public class RetroRushHandler : MonoBehaviour
{
    // Game Variables
    int totalLives = 3, gameMode = 0, buttonIndex = 0, totalScore = 0, gameID = -1, switchesPerformed = 1;
    const int NumOfGames = 4;
    [SerializeField] AudioSource changeSound;

    [SerializeField] FroggieGameHandler FroggieRushHandler;
    [SerializeField] LoneStarGameHandler LoneStarHandler;
    [SerializeField] CyberMayhamGameHandler CyberMayhamHandler;
    [SerializeField] ClickItGameHandler ClickItHandler;
    
    // General UI Variables
    [SerializeField] GameObject[] LifeIcons, GameStateScreens;
    [SerializeField] GameObject CoverScreen, froggieCover, LoneStarCover, CyberMayhamCover, ClickItCover, highScoreObject;
    [SerializeField] Button[] MainButtons;
    [SerializeField] Text ScoreDisplay, MenuHiScore;

    string key = "I_check_my_code_before_pushing_to_prod";
    string iv = "I_do_check_my_code_for_sensitive_keys";

    void Start()
    {
        Application.targetFrameRate = 65;

        froggieCover.SetActive(false);
        LoneStarCover.SetActive(false);
        CyberMayhamCover.SetActive(false);
        ClickItCover.SetActive(false);

        DefaultGameState();
    }

    void Update()
    {
        Application.targetFrameRate = 65;

        if (totalScore >= switchesPerformed * 1500)
        {
            ++switchesPerformed;
            SwitchingGame();
        }
        
        if (totalLives == 0) GameOver();

        if (gameMode == 0) GUISelection();
    }

    public void UpdateScore(int pts)
    {
        totalScore+=pts;
        ScoreDisplay.text = totalScore.ToString();
    }

    public int GetScore()
    {
        return totalScore;
    }

    public void LoseLife()
    {
        if (totalLives > 0) --totalLives;

        // 0 - 1 - 2 :: length = 3
        LifeIcons[LifeIcons.Length - (LifeIcons.Length - totalLives)].SetActive(false);
    }

    void GUISelection()
    {
        highScoreObject.SetActive(LoadHighScore() > 0);
        DisplayHighScore();

        if ((Input.GetKeyDown(KeyCode. UpArrow) || Input.GetKeyDown(KeyCode. W))) buttonIndex++;
        if ((Input.GetKeyDown(KeyCode. DownArrow) || Input.GetKeyDown(KeyCode. S))) buttonIndex--;

        if (buttonIndex >= MainButtons.Length) buttonIndex = 0;
        if (buttonIndex < 0) buttonIndex = MainButtons.Length - 1;

        MainButtons[buttonIndex].Select();
    }

    public void RevealGameExternal()
    {
        // Show the player the next game
        CoverScreen.SetActive(false);
    }

    void SwitchingGame()
    {
        // Cover the entire game screen to hide any visuals for changing the game
        CoverScreen.SetActive(true);

        // Make sure UI objects for specified gamed are disabled when switching
        LoneStarHandler.DisableGameUI();

        // Switch to a new game
        int rInt = Random.Range(0, NumOfGames - 1);

        // if the random number selected is the same as the gameID
        // keep fetching random number till we get something diff
        while (rInt == gameID)
        {
            rInt = Random.Range(0, NumOfGames - 1);
        }

        gameID = rInt;

        // Individual Game Debugging
        gameID = 2;

        StartCoroutine(ReadyPlayerForNextGame(gameID));
    }

    IEnumerator ReadyPlayerForNextGame(int gameID)
    {
        changeSound.pitch = Random.Range(0.75f, 1);
        changeSound.Play();

        // Cyber Mayham is Out of Order
        if (gameID == 2) {
            if (Random.Range(0, 100) % 3 == 0)
                --gameID;
            else
                ++gameID;
        }

        // Manual Dev Test
        // gameID = 0;

        switch (gameID)
        {
            case 0:
                LoneStarHandler.transform.gameObject.SetActive(false);
                CyberMayhamHandler.transform.gameObject.SetActive(false);
                ClickItHandler.transform.gameObject.SetActive(false);

                FroggieRushHandler.ResetFroggie();
                LoneStarHandler.CleanUpArea();
                ClickItHandler.StopHandleItems();
                if (CyberMayhamHandler.GetAntiVirus() != null) CyberMayhamHandler.GetAntiVirus().ShutDownAV();

                // Alert Player On Next Game
                froggieCover.SetActive(true);
                yield return new WaitForSeconds(3);
                froggieCover.SetActive(false);

                FroggieRushHandler.transform.gameObject.SetActive(true);
                FroggieRushHandler.NextChunk();
                FroggieRushHandler.ResetFroggie();

                // Show the player the next game
                CoverScreen.SetActive(false);
            break;
            case 1:
                FroggieRushHandler.transform.gameObject.SetActive(false);
                CyberMayhamHandler.transform.gameObject.SetActive(false);
                ClickItHandler.transform.gameObject.SetActive(false);

                FroggieRushHandler.ResetFroggie();
                LoneStarHandler.CleanUpArea();
                ClickItHandler.StopHandleItems();
                if (CyberMayhamHandler.GetAntiVirus() != null) CyberMayhamHandler.GetAntiVirus().ShutDownAV();

                // Alert Player On Next Game
                LoneStarCover.SetActive(true);
                yield return new WaitForSeconds(3);
                LoneStarCover.SetActive(false);

                LoneStarHandler.transform.gameObject.SetActive(true);
                LoneStarHandler.ReadyNextLevel(0);
                LoneStarHandler.IframePlayer();

                // Show the player the next game
                CoverScreen.SetActive(false);
            break;
            case 2:
                FroggieRushHandler.transform.gameObject.SetActive(false);
                LoneStarHandler.transform.gameObject.SetActive(false);
                ClickItHandler.transform.gameObject.SetActive(false);

                FroggieRushHandler.ResetFroggie();
                LoneStarHandler.CleanUpArea();
                ClickItHandler.StopHandleItems();
                if (CyberMayhamHandler.GetAntiVirus() != null) CyberMayhamHandler.GetAntiVirus().ShutDownAV();

                // Alert Player On Next Game
                CyberMayhamCover.SetActive(true);
                yield return new WaitForSeconds(3);
                CyberMayhamCover.SetActive(false);

                CyberMayhamHandler.SetTempScore(totalScore);
                CyberMayhamHandler.transform.gameObject.SetActive(true);

                // Show the player the next game
                CoverScreen.SetActive(false);
            break;
            case 3:
                FroggieRushHandler.transform.gameObject.SetActive(false);
                LoneStarHandler.transform.gameObject.SetActive(false);
                CyberMayhamHandler.transform.gameObject.SetActive(false);

                FroggieRushHandler.ResetFroggie();
                LoneStarHandler.CleanUpArea();
                ClickItHandler.StopHandleItems();
                if (CyberMayhamHandler.GetAntiVirus() != null) CyberMayhamHandler.GetAntiVirus().ShutDownAV();

                // Alert Player On Next Game
                ClickItCover.SetActive(true);
                yield return new WaitForSeconds(3);
                ClickItCover.SetActive(false);

                ClickItHandler.transform.gameObject.SetActive(true);
                ClickItHandler.ReloadGame();
                
                // Show the player the next game
                CoverScreen.SetActive(false);
            break;

            default:
            break;
        }
    }

//===================================================================================================
//===================================================================================================

    void DefaultGameState()
    {
        /*
            0 - Default
            1 - Loading
            2 - Game Over
        */
        GameStateScreens[0].SetActive(true); // main menu
        GameStateScreens[1].SetActive(false);
        GameStateScreens[2].SetActive(false);

        gameMode = 0;
    }

    public void StartGame() // triggered by a button
    {
        LifeIcons[0].SetActive(true);
        LifeIcons[1].SetActive(true);
        LifeIcons[2].SetActive(true);
        
        GameStateScreens[0].SetActive(false);
        GameStateScreens[1].SetActive(true); // load screen & game
        GameStateScreens[2].SetActive(false);

        gameMode = 1;

        SwitchingGame();
    }

    void GameOver()
    {
        // Screen will play a game over screen
        // then will switch to the main menu screen

        // attempt to save the score after game over
        SaveHighScore(totalScore);

        FroggieRushHandler.transform.gameObject.SetActive(false);
        FroggieRushHandler.ResetFroggie();

        LoneStarHandler.transform.gameObject.SetActive(false);
        LoneStarHandler.CleanUpArea();

        CyberMayhamHandler.transform.gameObject.SetActive(false);
        if (CyberMayhamHandler.GetAntiVirus() != null) CyberMayhamHandler.GetAntiVirus().ShutDownAV();

        ClickItHandler.transform.gameObject.SetActive(false);
        ClickItHandler.StopHandleItems();
        ClickItHandler.ClearText();

        gameMode = 2;
        
        GameStateScreens[0].SetActive(false);
        GameStateScreens[1].SetActive(false);
        GameStateScreens[2].SetActive(true); // game over screen

        totalLives = 3;
        gameMode = 0;

        buttonIndex = 0;
        totalScore = 0;

        UpdateScore(0);

        gameID = -1;

        Invoke("DefaultGameState", 3);
    }

    // Save high score
    public void SaveHighScore(int score)
    {
        if (score > LoadHighScore())
        {
            // Encrypt the string
            string encrypted = Encrypt(score.ToString(), key, iv);
            PlayerPrefs.SetString("HighScore", encrypted);
            PlayerPrefs.Save(); // Ensure the data is saved
        }
    }

    // Load high score
    public int LoadHighScore()
    {
        // Decrypt the string
        string decrypted = Decrypt(PlayerPrefs.GetString("HighScore", "0"), key, iv);
        if (decrypted == "0")
        {
            return 0;
        }

        return int.Parse(decrypted);
    }

    void DisplayHighScore()
    {
        MenuHiScore.text = LoadHighScore().ToString();
    }

    public static string Encrypt(string plainText, string key, string iv)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    byte[] encrypted = msEncrypt.ToArray();
                    return System.Convert.ToBase64String(encrypted);
                }
            }
        }
    }

    public static string Decrypt(string cipherText, string key, string iv)
    {
        if (cipherText == "0")
        {
            return "0";
        }

        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(key);
            aesAlg.IV = Encoding.UTF8.GetBytes(iv);
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(System.Convert.FromBase64String(cipherText)))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
}//EndScript    

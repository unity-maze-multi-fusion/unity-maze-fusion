using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Camera cam;
    public GameObject player;
    public GameObject wall;
    public GameObject key;
    public Canvas launchUI;
    public Canvas gameUI;
    public Canvas pauseUI;
    public Canvas leaderboard;
    public Canvas gameoverUI;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI keyText;
    public TMP_InputField inputField;

    private static int numRow = 9;
    private static int numCol = 9;
    private int[,,] map = new int[numRow, numCol, 5];

    private float scale = 5f;
    private float yPos = -1.75f;
    private float centre = 2.5f;

    public static bool isGameActive;
    public static bool isGameover;
    public static int keyCount;
    public string nameNew;
    public int time;

    // Start is called before the first frame update
    void Start()
    {
        isGameActive = false;
        isGameover = false;
        time = -1;
        inputField.characterLimit = 3;
    }

    public void StartGame()
    {
        isGameActive = true;
        launchUI.gameObject.SetActive(false);
        gameUI.gameObject.SetActive(true);
        GenerateMaze();
        keyCount = 0;
        InvokeRepeating("Timer", 0f, 1f);
    }

    public void RestartGame()
    {
        if (isGameover)
        {
            Leaderboard.record.Add((time, nameNew));
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    public void PauseGame()
    {
        gameUI.gameObject.SetActive(false);
        pauseUI.gameObject.SetActive(true);
        isGameActive = false;
    }

    public void ResumeGame()
    {
        pauseUI.gameObject.SetActive(false);
        gameUI.gameObject.SetActive(true);
        isGameActive = true;
        InvokeRepeating("Timer", 0f, 1f);
    }

    public void ShowLeaderboard()
    {
        leaderboard.gameObject.SetActive(true);
    }

    public void BackLaunch()
    {
        leaderboard.gameObject.SetActive(false);
    }



    void Update()
    {
        if (Input.GetKeyDown("escape") && !isGameover)
        {
            if (!launchUI.gameObject.activeSelf)
            {
                if (isGameActive)
                {
                    PauseGame();
                }
                else
                {
                    ResumeGame();
                }
            } else if (leaderboard.gameObject.activeSelf)
            {
                BackLaunch();
            }
        }
        if (!isGameActive)
        {
            CancelInvoke();
        }
        if (isGameover)
        {
            gameoverUI.gameObject.SetActive(true);
            nameNew = inputField.text;
        }
        keyText.text = "Keys\n" + keyCount;
    }

    public void Timer()
    {
        time++;
        timeText.text = "Time\n" + time + "\'\'";
    }

    public void GenerateMaze()
    {
        int r = 0;
        int c = 4;
        (int R, int C) pos = (r, c);
        Stack<(int, int)> fringe = new Stack<(int, int)>();
        fringe.Push(pos);

        List<char> dir;
        char next;
        int index;

        while (!(fringe.Count == 0))
        {
            map[r, c, 4] = 1;
            dir = new List<char>();

            if (c > 0 && map[r, c - 1, 4] == 0)
            {
                dir.Add('L');
            }
            if (r > 0 && map[r - 1, c, 4] == 0)
            {
                dir.Add('U');
            }
            if (c < numCol - 1 && map[r, c + 1, 4] == 0)
            {
                dir.Add('R');
            }
            if (r < numRow - 1 && map[r + 1, c, 4] == 0)
            {
                dir.Add('D');
            }

            if (!(dir.Count == 0))
            {
                pos = (r, c);
                fringe.Push(pos);
                index = Random.Range(0, dir.Count);
                next = dir[index];

                if (next == 'L')
                {
                    map[r, c, 0] = 1;
                    c -= 1;
                    map[r, c, 2] = 1;
                }
                if (next == 'U')
                {
                    map[r, c, 1] = 1;
                    r -= 1;
                    map[r, c, 3] = 1;
                }
                if (next == 'R')
                {
                    map[r, c, 2] = 1;
                    c += 1;
                    map[r, c, 0] = 1;
                }
                if (next == 'D')
                {
                    map[r, c, 3] = 1;
                    r += 1;
                    map[r, c, 1] = 1;
                }
            }
            else
            {
                pos = fringe.Pop();
                r = pos.R;
                c = pos.C;
            }
        }

        Vector3 spawnPos;

        for (int i = 0; i < numRow; i++)
        {
            for (int j = 0; j < numCol; j++)
            {
                if (i < numRow - 1 && map[i, j, 3] == 0)
                {
                    spawnPos = new Vector3(scale * (i + 1), yPos, scale * j + centre);
                    Instantiate(wall, spawnPos, Quaternion.Euler(-90f, 90f, 0f));
                }
                if (j < numCol - 1 && map[i, j, 2] == 0)
                {
                    spawnPos = new Vector3(scale * i + centre, yPos, scale * (j + 1));
                    Instantiate(wall, spawnPos, Quaternion.Euler(-90f, 0f, 0f));
                }
            }
        }
        
        List<Vector3> spawnPosVal = new List<Vector3>();

        for (int i = 0; i < 3; i++)
        {
            r = Random.Range(0, numRow - 1);
            c = Random.Range(0, numCol - 1);
            do
            {
                spawnPos = new Vector3(scale * r + centre, 0, scale * c + centre);
            } while (spawnPosVal.Contains(spawnPos));
            spawnPosVal.Add(spawnPos);
            Instantiate(key, spawnPos, Quaternion.Euler(-90f, 45f, 0f));
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro; // For TextMeshPro support

public class PuzzleUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject puzzlePanel;
    public Button exitButton;
    public GameObject completionPanel;
    public Button continueButton;
    public TextMeshProUGUI completionText; // Using TextMeshPro for better text rendering
    
    [Header("Puzzle Objects")]
    public GameObject pinkBall;
    public GameObject greenBall;
    public GameObject gate;
    public GameObject realPlayer;
    public MonoBehaviour playerControllerScript;
    public GameObject payphoneTrigger; // Tambahkan ini
    
    [Header("Settings")]
    public float ballSpeed = 5f;
    public bool makeGateDisappear = true;
    public float gateSpeed = 2f;
    public float gateOpenHeight = 5f;
    
    private bool puzzleActive = false;
    private bool gateOpening = false;
    private Vector3 pinkBallStartPos;
    private Vector3 gateStartPos;
    private Vector3 gateEndPos;

    void Start()
    {
        InitializePuzzle();
    }
    
    void InitializePuzzle()
    {
        // Hapus baris ini:
        // realPlayer = GameObject.FindGameObjectWithTag("Player");
        
        // Save positions
        pinkBallStartPos = pinkBall.transform.position;
        gateStartPos = gate.transform.position;
        gateEndPos = gateStartPos + Vector3.up * gateOpenHeight;
        
        // Setup UI
        exitButton.onClick.AddListener(ExitPuzzle);
        continueButton.onClick.AddListener(CompletePuzzle);
        
        // Hide panels initially
        puzzlePanel.SetActive(false);
        completionPanel.SetActive(false);
    }
    
    public void ShowPuzzle()
    {
        puzzlePanel.SetActive(true);

        // Nonaktifkan player
        if (realPlayer != null)
            realPlayer.SetActive(false);

        StartPuzzle();
    }
    
    void StartPuzzle()
    {
        puzzleActive = true;

        // Disable player controller
        if (playerControllerScript != null)
            playerControllerScript.enabled = false;

        pinkBall.transform.position = pinkBallStartPos;
        Debug.Log("Puzzle Started! Use WASD to move pink ball to green ball!");
    }
    
    void Update()
    {
        if (puzzleActive)
        {
            ControlPinkBall();
            CheckPuzzleComplete();
        }
        
        if (gateOpening && !makeGateDisappear)
        {
            MoveGate();
        }
    }
    
    void ControlPinkBall()
    {
        // Get input for pink ball movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        // Move pink ball (pakai unscaledDeltaTime agar tetap jalan saat Time.timeScale = 0)
        Vector3 movement = new Vector3(horizontal, vertical, 0) * ballSpeed * Time.unscaledDeltaTime;
        pinkBall.transform.position += movement;
    }
    
    void CheckPuzzleComplete()
    {
        float dist = Vector3.Distance(pinkBall.transform.position, greenBall.transform.position);
        Debug.Log("Distance: " + dist);
        if (dist < 1.0f)
        {
            PuzzleSuccess();
        }
    }
    
    public void PuzzleSuccess()
    {
        puzzleActive = false;
        completionText.text = "Puzzle Completed!";
        completionPanel.SetActive(true);

        Debug.Log("Puzzle Complete! Pink ball reached green ball!");
    }
    
    void CompletePuzzle()
    {
        completionPanel.SetActive(false);
        puzzlePanel.SetActive(false);

        // Aktifkan kembali player
        if (realPlayer != null)
            realPlayer.SetActive(true);

        // Aktifkan kembali player controller
        if (playerControllerScript != null)
            playerControllerScript.enabled = true;

        // Nonaktifkan payphone agar tidak bisa dipicu lagi
        if (payphoneTrigger != null)
            payphoneTrigger.SetActive(false);

        // Handle gate
        if (makeGateDisappear)
        {
            gate.SetActive(false);
            Debug.Log("Gate disappeared!");
        }
        else
        {
            gateOpening = true;
            Debug.Log("Gate opening...");
        }
    }
    
    void ExitPuzzle()
    {
        puzzlePanel.SetActive(false);
        puzzleActive = false;

        // Aktifkan kembali player
        if (realPlayer != null)
            realPlayer.SetActive(true);

        // Aktifkan kembali player controller
        if (playerControllerScript != null)
            playerControllerScript.enabled = true;

        // Reset pink ball
        pinkBall.transform.position = pinkBallStartPos;
    }
    
    void MoveGate()
    {
        gate.transform.position = Vector3.MoveTowards(
            gate.transform.position, 
            gateEndPos, 
            gateSpeed * Time.deltaTime
        );
        
        if (Vector3.Distance(gate.transform.position, gateEndPos) < 0.1f)
        {
            gateOpening = false;
            Debug.Log("Gate fully opened!");
        }
    }
}
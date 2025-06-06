using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManagement : MonoBehaviour
{

    // Game state variables
    // board variable
    private int[,] board;
    // [x, y] == -1 == empty spot
    // [x, y] == 0 == player 1 piece on the spot
    // [x, y] == 1 == player 2 piece on the spot


    private int current_turn = 0;
    //  0 == player 1 turn
    //  1 == player 2 turn

    // how many pieces a player has in finish spots
    // - first one to reach 5 wins and the game ends
    private int p1_finished_pieces = 0;
    private int p2_finished_pieces = 0;

    // end Game state variables

    // piece material/texture
    // - just a color material to assign to the model, so you know which piece belongs to which player
    public Material p1_material;
    public Material p2_material;
    // end piece material/texture


    // variables for communication

    // 1 - with a piece
    // for attempted move (AM)
    private PieceScript AM_piece;
    private int AM_oldX, AM_oldY;
    private int AM_newX, AM_newY;

    // 2 - with UI
    private bool move_confirmed = false;
    public Image screen_image;
    public TMP_Text screen_text;
    // TODO 

    // end variables for communication



    // Game Loop coroutine
    IEnumerator GameLoop()
    {
        // drop phase
        // players, alternatingly drop 1 piece in the center (blue tiles)
        // each player has 5 pieces so this goes on for 10 pieces
        
        int drops = 0;
        while (drops < 10)
        {
            // wait until move was confirmed
            yield return new WaitUntil(MoveConfirmed);


            // see if a move was detected
            if (!MoveDetected()) {
                // if not - flash screen yellow and continue
                StartCoroutine("FlashYellow");

                continue;
            }

            // see if the drop is valid
            if (TryDrop())
            {
                // if yes
                // - flash screen green
                StartCoroutine("FlashGreen");
                // - drops++
                drops++;
                // - reset AM variables
                Reset_AM_Variables();
            }
            else 
            {
                // if not - flash screen red and continue
                StartCoroutine("FlashRed");
            }

        }
        // end drop phase

        // move phase
        while (!GameOver()) 
        {
            // almost the same just different validation method called
            
            // wait until move was confirmed
            yield return new WaitUntil(MoveConfirmed);


            // see if a move was detected
            if (!MoveDetected())
            {
                // if not - flash screen yellow and continue
                StartCoroutine("FlashYellow");
                
                continue;
            }

            // see if the move is valid
            if (TryMove())
            {
                // if yes
                // - flash screen green
                StartCoroutine("FlashGreen");

                // - reset AM variables
                Reset_AM_Variables();
            }
            else 
            {
                // if not
                // - flash screen red
                StartCoroutine("FlashRed");

                // continue
                continue;
            }
            

        }
        // end move phase
    }
    // end Game Loop coroutine



    // move validation methods
    // 1 - drop phase move (drop)
    private bool TryDrop()
    {

        // check if it isn't a starter tile
        if(!BoardGenerator.IsStarterTile(AM_newX, AM_newY)) return false;

        // check if tile is not unnocupied
        if (!IsUnnocupied(AM_newX, AM_newY)) return false;


        // otherwise it's ok

        // - set piece texture and tag of the corresponding player
        Renderer renderer = AM_piece.GetComponentInChildren<Renderer>();
        if (current_turn == 0)
        {
            AM_piece.tag = "player1";
            renderer.material = p1_material;
        }
        else {
            AM_piece.tag = "player2";
            renderer.material = p2_material;
        }

        // update game state

        // board
        board[AM_newX, AM_newY] = current_turn;

        // turn
        AdvanceTurn();

        // piece board pos
        AM_piece.ConfirmMove(AM_newX, AM_newY);

        return true;
    }

    // 2 - move phase move (move)
    
    private bool TryMove()
    {
        // check if this belongs to the corresponding player
        if (board[AM_oldX, AM_oldY] != current_turn) return false;

        // check if the new spot is not unnocupied
        if (!IsUnnocupied(AM_newX, AM_newY)) return false;

        // validate move
        if (System.Math.Abs(AM_newX - AM_oldX) <= 1 && System.Math.Abs(AM_newY - AM_oldY) <= 1)
        {
            // a move is valid if it moved to an adjacent square in any direction horizontally, vertically or diagonally

            // update game state

            // board
            board[AM_oldX, AM_oldY] = -1;
            board[AM_newX, AM_newY] = current_turn;

            // turn
            AdvanceTurn();

            // piece board pos
            AM_piece.ConfirmMove(AM_newX, AM_newY);

            return true;
        }
        return false;

    }

    // end move validation methods


    // Game State methods
    private bool GameOver() { 
        return (p1_finished_pieces == 5 ||  p2_finished_pieces == 5);
    }
    private bool IsUnnocupied(int x, int y)
    {
        if(x >= 0 && y >= 0 && x < 7 && y < 7)
        {
            return board[x, y] == -1;
        }
        return false;
    }

    // for advancing the game, so that now the next player makes a move
    // easier to know when a turn was advanced
    private void AdvanceTurn() {
        current_turn = (current_turn + 1) % 2;
    }
    // end Game State methods

    // methods for communication with other 

    // whether a piece moved and sent a signal
    
    // 1 - piece
    private bool MoveDetected()
    {
        return AM_piece != null;
    }

    private bool MoveConfirmed() {
        if (move_confirmed) {
            move_confirmed = false;
            return true;
        }
        return false;
    }

    public void SendAttempt(PieceScript piece) {
        if (piece == null) return;
        AM_piece = piece;
        AM_oldX = piece.getOldX();
        AM_oldY = piece.getOldY();
        AM_newX = piece.getNewX();
        AM_newY = piece.getNewY();
    }

    public void Reset_AM_Variables() {
        AM_piece = null;
    }

    // 2 - UI
    public void ConfirmMove()
    {
        move_confirmed = true;
    }

    IEnumerator FlashGreen() 
    {
        // todo - also flash text

        Color color = screen_image.color;
        color.r = 0f;
        color.g = 0.8f;
        color.b = 0f;
        color.a = 0.5f;
        screen_image.color = color;
        yield return new WaitForSeconds(1.5f);
        color.a = 0f;
        screen_image.color = color;
    }

    IEnumerator FlashRed()
    {
        // todo - also flash text

        Color color = screen_image.color;
        color.r = 0.8f;
        color.g = 0f;
        color.b = 0f;
        color.a = 0.5f;
        screen_image.color = color;
        yield return new WaitForSeconds(1.5f);
        color.a = 0f;
        screen_image.color = color;
    }

    IEnumerator FlashYellow()
    {
        // todo - also flash text

        Color color = screen_image.color;
        color.r = 0.8f;
        color.g = 0.8f;
        color.b = 0f;
        color.a = 0.5f;
        screen_image.color = color;
        yield return new WaitForSeconds(1.5f);
        color.a = 0f;
        screen_image.color = color;
    }




    // end methods for communication with other scripts

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // set default value for board
        // each [i, j] == -1 for empty spot/tile
        board = new int[7, 7];
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                board[i, j] = -1;
            }
        }

        // start game loop
        StartCoroutine("GameLoop");
    }

    // Update is called once per frame
    void Update()
    {
        // TODO - input handling
        #if UNITY_EDITOR
            HandleMouseInput();
        #else
            HandleTouchInput();
        #endif
    }


    // input handling
    
    // mouse
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0)) {
            ConfirmMove();
        }
    }

    // touch for mobile
    private void HandleTouchInput()
    {
        // todo - tweak a little bit
        // some synchronization issues maybe 

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        { 
            ConfirmMove();
        }
    }
}

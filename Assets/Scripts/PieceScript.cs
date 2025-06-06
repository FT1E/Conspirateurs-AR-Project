using System.Collections;
using UnityEngine;

public class PieceScript : MonoBehaviour
{

    // board position variables
    private int oldX, oldY;
    private int newX, newY;
    // end board position variables


    // physical position variables
    private Vector3 last_position;
    // end physical position variables

    // variable for communication with GameManagement script
    public GameManagement gameManagement;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // setting default values
        oldX = oldY = newX = newY = -1;
        last_position = transform.position;

        StartCoroutine("OnMoveCoroutine");
    }

    IEnumerator OnMoveCoroutine() 
    {
        while (true) {
            yield return new WaitUntil(Moved);
            
            DetectSquare(); // raycast down to see which tile/spot you're on

            // if newX, newY are -1, i.e. no tile is under the piece, so continue
            if (newX == -1 || newY == -1) continue;

            // else
            // compare oldX, oldY with newX, newY
            if (oldX != newX || oldY != newY) {
                // if the piece was moved to a different tile/square
                // - send signal to GameManagement
                gameManagement.SendAttempt(this);

                // if move is ok, GameManagement will call ConfirmMove()
            }

        }
    }

    private bool Moved()
    {
        Vector3 new_position = transform.position;
        if (last_position != new_position) {
            last_position = new_position;
            return true;
        }
        return false;
    }


    private void DetectSquare() {

        // 1) Raycast down and see if it's a tile under the piece
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag("board_tile"))
        {
            // get its x, y
            // set newX, newY
            SpotScript spotScript = hit.collider.GetComponent<SpotScript>();
            newX = spotScript.GetX();
            newY = spotScript.GetY();
        }
        else 
        {
            // no tile under
            newX = newY = -1;
        }



    }

    public void ConfirmMove(int x, int y) {
        // passing parameters, in case piece is moved while move is being confirmed
        oldX = newX = x;
        oldY = newY = y;
    }

    public int getOldX() { return oldX; }
    public int getOldY() {  return oldY; }
    public int getNewX() {  return newX; }
    public int getNewY() {  return newY; }

    // Update is called once per frame
    void Update()
    {
        
    }
}

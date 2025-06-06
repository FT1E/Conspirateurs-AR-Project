using UnityEngine;

public class SpotScript : MonoBehaviour
{

    public int x, y;
    public void Init(int x, int y) {
        this.x = x;
        this.y = y;
    }


    public int GetX() { return x; }
    public int GetY() { return y; }

}

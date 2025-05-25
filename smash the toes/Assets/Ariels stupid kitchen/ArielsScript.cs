using UnityEngine;

public class ArielsScript : MonoBehaviour
{
    public int BinCount;

    public bool shmoogle = false;

    void Update()
    {
        kringlekrongle();
    }

    void kringlekrongle()
    {
        if (BinCount >= 5)
            shmoogle = true;

        if (shmoogle == true)
            Debug.Log("sugondeeznuts");
    }
}
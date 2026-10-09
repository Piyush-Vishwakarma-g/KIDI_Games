using UnityEngine;

public class MazeTraverseManager : MonoBehaviour
{
    public static MazeTraverseManager instance;

    public int mazeLevelIndex;
    public bool isblackBall=false;
    public bool istravel=false;
    public bool isheadPhone=false;
    public bool isbedsheet=false;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if(instance==null)
        {
            instance =this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using UnityEngine;

public class SystemEvents
{
    public void Init()
    {
        if ( Application.platform == RuntimePlatform.Android )
        {
            if ( Input.GetKeyDown( KeyCode.Escape ) )
            {
                Application.Quit();
            }
        }
    }
}

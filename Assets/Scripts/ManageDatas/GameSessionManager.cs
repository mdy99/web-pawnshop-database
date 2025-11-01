using UnityEngine;
using System.Collections.Generic;

public class GameSessionManager : MonoBehaviour
{
    public static GameSessionData currentGameSessionData;

    public static void InitializeGameSession(GameSessionData sessionData)
    {
        currentGameSessionData = sessionData;
    }
}

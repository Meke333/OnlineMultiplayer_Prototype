using System;

[Serializable]
public class UserData
{
    public string userName;
    public string userAuthID;
    public GameInfo userPreferrredGamePreferences = new GameInfo();
    //public string password;
    //public string email;
}

[Serializable]
public class GameInfo
{
    public MapType map;
    public GameModeType gameMode;
    public GameQueueType gameQueue;
    
    //These names/types (whatever you need) var should be the same as the ones set up in the multiplay dashboard on Unity
    private const string queueNameSolo = "Solo-Player";
    private const string queueNameTeam = "Team-Player";
    

    public string ToMultiplayQueue()
    {
        return gameQueue switch
        {
            GameQueueType.Solo => queueNameSolo,
            GameQueueType.Team => queueNameTeam,
            _ => queueNameSolo
        };
    }
    
}

#region Addtional
public enum MapType
{
    Default
}

public enum GameModeType
{
    Default
}

public enum GameQueueType
{
    Solo,
    Team
}

#endregion
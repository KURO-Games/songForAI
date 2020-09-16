using System;
using System.Collections.Generic;

[Serializable]
public class ResnponseGetRanking
{
    public List<Ranking> rankings;
}

[Serializable]
public class Ranking
{
    public int id;
    public string game_id;
    public int rank;
    public string name;
    public string score;
    public string comment;
    public DateTime created_at;
    public DateTime updated_at;
}
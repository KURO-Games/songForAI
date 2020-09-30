using System;
using System.Collections.Generic;

/// <summary>
/// ランキングデータ(一覧)
/// </summary>
[Serializable]
public class ResnponseGetRanking
{
    public List<Ranking> rankings;
}

/// <summary>
/// ランキングデータ(クラス)
/// </summary>
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
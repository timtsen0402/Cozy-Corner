using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RankingSystem : MonoBehaviour
{
    public class PlayerRank
    {
        public int PlayerId { get; set; }
        public float Score { get; set; }
        public int Rank { get; set; }
    }

    private List<PlayerRank> playerRanks = new List<PlayerRank>();
    private bool gameEnded = false;

    public void AddPlayer(int playerId)
    {
        playerRanks.Add(new PlayerRank { PlayerId = playerId, Score = 0, Rank = 0 });
    }

    public void UpdatePlayerScore(int playerId, float score)
    {
        var player = playerRanks.Find(p => p.PlayerId == playerId);
        if (player != null)
        {
            player.Score = score;
            UpdateRankings();
        }
    }

    private void UpdateRankings()
    {
        var sortedPlayers = playerRanks.OrderByDescending(p => p.Score).ToList();

        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            sortedPlayers[i].Rank = i + 1;
        }

        if (sortedPlayers.Count >= 3 && sortedPlayers[2].Rank == 3 && !gameEnded)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        gameEnded = true;
        Debug.Log("遊戲結束！前三名已經確定。");
        DisplayFinalRankings();
    }

    public void DisplayFinalRankings()
    {
        var topPlayers = playerRanks.Where(p => p.Rank <= 4).OrderBy(p => p.Rank).ToList();

        foreach (var player in topPlayers)
        {
            Debug.Log($"第 {player.Rank} 名: 玩家 {player.PlayerId}, 分數: {player.Score}");
        }
    }

    public bool IsGameEnded()
    {
        return gameEnded;
    }
}
using GlideNGlow.Common.Enums;
using GlideNGlow.Core.Models;

namespace GlideNGlow.Core.Services.Generic;

public class EntryComparer : IComparer<Entry>
{
    public int Compare(Entry? x, Entry? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (ReferenceEquals(null, y)) return 1;
        if (ReferenceEquals(null, x)) return -1;

        if (x.Game == null || y.Game == null) throw new NullReferenceException("When sorting Entries Game should be included!");
        if (x.GameId != y.GameId) throw new Exception("Be sure to group the entries by game before ordering!");

        var result = x.Score.Contains(':')
            ? TimeSpan.ParseExact(x.Score, "g", null).CompareTo(TimeSpan.ParseExact(y.Score, "g", null))
            : float.Parse(x.Score).CompareTo(float.Parse(y.Score));

        if (x.Game.ScoreImportance == ScoreImportance.Highest) result *= -1;

        return result;
    }
}
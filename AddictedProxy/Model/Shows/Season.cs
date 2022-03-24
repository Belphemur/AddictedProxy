using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Model.Shows;

[Index(nameof(TvShowId), nameof(Number), IsUnique = true)]
public class Season : IEquatable<Season>
{
    public int Id { get; set; }
    public int TvShowId { get; set; }

    [ForeignKey(nameof(TvShowId))]
    public virtual TvShow TvShow { get; set; }
    /// <summary>
    /// Number associated with the season
    /// </summary>
    public int Number { get; set; }

    public bool Equals(Season? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return TvShowId == other.TvShowId && Number == other.Number;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Season)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(TvShowId, Number);
    }

    public static bool operator ==(Season? left, Season? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Season? left, Season? right)
    {
        return !Equals(left, right);
    }
}
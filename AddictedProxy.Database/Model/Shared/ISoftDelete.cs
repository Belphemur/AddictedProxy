namespace AddictedProxy.Database.Model.Shared;

public interface ISoftDelete
{
    DateTime? DeletedAt { get; set; }
}
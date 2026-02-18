using AddictedProxy.Database.Context;
using AddictedProxy.Database.Model.State;
using Microsoft.EntityFrameworkCore;

namespace AddictedProxy.Database.Repositories.State;

public class SuperSubtitlesStateRepository : ISuperSubtitlesStateRepository
{
    private readonly EntityContext _context;

    public SuperSubtitlesStateRepository(EntityContext context)
    {
        _context = context;
    }

    public async Task<long> GetMaxSubtitleIdAsync(CancellationToken token)
    {
        var state = await _context.SuperSubtitlesStates
            .FirstOrDefaultAsync(token);
        return state?.MaxSubtitleId ?? 0;
    }

    public async Task SetMaxSubtitleIdAsync(long maxSubtitleId, CancellationToken token)
    {
        var state = await _context.SuperSubtitlesStates
            .FirstOrDefaultAsync(token);

        if (state == null)
        {
            state = new SuperSubtitlesState { MaxSubtitleId = maxSubtitleId };
            _context.SuperSubtitlesStates.Add(state);
        }
        else
        {
            state.MaxSubtitleId = maxSubtitleId;
        }

        await _context.SaveChangesAsync(token);
    }
}

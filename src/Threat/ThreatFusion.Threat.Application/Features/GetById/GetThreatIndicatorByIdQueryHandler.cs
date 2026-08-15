using MediatR;
using Microsoft.EntityFrameworkCore;
using ThreatFusion.Threat.Application.Abstractions;

namespace ThreatFusion.Threat.Application.Features.ThreatIndicators.GetById;


public sealed class GetThreatIndicatorByIdQueryHandler
    : IRequestHandler<
        GetThreatIndicatorByIdQuery,
        ThreatIndicatorDetailsDto?>
{
    private readonly IThreatDbContext _dbContext;


    public GetThreatIndicatorByIdQueryHandler(
        IThreatDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<ThreatIndicatorDetailsDto?> Handle(
        GetThreatIndicatorByIdQuery request,
        CancellationToken cancellationToken)
    {
        var indicator =
            await _dbContext.ThreatIndicators
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == request.Id,
                    cancellationToken);


        if (indicator == null)
        {
            return null;
        }


        return new ThreatIndicatorDetailsDto(
            indicator.Id,
            indicator.Type.ToString(),
            indicator.Value,
            indicator.Severity.ToString(),
            indicator.Confidence,
            indicator.SourceName,
            indicator.Description,
            indicator.FirstSeenUtc,
            indicator.LastSeenUtc,
            indicator.CvssScore,
            indicator.CvssVersion,
            indicator.CvssVector,
            indicator.CweId,
            indicator.ReferenceUrl,
            indicator.IsActive);
    }
}
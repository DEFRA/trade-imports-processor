using Defra.TradeImportsDataApi.Domain.Gvms;
using FluentValidation;

namespace Defra.TradeImportsProcessor.Processor.Validation.Gmrs;

public class GmrValidator : AbstractValidator<Gmr>
{
    public GmrValidator()
    {
        RuleFor(g => g.Id).NotEmpty();
        RuleFor(g => g.HaulierEori).NotEmpty();
        RuleFor(g => g.UpdatedSource).NotEmpty();
        RuleFor(g => g.Direction).NotEmpty();

        RuleFor(g => g.ActualCrossing).Must(HaveAnActualCrossingRouteId);
        RuleFor(g => g.CheckedInCrossing).Must(HaveACheckedInCrossingRouteId);
        RuleFor(g => g.PlannedCrossing).Must(HaveAPlannedCrossingRouteId);
    }

    private static bool HaveAnActualCrossingRouteId(Gmr gmr, ActualCrossing? actualCrossing)
    {
        return actualCrossing is null || actualCrossing.RouteId is not null;
    }

    private static bool HaveACheckedInCrossingRouteId(Gmr gmr, CheckedInCrossing? checkedInCrossing)
    {
        return checkedInCrossing is null || checkedInCrossing.RouteId is not null;
    }

    private static bool HaveAPlannedCrossingRouteId(Gmr gmr, PlannedCrossing? plannedCrossing)
    {
        return plannedCrossing is null || plannedCrossing.RouteId is not null;
    }
}

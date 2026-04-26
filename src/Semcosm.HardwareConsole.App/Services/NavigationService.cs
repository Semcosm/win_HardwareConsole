using System;
using Microsoft.UI.Xaml.Controls;
using Semcosm.HardwareConsole.Abstractions;

namespace Semcosm.HardwareConsole.App.Services;

public sealed class NavigationService : INavigationService
{
    private readonly IDiagnosticsSink _diagnosticsSink;
    private readonly IPageFactory _pageFactory;
    private Frame? _frame;

    public NavigationService(
        IPageFactory pageFactory,
        IDiagnosticsSink diagnosticsSink)
    {
        _pageFactory = pageFactory;
        _diagnosticsSink = diagnosticsSink;
    }

    public void Initialize(Frame frame)
    {
        _frame = frame;
    }

    public bool Navigate(string route)
    {
        if (_frame is null)
        {
            _diagnosticsSink.Report(new DiagnosticRecord(
                DiagnosticSeverity.Error,
                DiagnosticSource.Routes,
                "routes.frame_uninitialized",
                $"Navigation requested for route '{route}' before the shell frame was initialized.",
                route,
                DateTimeOffset.UtcNow));
            return false;
        }

        var pageType = _pageFactory.ResolvePageType(route);
        if (pageType is null)
        {
            _diagnosticsSink.Report(new DiagnosticRecord(
                DiagnosticSeverity.Error,
                DiagnosticSource.Routes,
                "routes.unresolved_route",
                $"Navigation route '{route}' could not be resolved to a page type.",
                route,
                DateTimeOffset.UtcNow));
            return false;
        }

        if (_frame.CurrentSourcePageType == pageType)
        {
            return true;
        }

        var navigateResult = _frame.Navigate(pageType);
        if (!navigateResult)
        {
            _diagnosticsSink.Report(new DiagnosticRecord(
                DiagnosticSeverity.Error,
                DiagnosticSource.Routes,
                "routes.navigate_failed",
                $"Navigation to route '{route}' failed when the frame attempted to load '{pageType.Name}'.",
                route,
                DateTimeOffset.UtcNow));
        }

        return navigateResult;
    }
}

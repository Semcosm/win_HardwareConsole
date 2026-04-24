using System;

namespace Semcosm.HardwareConsole.App.Services;

public interface IPageFactory
{
    Type? ResolvePageType(string route);
}

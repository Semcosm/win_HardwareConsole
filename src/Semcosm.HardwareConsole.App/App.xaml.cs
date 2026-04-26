using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Semcosm.HardwareConsole.Abstractions;
using Semcosm.HardwareConsole.App.Services;
using Semcosm.HardwareConsole.App.ViewModels;
using Semcosm.HardwareConsole.Mock.Services;
using System;

namespace Semcosm.HardwareConsole.App
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;
        private Window? _window;

        public App()
        {
            InitializeComponent();

            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        public static T GetService<T>() where T : notnull
        {
            return ((App)Current)._serviceProvider.GetRequiredService<T>();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<DiagnosticsStore>();
            services.AddSingleton<IDiagnosticsSink>(serviceProvider => serviceProvider.GetRequiredService<DiagnosticsStore>());
            services.AddSingleton<IDiagnosticsProvider>(serviceProvider => serviceProvider.GetRequiredService<DiagnosticsStore>());
            services.AddSingleton<IHardwareInventoryService, MockHardwareInventoryService>();
            services.AddSingleton<IPluginRegistry, MockPluginRegistry>();
            services.AddSingleton<IProfileRuntimeService, MockProfileRuntimeService>();
            services.AddSingleton<IPolicyValidator<PowerPolicyDescriptor, PowerPolicyValidationResult>, MockPowerPolicyValidator>();
            services.AddSingleton<IPolicyValidator<SchedulerPolicyDescriptor, SchedulerPolicyValidationResult>, MockSchedulerPolicyValidator>();
            services.AddSingleton<IPolicyValidator<ThermalPolicyDescriptor, ThermalPolicyValidationResult>, MockThermalPolicyValidator>();
            services.AddSingleton<MockPolicyRuntimeService>();
            services.AddSingleton<IFanPolicyRuntimeService>(serviceProvider => serviceProvider.GetRequiredService<MockPolicyRuntimeService>());
            services.AddSingleton<IPowerPolicyRuntimeService>(serviceProvider => serviceProvider.GetRequiredService<MockPolicyRuntimeService>());
            services.AddSingleton<ISchedulerPolicyRuntimeService>(serviceProvider => serviceProvider.GetRequiredService<MockPolicyRuntimeService>());
            services.AddSingleton<IThermalPolicyRuntimeService>(serviceProvider => serviceProvider.GetRequiredService<MockPolicyRuntimeService>());
            services.AddSingleton<ISensorSnapshotProvider, MockSensorSnapshotProvider>();
            services.AddSingleton<PluginDiagnosticsReporter>();
            services.AddSingleton<INavigationRouteProvider, BuiltInNavigationRouteProvider>();
            services.AddSingleton<INavigationRouteRegistry, CompositeNavigationRouteRegistry>();
            services.AddSingleton<IRouteContentFactory, BuiltInPageRouteContentFactory>();
            services.AddSingleton<IPageFactory, PageFactory>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<DevicePresentationMapper>();
            services.AddSingleton<FanPolicyPresentationMapper>();
            services.AddSingleton<PowerPolicyPresentationMapper>();
            services.AddSingleton<ProfilePresentationMapper>();
            services.AddSingleton<SchedulerPolicyPresentationMapper>();
            services.AddSingleton<ThermalPolicyPresentationMapper>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<DevicesViewModel>();
            services.AddTransient<DiagnosticsViewModel>();
            services.AddTransient<FansViewModel>();
            services.AddTransient<PowerViewModel>();
            services.AddTransient<ProfilesViewModel>();
            services.AddTransient<PluginsViewModel>();
            services.AddTransient<SchedulerViewModel>();
            services.AddTransient<ThermalViewModel>();
            services.AddTransient<MainWindow>();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            GetService<PluginDiagnosticsReporter>().PublishSnapshot();
            _window = GetService<MainWindow>();
            _window.Activate();
        }
    }
}

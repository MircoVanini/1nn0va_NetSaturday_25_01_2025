using MediatR;
using NativeSceneAutomation.Board.LedStrip;
using NativeSceneAutomation.Models;
using NativeSceneAutomation.Notifications;

namespace NativeSceneAutomation.Board
{
    public class HWService : IHWService
    {
        private IServiceProvider _serviceProvider;
        private ILogger<HWService> _logger;
        private IConfiguration _configuration;

        private LedController? _ledController;
        private MotorController? _motorController;
        private ProximityController? _proximityController;
        private IMediator _mediator;

        private CancellationTokenSource? _ctsProximity;
        private CancellationTokenSource? _ctsRotation;
        private CancellationTokenSource? _ctsRotationAngle;
        private CancellationTokenSource? _ctsDayLight;
        private CancellationTokenSource? _ctsNigthLight;


        public HWService(IServiceProvider serviceProvider, ILogger<HWService> logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger          = logger;
            _configuration   = configuration;
            _mediator        = serviceProvider.GetRequiredService<IMediator>();

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                return;

            _ledController       = new LedController();
            _motorController     = new MotorController();
            _proximityController = new ProximityController();

            _ctsProximity     = new CancellationTokenSource();
            _ctsRotation      = new CancellationTokenSource();
            _ctsDayLight      = new CancellationTokenSource();
            _ctsNigthLight    = new CancellationTokenSource();
            _ctsRotationAngle = new CancellationTokenSource();

            _proximityController.ProximityChanged += OnProximityChanged;
        }

            public Task InitializeAsync()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                return Task.CompletedTask;

            _ledController?.Initialize();
            _motorController?.Initialize();
            _proximityController?.Initialize();

            return Task.CompletedTask;
        }

        public async Task StartAsync()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                return;

            _ctsProximity?.Dispose();
            _ctsProximity = new CancellationTokenSource();
            await _proximityController!.StartReadAsync(_ctsProximity.Token);
        }

        public async Task StopAsync()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                return;

            StopAllRotations();
            StopAllLedTransitions();

            _ctsProximity?.Cancel();
            await Task.Delay(100);
        }

        public Task StartAngleRotateAsync(int angle)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                return Task.CompletedTask;

            StopAllRotations();

            _ctsRotationAngle?.Dispose();
            _ctsRotationAngle = new CancellationTokenSource();

            return _motorController!.RotateAngleClockwiseAsync(angle, _ctsRotationAngle.Token);
        }

        public Task StartRotateAsync(int duration, bool clockwise)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                return Task.CompletedTask;

            StopAllRotations();

            _ctsRotation?.Dispose();
            _ctsRotation = new CancellationTokenSource();

            if (!clockwise)
                return _motorController!.RotateAntiClockwiseAsync(duration, _ctsRotation.Token);
            else
                return _motorController!.RotateClockwiseAsync(duration, _ctsRotation.Token);
        }

        public async Task StopRotateAsync()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                return;
            
            StopAllRotations();

            await Task.Delay(100);
        }

        public async Task DayPixelsColorsTransitionAsync(int duration)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                return;

            StopAllLedTransitions();

            _ctsDayLight?.Dispose();
            _ctsDayLight = new CancellationTokenSource();

            await _ledController!.StartDayPixelsColorsAsync(duration, _ctsDayLight.Token);
        }

        public async Task NightPixelsColorsTransitionAsync(int duration)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                return;

            StopAllLedTransitions();

            _ctsNigthLight?.Dispose();
            _ctsNigthLight = new CancellationTokenSource();

            await _ledController!.StartNightPixelsColorsAsync(duration, _ctsNigthLight.Token);
        }

        public async Task TurnOffPixelsAsync()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                return;

            StopAllLedTransitions();

            await _ledController!.StartSwitchOffLedsAsync();
        }

        private void OnProximityChanged(object? sender, ProximityArgs e)
        {
            _logger.LogInformation($"Proximity Changed: {e.OnRange}");
            _mediator.Publish(new ProximityNotification(e.OnRange));
        }

        private void StopAllRotations()
        {
            try
            {
                if (_ctsRotationAngle?.IsCancellationRequested == false)
                    _ctsRotationAngle?.Cancel();

                if (_ctsRotation?.IsCancellationRequested == false)
                    _ctsRotation?.Cancel();
            }
            catch {}
        }

        private void StopAllLedTransitions()
        {
            try
            {
                if (_ctsDayLight?.IsCancellationRequested == false)
                    _ctsDayLight?.Cancel();

                if (_ctsNigthLight?.IsCancellationRequested == false)
                    _ctsNigthLight?.Cancel();
            }
            catch { }
        }
    }
}

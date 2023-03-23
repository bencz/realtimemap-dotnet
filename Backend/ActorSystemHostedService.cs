namespace Backend
{
    public class ActorSystemHostedService : IHostedService
    {
        private readonly ActorSystem _actorSystem;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly ILogger<ActorSystemHostedService> _logger;

        public ActorSystemHostedService(
            ActorSystem actorSystem,
            IHostApplicationLifetime hostApplicationLifetime,
            ILogger<ActorSystemHostedService> logger)
        {
            _actorSystem = actorSystem;
            _hostApplicationLifetime = hostApplicationLifetime;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _hostApplicationLifetime.ApplicationStarted.Register(OnStarted);
            _hostApplicationLifetime.ApplicationStopping.Register(OnStopping);
            
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;

        private void OnStarted()
        {
            Task.Run(async () =>
            {
                _logger.LogInformation("Starting Proto actor system");
            
                await _actorSystem
                    .Cluster()
                    .StartMemberAsync();
            }).GetAwaiter().GetResult();
        }
    
        private void OnStopping()
        {
            Task.Run(async () =>
            {
                _logger.LogInformation("Stopping Proto actor system");

                await _actorSystem
                    .Cluster()
                    .ShutdownAsync();
            }).GetAwaiter().GetResult();
        }
    }
}
using AuditServer.Services.AuthService;

namespace AuditServer.Services.BackgroundServices
{
    public class TokenCleanupHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<TokenCleanupHostedService> _logger;

        // المنطقة الزمنية لمصر
        private static readonly TimeZoneInfo EgyptTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById(OperatingSystem.IsWindows() ? "Egypt Standard Time" : "Africa/Cairo");

        public TokenCleanupHostedService(
            IServiceScopeFactory scopeFactory,
            ILogger<TokenCleanupHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var nowEgypt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, EgyptTimeZone);
            _logger.LogInformation("بدأت خدمة تنظيف التوكنز في الخلفية. التوقيت الحالي بمصر: {EgyptTime}", nowEgypt.ToString("yyyy-MM-dd HH:mm:ss"));

            while (!stoppingToken.IsCancellationRequested)
            {
                // 1. حساب الوقت المتبقي حتى منتصف الليل التالي (12:00 AM) بتوقيت مصر
                var delay = GetDelayUntilNextTargetTime(12, 0); // 12:00 AM (00:00)

                _logger.LogInformation("العملية القادمة مجدولة بعد: {Hours} ساعة و {Minutes} دقيقة.", delay.Hours, delay.Minutes);

                // 2. الانتظار حتى حلول الموعد أو إيقاف التطبيق
                await Task.Delay(delay, stoppingToken);

                // 3. التنفيذ عند حلول الساعة 12:00 AM
                try
                {
                    var executionTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, EgyptTimeZone);
                    _logger.LogInformation("جاري تنفيذ عملية التنظيف... التوقيت الحالي بمصر: {EgyptTime}", executionTime.ToString("yyyy-MM-dd HH:mm:ss"));

                    using var scope = _scopeFactory.CreateScope();
                    var authRepo = scope.ServiceProvider.GetRequiredService<IAuthRepo>();

                    int deletedCount = await authRepo.RevokeExpiredRefreshTokensAsync();

                    _logger.LogInformation("تمت عملية التنظيف بنجاح. عدد التوكنز الحذوفة: {Count}", deletedCount);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "حدث خطأ أثناء تنفيذ خدمة تنظيف الـ Refresh Tokens.");
                }
            }
        }

        // دالة حساب التأخير المتبقي بالظبط حتى موعد التنفيذ القادم
        private TimeSpan GetDelayUntilNextTargetTime(int targetHour, int targetMinute)
        {
            var nowEgypt = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, EgyptTimeZone);

            // تاريخ اليوم الساعة 12:00 AM (منتصف الليل)
            var nextRun = nowEgypt.Date.AddHours(targetHour == 12 ? 0 : targetHour).AddMinutes(targetMinute);

            // إذا كان الوقت الحالي تجاوز 12:00 AM لليوم، نجدول لليوم التالي
            if (nowEgypt >= nextRun)
            {
                nextRun = nextRun.AddDays(1);
            }

            return nextRun - nowEgypt;
        }
    }
}
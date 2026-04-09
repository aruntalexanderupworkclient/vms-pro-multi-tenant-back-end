namespace VMS.Shared.Helpers;

public static class QrCodeGenerator
{
    public static string GenerateQrCodeString()
    {
        return $"VMS-{Guid.NewGuid():N}".ToUpperInvariant();
    }

    public static string GenerateQrCodeString(Guid visitId)
    {
        return $"VMS-{visitId:N}".ToUpperInvariant();
    }
}

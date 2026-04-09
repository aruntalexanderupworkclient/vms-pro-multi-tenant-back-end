using VMS.Application.Interfaces;

namespace VMS.Infrastructure.Services;

public class TenantProvider : ITenantProvider
{
    private Guid _tenantId = Guid.Empty;

    public Guid GetTenantId() => _tenantId;

    public void SetTenantId(Guid tenantId)
    {
        _tenantId = tenantId;
    }
}

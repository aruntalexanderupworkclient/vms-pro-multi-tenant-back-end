namespace VMS.Shared.Constants;

public static class AppConstants
{
    public static class MdmCodes
    {
        public const string TenantType = "TENANT_TYPE";
        public const string PlanType = "PLAN_TYPE";
        public const string LocationType = "LOCATION_TYPE";
        public const string FileType = "FILE_TYPE";
        public const string EntityType = "ENTITY_TYPE";
    }

    public static class MdmValueCodes
    {
        // Tenant Types
        public const string Enterprise = "ENTERPRISE";
        public const string SMB = "SMB";

        // Plan Types
        public const string Basic = "BASIC";
        public const string Pro = "PRO";

        // Location Types
        public const string Tower = "TOWER";
        public const string Floor = "FLOOR";
        public const string Room = "ROOM";
        public const string Department = "DEPARTMENT";

        // File Types
        public const string Pdf = "PDF";
        public const string Image = "IMAGE";
        public const string Doc = "DOC";

        // Entity Types
        public const string VisitorEntity = "VISITOR";
        public const string UserEntity = "USER";
        public const string HostEntity = "HOST";
    }

    public static class Roles
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string FrontDesk = "FrontDesk";
        public const string Security = "Security";
    }

    public static class ClaimTypes
    {
        public const string TenantId = "tenantId";
        public const string RoleId = "roleId";
        public const string IsAdmin = "isAdmin";
        public const string UserId = "userId";
    }
}

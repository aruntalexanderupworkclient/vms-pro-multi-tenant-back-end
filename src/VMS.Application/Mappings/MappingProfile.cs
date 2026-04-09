using AutoMapper;
using VMS.Application.DTOs.Documents;
using VMS.Application.DTOs.Hosts;
using VMS.Application.DTOs.Locations;
using VMS.Application.DTOs.Mdm;
using VMS.Application.DTOs.Menus;
using VMS.Application.DTOs.Notifications;
using VMS.Application.DTOs.Roles;
using VMS.Application.DTOs.Users;
using VMS.Application.DTOs.Visitors;
using VMS.Core.Entities;

namespace VMS.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User
        CreateMap<User, UserDto>()
            .ForMember(d => d.RoleName, o => o.MapFrom(s => s.Role != null ? s.Role.Name : null));
        CreateMap<CreateUserDto, User>();

        // Visitor
        CreateMap<Visitor, VisitorDto>();
        CreateMap<CreateVisitorDto, Visitor>();

        // HostPerson
        CreateMap<HostPerson, HostPersonDto>()
            .ForMember(d => d.LocationName, o => o.MapFrom(s => s.Location != null ? s.Location.Name : null));
        CreateMap<CreateHostPersonDto, HostPerson>();

        // Visit
        CreateMap<Visit, VisitDto>()
            .ForMember(d => d.VisitorName, o => o.MapFrom(s => s.Visitor != null ? s.Visitor.FullName : null))
            .ForMember(d => d.HostName, o => o.MapFrom(s => s.Host != null ? s.Host.FullName : null))
            .ForMember(d => d.LocationName, o => o.MapFrom(s => s.Location != null ? s.Location.Name : null));
        CreateMap<CreateVisitDto, Visit>();

        // Location
        CreateMap<Location, LocationDto>()
            .ForMember(d => d.TypeName, o => o.MapFrom(s => s.Type != null ? s.Type.Name : null))
            .ForMember(d => d.ParentName, o => o.MapFrom(s => s.Parent != null ? s.Parent.Name : null))
            .ForMember(d => d.Children, o => o.MapFrom(s => s.Children));
        CreateMap<CreateLocationDto, Location>();

        // Role
        CreateMap<Role, RoleDto>()
            .ForMember(d => d.Permissions, o => o.MapFrom(s => s.RolePermissions));
        CreateMap<CreateRoleDto, Role>();
        CreateMap<RolePermission, RolePermissionDto>()
            .ForMember(d => d.MenuName, o => o.MapFrom(s => s.Menu != null ? s.Menu.Name : null));

        // Menu
        CreateMap<Menu, MenuDto>()
            .ForMember(d => d.Children, o => o.MapFrom(s => s.Children.Where(c => c.IsActive)));
        CreateMap<CreateMenuDto, Menu>();

        // MDM
        CreateMap<MdmTenantType, MdmItemDto>();
        CreateMap<MdmPlanType, MdmItemDto>();
        CreateMap<MdmLocationType, MdmItemDto>();
        CreateMap<MdmFileType, MdmItemDto>();
        CreateMap<MdmEntityType, MdmItemDto>();

        // Document
        CreateMap<Document, DocumentDto>()
            .ForMember(d => d.EntityTypeName, o => o.MapFrom(s => s.EntityType != null ? s.EntityType.Name : null))
            .ForMember(d => d.FileTypeName, o => o.MapFrom(s => s.FileType != null ? s.FileType.Name : null));

        // Notification
        CreateMap<Notification, NotificationDto>();
        CreateMap<CreateNotificationDto, Notification>();
    }
}

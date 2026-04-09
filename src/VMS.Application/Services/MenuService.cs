using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VMS.Application.DTOs.Common;
using VMS.Application.DTOs.Menus;
using VMS.Application.Interfaces;
using VMS.Core.Entities;
using VMS.Core.Interfaces;

namespace VMS.Application.Services;

public class MenuService : IMenuService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public MenuService(IUnitOfWork uow, IMapper mapper, ICurrentUserService currentUserService)
    {
        _uow = uow;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<ApiResponse<List<MenuDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var menus = await _uow.Repository<Menu>().Query()
            .Where(m => m.IsActive)
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync(cancellationToken);

        return ApiResponse<List<MenuDto>>.SuccessResponse(_mapper.Map<List<MenuDto>>(menus));
    }

    public async Task<ApiResponse<List<MenuDto>>> GetTreeAsync(CancellationToken cancellationToken = default)
    {
        var menus = await _uow.Repository<Menu>().Query()
            .Include(m => m.Children.Where(c => !c.IsDeleted))
            .Where(m => m.ParentId == null && m.IsActive)
            .OrderBy(m => m.DisplayOrder)
            .ToListAsync(cancellationToken);

        return ApiResponse<List<MenuDto>>.SuccessResponse(_mapper.Map<List<MenuDto>>(menus));
    }

    public async Task<ApiResponse<List<MenuWithPermissionDto>>> GetMenusByRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var permissions = await _uow.Repository<RolePermission>().Query()
            .Include(rp => rp.Menu)
            .Where(rp => rp.RoleId == roleId && rp.Menu != null && rp.Menu.IsActive)
            .OrderBy(rp => rp.Menu!.DisplayOrder)
            .ToListAsync(cancellationToken);

        var menuDtos = permissions.Select(rp => new MenuWithPermissionDto
        {
            MenuId = rp.MenuId,
            MenuName = rp.Menu?.Name ?? string.Empty,
            Icon = rp.Menu?.Icon,
            Route = rp.Menu?.Route,
            ParentId = rp.Menu?.ParentId,
            DisplayOrder = rp.Menu?.DisplayOrder ?? 0,
            CanCreate = rp.CanCreate,
            CanRead = rp.CanRead,
            CanUpdate = rp.CanUpdate,
            CanDelete = rp.CanDelete,
            CanPrint = rp.CanPrint
        }).ToList();

        return ApiResponse<List<MenuWithPermissionDto>>.SuccessResponse(BuildMenuTree(menuDtos));
    }

    public async Task<ApiResponse<List<MenuWithPermissionDto>>> GetMyMenusAsync(CancellationToken cancellationToken = default)
    {
        var roleId = _currentUserService.RoleId;
        var isAdmin = _currentUserService.IsAdmin;

        if (isAdmin)
        {
            var allMenus = await _uow.Repository<Menu>().Query()
                .Where(m => m.IsActive)
                .OrderBy(m => m.DisplayOrder)
                .ToListAsync(cancellationToken);

            var adminDtos = allMenus.Select(m => new MenuWithPermissionDto
            {
                MenuId = m.Id,
                MenuName = m.Name,
                Icon = m.Icon,
                Route = m.Route,
                ParentId = m.ParentId,
                DisplayOrder = m.DisplayOrder,
                CanCreate = true,
                CanRead = true,
                CanUpdate = true,
                CanDelete = true,
                CanPrint = true
            }).ToList();

            return ApiResponse<List<MenuWithPermissionDto>>.SuccessResponse(BuildMenuTree(adminDtos));
        }

        if (!roleId.HasValue)
            return ApiResponse<List<MenuWithPermissionDto>>.SuccessResponse(new List<MenuWithPermissionDto>());

        return await GetMenusByRoleAsync(roleId.Value, cancellationToken);
    }

    public async Task<ApiResponse<MenuDto>> CreateAsync(CreateMenuDto dto, CancellationToken cancellationToken = default)
    {
        var menu = _mapper.Map<Menu>(dto);
        await _uow.Repository<Menu>().AddAsync(menu, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return ApiResponse<MenuDto>.SuccessResponse(_mapper.Map<MenuDto>(menu), "Menu created successfully.");
    }

    public async Task<ApiResponse<MenuDto>> UpdateAsync(Guid id, UpdateMenuDto dto, CancellationToken cancellationToken = default)
    {
        var menu = await _uow.Repository<Menu>().GetByIdAsync(id, cancellationToken);
        if (menu == null)
            return ApiResponse<MenuDto>.FailResponse("Menu not found.", "MENU_NOT_FOUND");

        if (dto.Name != null) menu.Name = dto.Name;
        if (dto.Icon != null) menu.Icon = dto.Icon;
        if (dto.Route != null) menu.Route = dto.Route;
        if (dto.ParentId.HasValue) menu.ParentId = dto.ParentId;
        if (dto.DisplayOrder.HasValue) menu.DisplayOrder = dto.DisplayOrder.Value;
        if (dto.IsActive.HasValue) menu.IsActive = dto.IsActive.Value;

        _uow.Repository<Menu>().Update(menu);
        await _uow.SaveChangesAsync(cancellationToken);

        return ApiResponse<MenuDto>.SuccessResponse(_mapper.Map<MenuDto>(menu), "Menu updated successfully.");
    }

    public async Task<ApiResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var menu = await _uow.Repository<Menu>().GetByIdAsync(id, cancellationToken);
        if (menu == null)
            return ApiResponse.FailResponse("Menu not found.", "MENU_NOT_FOUND");

        var hasChildren = await _uow.Repository<Menu>().AnyAsync(m => m.ParentId == id, cancellationToken);
        if (hasChildren)
            return ApiResponse.FailResponse("Cannot delete menu with child items.", "MENU_HAS_CHILDREN");

        _uow.Repository<Menu>().SoftDelete(menu);
        await _uow.SaveChangesAsync(cancellationToken);

        return ApiResponse.SuccessResponse("Menu deleted successfully.");
    }

    private static List<MenuWithPermissionDto> BuildMenuTree(List<MenuWithPermissionDto> flatList)
    {
        var lookup = flatList.ToDictionary(m => m.MenuId);
        var roots = new List<MenuWithPermissionDto>();

        foreach (var item in flatList)
        {
            if (item.ParentId.HasValue && lookup.TryGetValue(item.ParentId.Value, out var parent))
            {
                parent.Children.Add(item);
            }
            else
            {
                roots.Add(item);
            }
        }

        return roots.OrderBy(r => r.DisplayOrder).ToList();
    }
}

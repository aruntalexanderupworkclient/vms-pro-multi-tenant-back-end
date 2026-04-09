using AutoMapper;
using Microsoft.EntityFrameworkCore;
using VMS.Application.DTOs.Common;
using VMS.Application.DTOs.Users;
using VMS.Application.Interfaces;
using VMS.Core.Entities;
using VMS.Core.Interfaces;
using VMS.Shared.Helpers;

namespace VMS.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public UserService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResult<UserDto>>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _uow.Repository<User>().Query().Include(u => u.Role);
        var totalCount = await query.CountAsync(cancellationToken);
        var users = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return ApiResponse<PagedResult<UserDto>>.SuccessResponse(new PagedResult<UserDto>
        {
            Items = _mapper.Map<List<UserDto>>(users),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    public async Task<ApiResponse<UserDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _uow.Repository<User>().Query()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user == null)
            return ApiResponse<UserDto>.FailResponse("User not found.", "USER_NOT_FOUND");

        return ApiResponse<UserDto>.SuccessResponse(_mapper.Map<UserDto>(user));
    }

    public async Task<ApiResponse<UserDto>> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken = default)
    {
        var exists = await _uow.Repository<User>().AnyAsync(u => u.Email == dto.Email, cancellationToken);
        if (exists)
            return ApiResponse<UserDto>.FailResponse("Email already exists.", "USER_EMAIL_EXISTS");

        var user = _mapper.Map<User>(dto);
        user.PasswordHash = PasswordHasher.Hash(dto.Password);

        await _uow.Repository<User>().AddAsync(user, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var created = await _uow.Repository<User>().Query()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);

        return ApiResponse<UserDto>.SuccessResponse(_mapper.Map<UserDto>(created), "User created successfully.");
    }

    public async Task<ApiResponse<UserDto>> UpdateAsync(Guid id, UpdateUserDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _uow.Repository<User>().Query()
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        if (user == null)
            return ApiResponse<UserDto>.FailResponse("User not found.", "USER_NOT_FOUND");

        if (dto.FullName != null) user.FullName = dto.FullName;
        if (dto.Phone != null) user.Phone = dto.Phone;
        if (dto.RoleId.HasValue) user.RoleId = dto.RoleId;
        if (dto.IsActive.HasValue) user.IsActive = dto.IsActive.Value;

        _uow.Repository<User>().Update(user);
        await _uow.SaveChangesAsync(cancellationToken);

        return ApiResponse<UserDto>.SuccessResponse(_mapper.Map<UserDto>(user), "User updated successfully.");
    }

    public async Task<ApiResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _uow.Repository<User>().GetByIdAsync(id, cancellationToken);
        if (user == null)
            return ApiResponse.FailResponse("User not found.", "USER_NOT_FOUND");

        _uow.Repository<User>().SoftDelete(user);
        await _uow.SaveChangesAsync(cancellationToken);

        return ApiResponse.SuccessResponse("User deleted successfully.");
    }
}

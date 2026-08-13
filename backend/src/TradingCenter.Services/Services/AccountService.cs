using AutoMapper;
using TradingCenter.Domain.Entities;
using TradingCenter.Domain.Interfaces;
using TradingCenter.Services.DTOs.Account;

namespace TradingCenter.Services.Services;

public interface IAccountService
{
    Task<IReadOnlyList<AccountDto>> GetUserAccountsAsync(CancellationToken ct = default);
    Task<AccountDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<AccountDto> CreateAccountAsync(CreateAccountDto dto, CancellationToken ct = default);
}

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public AccountService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<IReadOnlyList<AccountDto>> GetUserAccountsAsync(CancellationToken ct = default)
    {
        var accounts = await _unitOfWork.Repository<Account>().GetAllAsync(ct);
        return _mapper.Map<IReadOnlyList<AccountDto>>(accounts);
    }

    public async Task<AccountDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var account = await _unitOfWork.Repository<Account>().GetByIdAsync(id, ct);
        return account == null ? null : _mapper.Map<AccountDto>(account);
    }

    public async Task<AccountDto> CreateAccountAsync(CreateAccountDto dto, CancellationToken ct = default)
    {
        var userId = _currentUserService.UserId 
            ?? throw new UnauthorizedAccessException("Authenticated user ID is missing.");

        var account = _mapper.Map<Account>(dto);
        account.UserId = userId;

        await _unitOfWork.Repository<Account>().AddAsync(account, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return _mapper.Map<AccountDto>(account);
    }
}

using AutoMapper;
using TradingCenter.Domain.Entities;
using TradingCenter.Services.DTOs.Account;
using TradingCenter.Services.DTOs.Investment;
using TradingCenter.Services.DTOs.Portfolio;
using TradingCenter.Services.DTOs.Transaction;

namespace TradingCenter.Services.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Account, AccountDto>();
        CreateMap<CreateAccountDto, Account>();

        CreateMap<Investment, InvestmentDto>();
        CreateMap<CreateInvestmentDto, Investment>();

        CreateMap<Transaction, TransactionDto>();
        CreateMap<CreateTransactionDto, Transaction>();

        CreateMap<PortfolioSnapshot, PortfolioSnapshotDto>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.SnapshotDate));
    }
}

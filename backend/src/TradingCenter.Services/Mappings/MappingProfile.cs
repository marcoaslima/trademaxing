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

        CreateMap<Asset, CreateAssetDto>().ReverseMap();

        CreateMap<Investment, InvestmentDto>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Asset.Name))
            .ForMember(dest => dest.Ticker, opt => opt.MapFrom(src => src.Asset.Ticker))
            .ForMember(dest => dest.AssetCategory, opt => opt.MapFrom(src => src.Asset.AssetCategory))
            .ForMember(dest => dest.ValuationType, opt => opt.MapFrom(src => src.Asset.ValuationType))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Asset.Currency))
            .ForMember(dest => dest.IndexBenchmark, opt => opt.MapFrom(src => src.Asset.IndexBenchmark))
            .ForMember(dest => dest.LogoUrl, opt => opt.MapFrom(src => src.Asset.LogoUrl));

        CreateMap<CreateInvestmentDto, Investment>();

        CreateMap<Transaction, TransactionDto>();
        CreateMap<CreateTransactionDto, Transaction>();

        CreateMap<PortfolioSnapshot, PortfolioSnapshotDto>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.SnapshotDate));
    }
}

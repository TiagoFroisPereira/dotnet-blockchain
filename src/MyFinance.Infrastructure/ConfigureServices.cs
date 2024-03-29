﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyFinance.Application.Services;
using MyFinance.Infrastructure.Blockchain;
using MyFinance.Infrastructure.Contracts;

namespace MyFinance.Infrastructure;

public static class ConfigureServices
{
    public static void AddInfra(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BlockchainOptions>(configuration.GetSection(BlockchainOptions.Position));
        services.AddScoped<IBlockchainClient, BlockchainClient>();
        services.AddScoped<IFinanceContract, FinanceContract>();
        services.AddScoped<IStakinContract, StakinContract>();
    }
}

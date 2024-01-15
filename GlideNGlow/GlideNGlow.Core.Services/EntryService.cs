using GlideNGlow.Core.Data;
using GlideNGlow.Core.Dto;
using GlideNGlow.Core.Models;
using GlideNGlow.Core.Services.Abstractions;
using GlideNGlow.Core.Services.Enums;
using Microsoft.EntityFrameworkCore;

namespace GlideNGlow.Core.Services;

public class EntryService : IEntryService
{
    private readonly GlideNGlowDbContext _dbContext;

    public EntryService(GlideNGlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }
}
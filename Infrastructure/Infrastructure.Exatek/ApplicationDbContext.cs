﻿using System.Reflection.Emit;

using Infrastructure.Data.Config;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Models.Entities.Localization;
using Models.Entities.System;
using Models.Entities.User;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<Users, IdentityRole<long>, long>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }
        public override DbSet<Users> Users { get; set; }

        public DbSet<Languages> Languages { get; set; }
        public DbSet<Dictionary> Dictionaries { get; set; }
        public DbSet<DictionaryLocalization> DictionaryLocalizations { get; set; }

        public DbSet<PolicyLocalization> PolicyLocalizations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("User");

            base.OnModelCreating(builder);
            new Localization_Configuration(builder);

            builder.ApplyConfiguration(new User_Configuration());
            builder.ApplyConfiguration(new Policy_Configuration());
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Models.Entities.Localization;
using Models.Entities.System;

namespace Infrastructure.Data.Config
{
    public class Policy_Configuration : IEntityTypeConfiguration<PolicyLocalization>
    {
        public void Configure(EntityTypeBuilder<PolicyLocalization> builder)
        {
            builder.HasKey(pl => new { pl.ID, pl.LanguageID });
            var policy = builder.HasData(new List<PolicyLocalization> {
                new PolicyLocalization { ID = 1, LanguageID = 1, Content = "Lorem ipsum policy text in English." },
                new PolicyLocalization { ID = 1, LanguageID = 2, Content = "لوريم إيبسوم نص السياسات باللغة العربية." }});
        }
    }
}

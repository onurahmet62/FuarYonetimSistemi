using FuarYonetimSistemi.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuarYonetimSistemi.Infrastructure.Persistence
{
    public class FairConfiguration : IEntityTypeConfiguration<Fair>
    {
        public void Configure(EntityTypeBuilder<Fair> builder)
        {
            builder.HasKey(f => f.Id);
            builder.Property(f => f.Name).IsRequired().HasMaxLength(100);
            builder.Property(f => f.Location).IsRequired().HasMaxLength(200);
          
            builder.Property(f => f.StartDate).IsRequired();
            builder.Property(f => f.EndDate).IsRequired();
        }
    }
}

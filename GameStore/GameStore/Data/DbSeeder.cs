using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Data.Entities;

namespace GameStore.Data
{
    public static class DbSeeder
    {
        public static void SeedData(AppDbContext context)
        {
            context.Database.EnsureCreated();

            TagSeed(context);
        }

        private static void TagSeed(AppDbContext context)
        {
            if (context.Tags.Any()) return;

            IList<Tag> tags = new List<Tag>
            {
                new Tag()
                {
                    CTime = DateTime.Now,
                    UTime = DateTime.Now,
                    GuidValue = Guid.NewGuid(),
                    Name = "Best Seller All time",
                    IsDeleted = false
                },
                new Tag()
                {
                    CTime = DateTime.Now,
                    UTime = DateTime.Now,
                    GuidValue = Guid.NewGuid(),
                    Name = "Highest seller of this month",
                    IsDeleted = false
                },
                new Tag()
                {
                    CTime = DateTime.Now,
                    UTime = DateTime.Now,
                    GuidValue = Guid.NewGuid(),
                    Name = "Demo",
                    IsDeleted = false
                },
                new Tag()
                {
                    CTime = DateTime.Now,
                    UTime = DateTime.Now,
                    GuidValue = Guid.NewGuid(),
                    Name = "Demo Deleted",
                    IsDeleted = true
                },
            };

            context.Tags.AddRange(tags);
            context.SaveChanges();
        }
    }
}

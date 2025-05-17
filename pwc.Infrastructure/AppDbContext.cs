using Microsoft.EntityFrameworkCore;
using pwc.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pwc.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Item> Items { get; set; }
        public DbSet<Monster> Monsters { get; set; }
        public DbSet<Charakter> Characters { get; set; }
        public DbSet<CharakterItem> CharacterItems { get; set; }
        public DbSet<MonsterItemDrop> MonsterItemDrops { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MonsterItemDrop>()
                .HasKey(mid => new { mid.MonsterId, mid.ItemId });

            modelBuilder.Entity<CharakterItem>()
                .HasKey(ci => new { ci.CharakterId, ci.ItemId });

            modelBuilder.Entity<MonsterItemDrop>()
                .HasOne(mid => mid.Monster)
                .WithMany(m => m.MonsterItemDrops)
                .HasForeignKey(mid => mid.MonsterId);

            modelBuilder.Entity<MonsterItemDrop>()
                .HasOne(mid => mid.Item)
                .WithMany(i => i.MonsterItemDrops)
                .HasForeignKey(mid => mid.ItemId);

            modelBuilder.Entity<CharakterItem>()
                .HasOne(ci => ci.Charakter)
                .WithMany(c => c.CharakterItems)
                .HasForeignKey(ci => ci.CharakterId);

            modelBuilder.Entity<CharakterItem>()
                .HasOne(ci => ci.Item)
                .WithMany(i => i.CharakterItems)
                .HasForeignKey(ci => ci.ItemId);
        }
    }
}

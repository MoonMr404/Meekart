﻿using Microsoft.EntityFrameworkCore;
using Shared.Entities;

namespace ServerBackend.Data;

public class MeerkatDatabase : DbContext
{
    public MeerkatDatabase(DbContextOptions<MeerkatDatabase> options) : base(options) { } //Costruttore per Dependecy injection
    
    /*
     * Ogni DbSet è una tabella all'interno dell'ORM
     */
    public DbSet<User> Users { get; set; }
    public DbSet<Team> Teams { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Name).IsRequired().HasMaxLength(50);
            entity.Property(u => u.Surname).IsRequired().HasMaxLength(50);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Password).IsRequired();
            entity.Property(u => u.BirthDate).IsRequired();
            entity.Property(u => u.Image);

            // Configurazione della relazione molti a molti
            entity.HasMany(u => u.MemberOfTeams)
                .WithMany(t => t.Members);
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Name).IsRequired().HasMaxLength(100);
            entity.Property(t => t.Description).HasMaxLength(500);
            entity.Property(t => t.Deadline);
            entity.Property(t => t.Image);

            // Configurazione della relazione uno a molti
            entity.HasOne(t => t.Manager)
                .WithMany(u => u.ManagedTeams)
                .HasForeignKey(t => t.ManagerId)
                .OnDelete(DeleteBehavior.Restrict); // Previene l'eliminazione di uno user in caso sia manager
        });
    }
}
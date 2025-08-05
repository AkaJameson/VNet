using Microsoft.EntityFrameworkCore;
using VNet.Web.Database.Data;

namespace VNet.Web.Database
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    public class VNetDbContext : DbContext
    {
        public VNetDbContext(DbContextOptions<VNetDbContext> options) : base(options)
        {
        }

        #region RBAC相关表

        /// <summary>
        /// 用户表
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// 用户信息表
        /// </summary>
        public DbSet<UserProfile> UserProfiles { get; set; }

        /// <summary>
        /// 角色表
        /// </summary>
        public DbSet<Role> Roles { get; set; }

        /// <summary>
        /// 权限表
        /// </summary>
        public DbSet<Permission> Permissions { get; set; }

        /// <summary>
        /// API接口表
        /// </summary>
        public DbSet<ApiEndpoint> ApiEndpoints { get; set; }

        /// <summary>
        /// 用户角色关联表
        /// </summary>
        public DbSet<UserRole> UserRoles { get; set; }

        /// <summary>
        /// 角色权限关联表
        /// </summary>
        public DbSet<RolePermission> RolePermissions { get; set; }

        /// <summary>
        /// 权限接口关联表
        /// </summary>
        public DbSet<PermissionApiEndpoint> PermissionApiEndpoints { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 用户表配置
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.PasswordHash).HasMaxLength(500).IsRequired();
                entity.Property(e => e.Salt).HasMaxLength(100).IsRequired();
            });

            // 用户信息表配置
            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.User)
                      .WithOne(e => e.Profile)
                      .HasForeignKey<UserProfile>(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.RealName).HasMaxLength(50);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Avatar).HasMaxLength(500);
            });

            // 角色表配置
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(200);
            });

            // 权限表配置
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Code).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(200);
            });

            // API接口表配置
            modelBuilder.Entity<ApiEndpoint>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.Method, e.Path }).IsUnique();
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Method).HasMaxLength(10).IsRequired();
                entity.Property(e => e.Path).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Controller).HasMaxLength(100);
                entity.Property(e => e.Action).HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(200);
            });

            // 用户角色关联表配置
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });
                entity.HasOne(e => e.User)
                      .WithMany(e => e.UserRoles)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Role)
                      .WithMany(e => e.UserRoles)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 角色权限关联表配置
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(e => new { e.RoleId, e.PermissionId });
                entity.HasOne(e => e.Role)
                      .WithMany(e => e.RolePermissions)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Permission)
                      .WithMany(e => e.RolePermissions)
                      .HasForeignKey(e => e.PermissionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // 权限接口关联表配置
            modelBuilder.Entity<PermissionApiEndpoint>(entity =>
            {
                entity.HasKey(e => new { e.PermissionId, e.ApiEndpointId });
                entity.HasOne(e => e.Permission)
                      .WithMany(e => e.PermissionApiEndpoints)
                      .HasForeignKey(e => e.PermissionId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.ApiEndpoint)
                      .WithMany(e => e.PermissionApiEndpoints)
                      .HasForeignKey(e => e.ApiEndpointId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}

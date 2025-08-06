using Microsoft.EntityFrameworkCore;
using VNet.Web.BaseCore.Data;

namespace VNet.Web.BaseCore.Database
{
    public class VNetDbContext : DbContext
    {
        #region 角色权限配置
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
        /// 用户角色关系表
        /// </summary>
        public DbSet<UserRole> UserRoles { get; set; }
        /// <summary>
        /// 角色权限关系表
        /// </summary>
        public DbSet<RolePermission> RolePermissions { get; set; }
        #endregion
        public VNetDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            #region 角色权限配置
            // 用户 - 用户信息（一对一）
            modelBuilder.Entity<User>()
                .HasOne(u => u.Profile)
                .WithOne(p => p.User)
                .HasForeignKey<UserProfile>(p => p.UserId);

            // 用户角色关系配置（多对多）
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany()
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            // 角色权限关系配置（多对多）
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);

            modelBuilder.Entity<Role>()
                .Property(r => r.Code)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<Role>()
                .Property(r => r.Name)
                .HasMaxLength(100)
                .IsRequired();
            #endregion

        }
    }
}

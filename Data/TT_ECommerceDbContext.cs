using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TT_ECommerce.Models;
using TT_ECommerce.Models.EF;

namespace TT_ECommerce.Data
{
    public partial class TT_ECommerceDbContext : IdentityDbContext<IdentityUser>
    {
        public TT_ECommerceDbContext(DbContextOptions<TT_ECommerceDbContext> options)
            : base(options)
        {
        }

        // DbSets for the application models
        public virtual DbSet<TbContact> TbContacts { get; set; }
        public virtual DbSet<TbOrder> TbOrders { get; set; }
        public virtual DbSet<TbOrderDetail> TbOrderDetails { get; set; }
        public virtual DbSet<TbPost> TbPosts { get; set; }
        public virtual DbSet<TbProduct> TbProducts { get; set; }
        public virtual DbSet<TbProductCategory> TbProductCategories { get; set; }
        public virtual DbSet<TbProductImage> TbProductImages { get; set; }
        public virtual DbSet<TbSubscribe> TbSubscribes { get; set; }
        public virtual DbSet<TbSystemSetting> TbSystemSettings { get; set; }
        public virtual DbSet<ThongKe> ThongKes { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {


                optionsBuilder.UseSqlServer("Data Source=LAPTOP-RAAET1NE\\MINHNHAT;Initial Catalog=TT_ECommerce_V2;Integrated Security=True;Encrypt=True;Trust Server Certificate=True"); // Use your actual connection string here



            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Ensure base method is called

   

            // Configure the IdentityUserRole entity
            modelBuilder.Entity<IdentityUserRole<string>>()
                .HasKey(iur => new { iur.UserId, iur.RoleId });
            modelBuilder.Entity<TbContact>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_dbo.tb_Contact");
                entity.ToTable("tb_Contact");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.Email).HasMaxLength(150);
                entity.Property(e => e.Message).HasMaxLength(4000);
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.Name).HasMaxLength(150);
            });

            modelBuilder.Entity<TbOrder>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_dbo.tb_Order");
                entity.ToTable("tb_Order");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<TbOrderDetail>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_dbo.tb_OrderDetail");
                entity.ToTable("tb_OrderDetail");
                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
                entity.HasOne(d => d.Order)
                    .WithMany(p => p.TbOrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_dbo.tb_OrderDetail_dbo.tb_Order_OrderId");
                entity.HasOne(d => d.Product)
                    .WithMany(p => p.TbOrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_dbo.tb_OrderDetail_dbo.tb_Product_ProductId");
            });

            modelBuilder.Entity<TbPost>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_dbo.tb_Posts");
                entity.ToTable("tb_Posts");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.Image).HasMaxLength(250);
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.SeoDescription).HasMaxLength(500);
                entity.Property(e => e.SeoKeywords).HasMaxLength(250);
                entity.Property(e => e.SeoTitle).HasMaxLength(250);
                entity.Property(e => e.Title).HasMaxLength(150);
            });

            modelBuilder.Entity<TbProduct>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_dbo.tb_Product");
                entity.ToTable("tb_Product");
                entity.Property(e => e.Alias).HasMaxLength(250);
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.Image).HasMaxLength(250);
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.OriginalPrice).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.PriceSale).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.ProductCode).HasMaxLength(50);
                entity.Property(e => e.SeoDescription).HasMaxLength(500);
                entity.Property(e => e.SeoKeywords).HasMaxLength(250);
                entity.Property(e => e.SeoTitle).HasMaxLength(250);
                entity.Property(e => e.Title).HasMaxLength(250);
                entity.HasOne(d => d.ProductCategory)
                    .WithMany(p => p.TbProducts)
                    .HasForeignKey(d => d.ProductCategoryId)
                    .HasConstraintName("FK_dbo.tb_Product_dbo.tb_ProductCategory_ProductCategoryId");
            });

            modelBuilder.Entity<TbProductCategory>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_dbo.tb_ProductCategory");
                entity.ToTable("tb_ProductCategory");
                entity.Property(e => e.Alias).HasMaxLength(150).HasDefaultValue("");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.Icon).HasMaxLength(250);
                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
                entity.Property(e => e.SeoDescription).HasMaxLength(500);
                entity.Property(e => e.SeoKeywords).HasMaxLength(250);
                entity.Property(e => e.SeoTitle).HasMaxLength(250);
                entity.Property(e => e.Title).HasMaxLength(150);
            });

            modelBuilder.Entity<TbProductImage>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_dbo.tb_ProductImage");
                entity.ToTable("tb_ProductImage");
                entity.Property(e => e.Image).HasMaxLength(250);
                entity.HasOne(d => d.Product)
                    .WithMany(p => p.TbProductImages)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_dbo.tb_ProductImage_dbo.tb_Product_ProductId");
            });

            modelBuilder.Entity<TbSubscribe>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_dbo.tb_Subscribe");
                entity.ToTable("tb_Subscribe");
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
                entity.Property(e => e.Email).HasMaxLength(150);
            });

            modelBuilder.Entity<TbSystemSetting>(entity =>
            {
                entity.HasKey(e => e.SettingKey).HasName("PK_dbo.tb_SystemSetting");

                entity.ToTable("tb_SystemSetting");

                entity.Property(e => e.SettingKey).HasMaxLength(50);
                entity.Property(e => e.SettingDescription).HasMaxLength(4000);
                entity.Property(e => e.SettingValue).HasMaxLength(4000);
            });

            modelBuilder.Entity<ThongKe>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_dbo.ThongKes");

                entity.Property(e => e.ThoiGian).HasColumnType("datetime");
            });
            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });
                entity.ToTable("AspNetUserLogins");
            });
            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

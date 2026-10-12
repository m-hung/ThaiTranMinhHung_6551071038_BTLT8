using Microsoft.EntityFrameworkCore;

namespace TiemTapHoa.Models;

public partial class TiemTapHoaContext : DbContext
{
    public TiemTapHoaContext()
    {
    }

    public TiemTapHoaContext(DbContextOptions<TiemTapHoaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<HangHoa> HangHoas { get; set; }

    // !!! Sửa chuỗi kết nối cho đúng máy của bạn (vd: Server=.\SQLEXPRESS;...)
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\MSSQLLocalDB;Database=TiemTapHoa;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HangHoa>(entity =>
        {
            entity.ToTable("HangHoa");
            entity.HasKey(e => e.MaHang);
            entity.Property(e => e.TenHang).HasMaxLength(100);
            entity.Property(e => e.LoaiHang).HasMaxLength(50);
            entity.Property(e => e.DonVi).HasMaxLength(20);
            entity.Property(e => e.GiaBan).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.NgayNhap).HasColumnType("datetime");
        });
    }
}

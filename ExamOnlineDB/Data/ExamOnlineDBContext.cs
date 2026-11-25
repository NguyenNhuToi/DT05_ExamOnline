using ExamOnlineDB.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamOnlineDB.Data;

public class ExamOnlineDBContext : DbContext
{
    public ExamOnlineDBContext(DbContextOptions<ExamOnlineDBContext> options) : base(options) { }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();
    public DbSet<ExamMatrix> ExamMatrices => Set<ExamMatrix>();
    public DbSet<ExamMatrixDetail> ExamMatrixDetails => Set<ExamMatrixDetail>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<ExamParticipant> ExamParticipants => Set<ExamParticipant>();
    public DbSet<UserExam> UserExams => Set<UserExam>();
    public DbSet<UserExamQuestion> UserExamQuestions => Set<UserExamQuestion>();
    public DbSet<UserExamOption> UserExamOptions => Set<UserExamOption>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        // 1. Ánh xạ tên bảng theo script SQL
        mb.Entity<Role>().ToTable("Roles");
        mb.Entity<User>().ToTable("Users");
        mb.Entity<Subject>().ToTable("Subjects");
        mb.Entity<Topic>().ToTable("Topics");
        mb.Entity<Question>().ToTable("Questions");
        mb.Entity<QuestionOption>().ToTable("QuestionOptions");
        mb.Entity<ExamMatrix>().ToTable("ExamMatrices");
        mb.Entity<ExamMatrixDetail>().ToTable("ExamMatrixDetails");
        mb.Entity<Exam>().ToTable("Exams");
        mb.Entity<ExamParticipant>().ToTable("ExamParticipants");
        mb.Entity<UserExam>().ToTable("UserExams");
        mb.Entity<UserExamQuestion>().ToTable("UserExamQuestions");
        mb.Entity<UserExamOption>().ToTable("UserExamOptions");

        // 2. CHỈ ĐỊNH KHÓA CHÍNH (Bắt buộc phải có để sửa lỗi)
        mb.Entity<Role>().HasKey(e => e.RoleId);
        mb.Entity<User>().HasKey(e => e.UserId);
        mb.Entity<Subject>().HasKey(e => e.SubjectId);
        mb.Entity<Topic>().HasKey(e => e.TopicId);
        mb.Entity<Question>().HasKey(e => e.QuestionId);
        mb.Entity<QuestionOption>().HasKey(e => e.OptionId);
        mb.Entity<ExamMatrix>().HasKey(e => e.MatrixId);
        mb.Entity<ExamMatrixDetail>().HasKey(e => e.DetailId);
        mb.Entity<Exam>().HasKey(e => e.ExamId);
        mb.Entity<ExamParticipant>().HasKey(e => e.ParticipantId);
        mb.Entity<UserExam>().HasKey(e => e.UserExamId);
        mb.Entity<UserExamQuestion>().HasKey(e => e.UserExamQuestionId);
        mb.Entity<UserExamOption>().HasKey(e => e.UserExamOptionId);

        // 3. Precision decimal
        mb.Entity<ExamMatrixDetail>().Property(d => d.ScorePerQuestion).HasPrecision(5, 2);
        mb.Entity<UserExamQuestion>().Property(q => q.Point).HasPrecision(5, 2);
        mb.Entity<UserExam>().Property(u => u.TotalScore).HasPrecision(5, 2);

        // 4. Cấu hình các mối quan hệ (Foreign Key)

        // SỬA LỖI CREATORUSERID: Ánh xạ đúng cột CreatedBy trong SQL
        mb.Entity<Question>()
          .HasOne(q => q.Creator)
          .WithMany()
          .HasForeignKey(q => q.CreatedBy);
          
        // FK vòng: SelectedUserExamOptionId -> UserExamOptions (không cascade)
        mb.Entity<UserExamQuestion>()
          .HasOne<UserExamOption>()
          .WithMany()
          .HasForeignKey(q => q.SelectedUserExamOptionId)
          .IsRequired(false)
          .OnDelete(DeleteBehavior.NoAction);

        // UserExamQuestion -> Options (1-n, cascade)
        mb.Entity<UserExamQuestion>()
          .HasMany(q => q.Options)
          .WithOne(o => o.UserExamQuestion)
          .HasForeignKey(o => o.UserExamQuestionId)
          .OnDelete(DeleteBehavior.Cascade);

        // UserExamOption -> OriginalOption (n-1, không cascade)
        mb.Entity<UserExamOption>()
          .HasOne(o => o.OriginalOption)
          .WithMany()
          .HasForeignKey(o => o.OriginalOptionId)
          .OnDelete(DeleteBehavior.NoAction);
    }
}
using ExamOnlineDB.Data;
using ExamOnlineDB.Models;
using Microsoft.EntityFrameworkCore;

namespace ExamOnlineDB.Services;

public class ExamService : IExamService
{
    private readonly ExamOnlineDBContext _db;
    public ExamService(ExamOnlineDBContext db) => _db = db;

    public Task<List<Exam>> GetExamsForStudentAsync(int studentId)
        => _db.Exams
              .Where(e => e.Participants.Any(p => p.StudentId == studentId && p.IsEligible))
              .Include(e => e.Subject)
              .ToListAsync();

    public Task<Exam?> GetExamAsync(int examId)
        => _db.Exams.Include(e => e.Matrix).ThenInclude(m => m!.Details)
                    .FirstOrDefaultAsync(e => e.ExamId == examId);

    public async Task<UserExam?> GetOrCreateUserExamAsync(int examId, int studentId)
    {
        var ue = await _db.UserExams
            .Include(u => u.Exam)
            .Include(u => u.Questions).ThenInclude(q => q.Question)
            .Include(u => u.Questions).ThenInclude(q => q.Options).ThenInclude(o => o.OriginalOption)
            .FirstOrDefaultAsync(u => u.ExamId == examId && u.StudentId == studentId);

        if (ue != null)
        {
            var existingExam = ue.Exam ?? await GetExamAsync(examId);
            var eligible = await _db.ExamParticipants
                .AnyAsync(p => p.ExamId == examId && p.StudentId == studentId && p.IsEligible);
            if (!eligible || existingExam == null)
                return null;

            if (ue.Status == 2)
                return ue;

            var withinDuration = ue.Status == 1
                && DateTime.Now >= existingExam.StartTime
                && DateTime.Now <= ue.StartedAt.AddMinutes(existingExam.DurationMinutes)
                && DateTime.Now <= existingExam.EndTime;

            return withinDuration ? ue : null;
        }

        var exam = await GetExamAsync(examId);
        var now = DateTime.Now;
        if (exam == null || exam.Status != 1 || now < exam.StartTime || now > exam.EndTime)
            return null;

        var isEligible = await _db.ExamParticipants
            .AnyAsync(p => p.ExamId == examId && p.StudentId == studentId && p.IsEligible);
        if (!isEligible) return null;

        ue = new UserExam
        {
            ExamId = examId,
            StudentId = studentId,
            GeneratedExamCode = $"UE_{examId}_{studentId}_{DateTime.Now:HHmmss}",
            StartedAt = DateTime.Now,
            Status = 1
        };
        _db.UserExams.Add(ue);
        await _db.SaveChangesAsync();

        var details = exam.Matrix?.Details ?? new List<ExamMatrixDetail>();
        var usedIds = new HashSet<int>();
        int order = 1;

        foreach (var d in details)
        {
            var qs = await _db.Questions
                .Where(q => q.TopicId == d.TopicId
                         && q.DifficultyLevel == d.DifficultyLevel
                         && q.IsActive
                         && !usedIds.Contains(q.QuestionId))
                .Take(d.QuestionCount)
                .ToListAsync();

            foreach (var q in qs)
            {
                usedIds.Add(q.QuestionId);
                var ueq = new UserExamQuestion
                {
                    UserExamId = ue.UserExamId,
                    QuestionId = q.QuestionId,
                    QuestionOrder = order++,
                    Point = d.ScorePerQuestion
                };
                _db.UserExamQuestions.Add(ueq);
                await _db.SaveChangesAsync();

                var opts = await _db.QuestionOptions
                    .Where(o => o.QuestionId == q.QuestionId)
                    .ToListAsync();

                int labelIdx = 0;
                foreach (var o in opts)
                {
                    _db.UserExamOptions.Add(new UserExamOption
                    {
                        UserExamQuestionId = ueq.UserExamQuestionId,
                        OriginalOptionId = o.OptionId,
                        DisplayLabel = (char)('A' + labelIdx++),
                        DisplayOrder = labelIdx,
                        IsCorrectSnapshot = o.IsCorrect
                    });
                }
                await _db.SaveChangesAsync();
            }
        }

        return await _db.UserExams
            .Include(u => u.Questions).ThenInclude(q => q.Question)
            .Include(u => u.Questions).ThenInclude(q => q.Options).ThenInclude(o => o.OriginalOption)
            .FirstOrDefaultAsync(u => u.UserExamId == ue.UserExamId);
    }

    public async Task<bool> SaveAnswerAsync(int userExamQuestionId, int selectedOptionId, int studentId)
    {
        var ueq = await _db.UserExamQuestions
            .Include(q => q.Options)
            .Include(q => q.UserExam).ThenInclude(u => u!.Exam)
            .FirstOrDefaultAsync(q => q.UserExamQuestionId == userExamQuestionId);

        // Chỉ lưu nếu bài thi đang ở trạng thái "Đang làm" và thuộc sinh viên đang đăng nhập
        var now = DateTime.Now;
        if (ueq?.UserExam == null
            || ueq.UserExam.Status != 1
            || ueq.UserExam.StudentId != studentId
            || ueq.UserExam.Exam == null
            || now < ueq.UserExam.Exam.StartTime
            || now > ueq.UserExam.Exam.EndTime
            || now > ueq.UserExam.StartedAt.AddMinutes(ueq.UserExam.Exam.DurationMinutes))
            return false;

        // Kiểm tra đáp án có thuộc câu hỏi này không
        if (!ueq.Options.Any(o => o.UserExamOptionId == selectedOptionId))
            return false;

        ueq.SelectedUserExamOptionId = selectedOptionId;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SubmitExamAsync(int userExamId, int studentId)
    {
        var ue = await _db.UserExams
            .Include(u => u.Exam)
            .Include(u => u.Questions).ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(u => u.UserExamId == userExamId && u.StudentId == studentId);

        if (ue == null || ue.Status == 2 || ue.Exam == null)
            return false;

        var now = DateTime.Now;
        if (now < ue.StartedAt || now < ue.Exam.StartTime)
            return false;

        decimal total = 0;
        foreach (var q in ue.Questions)
        {
            if (q.SelectedUserExamOptionId == null)
                continue;

            var selectedOption = q.Options.FirstOrDefault(o => o.UserExamOptionId == q.SelectedUserExamOptionId);
            if (selectedOption != null && selectedOption.IsCorrectSnapshot)
            {
                total += q.Point;
            }
        }

        ue.TotalScore = total;
        ue.SubmittedAt = DateTime.Now;
        ue.Status = 2; // Đã nộp
        await _db.SaveChangesAsync();
        return true;
    }
}
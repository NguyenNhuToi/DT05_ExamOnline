function initTakeExam(endTime, userExamId) {
    startCountdown(endTime, userExamId);

    $('.answer-radio:checked').each(function () {
        const qid = $(this).data('qid');
        $('#status-' + qid).removeClass('bg-secondary').addClass('bg-success').text('Đã trả lời');
    });

    $('.answer-radio').on('change', function () {
        const qid = $(this).data('qid');
        const oid = $(this).val();
        $.post('/Student/TakeExam/SaveAnswer',
            { userExamQuestionId: qid, selectedOptionId: oid },
            function () {
                $('#status-' + qid).removeClass('bg-secondary').addClass('bg-success').text('Đã trả lời');
            });
    });

    $('#btnSubmit').on('click', function () {
        if (!confirm('Bạn chắc chắn muốn nộp bài?')) return;
        $.post('/Student/TakeExam/Submit', { userExamId: userExamId }, function (res) {
            alert('Nộp bài thành công!');
            window.location.href = res.redirectUrl;
        });
    });
}
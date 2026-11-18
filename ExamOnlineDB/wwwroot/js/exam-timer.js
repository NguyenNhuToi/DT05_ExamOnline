function startCountdown(endTimeMs, userExamId) {
    const el = document.getElementById('countdown');
    if (!el) return;
    function tick() {
        const diff = Math.floor((endTimeMs - Date.now()) / 1000);
        if (diff <= 0) {
            el.textContent = '00:00';
            $.post('/Student/TakeExam/Submit', { userExamId: userExamId }, function (res) {
                alert('Hết giờ! Bài thi đã được nộp tự động.');
                window.location.href = res.redirectUrl;
            });
            return;
        }
        const m = String(Math.floor(diff / 60)).padStart(2, '0');
        const s = String(diff % 60).padStart(2, '0');
        el.textContent = m + ':' + s;
        setTimeout(tick, 1000);
    }
    tick();
}
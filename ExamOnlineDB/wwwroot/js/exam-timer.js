function startCountdown(endTimeMs, userExamId, verificationToken) {
    const el = document.getElementById('countdown');
    if (!el) return;

    function tick() {
        const diff = Math.floor((endTimeMs - Date.now()) / 1000);

        // Hết giờ
        if (diff <= 0) {
            el.textContent = '00:00';
            alert('Hết giờ làm bài! Hệ thống sẽ tự động nộp bài.');

            // Gọi API nộp bài tự động (Dùng XMLHttpRequest để không bị lỗi TS)
            var xhr = new XMLHttpRequest();
            xhr.open("POST", '/Student/TakeExam/Submit', true);
            xhr.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            xhr.onreadystatechange = function () {
                if (xhr.readyState === 4 && xhr.status === 200) {
                    var res = JSON.parse(xhr.responseText);
                    if (res.success) {
                        window.location.href = res.redirectUrl;
                    }
                }
            };
            xhr.send("__RequestVerificationToken=" + encodeURIComponent(verificationToken) +
                "&userExamId=" + userExamId);
            return;
        }

        const m = String(Math.floor(diff / 60)).padStart(2, '0');
        const s = String(diff % 60).padStart(2, '0');
        el.textContent = m + ':' + s;

        // Đổi màu đỏ khi còn dưới 5 phút
        if (diff < 300) {
            el.classList.add('text-danger');
        }

        setTimeout(tick, 1000);
    }
    tick();
}
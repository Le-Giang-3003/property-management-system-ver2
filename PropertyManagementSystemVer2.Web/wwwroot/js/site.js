// File: wwwroot/js/site.js

// ===== PASSWORD TOGGLE =====
document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.password-toggle').forEach(function (btn) {
        btn.addEventListener('click', function () {
            var input = this.closest('.input-icon-wrapper').querySelector('input');
            var icon = this.querySelector('i');
            if (input.type === 'password') {
                input.type = 'text';
                icon.classList.replace('bi-eye', 'bi-eye-slash');
            } else {
                input.type = 'password';
                icon.classList.replace('bi-eye-slash', 'bi-eye');
            }
        });
    });
});

// ===== PASSWORD STRENGTH =====
function checkPasswordStrength(password) {
    var score = 0;
    if (password.length >= 8) score++;
    if (/[A-Z]/.test(password)) score++;
    if (/[a-z]/.test(password)) score++;
    if (/[0-9]/.test(password)) score++;
    if (/[\W_]/.test(password)) score++;

    var bar = document.getElementById('passwordStrengthBar');
    var text = document.getElementById('passwordStrengthText');
    if (!bar) return;

    bar.className = 'password-strength-bar';
    if (score <= 1) { bar.classList.add('strength-weak'); text.textContent = 'Yếu'; }
    else if (score <= 2) { bar.classList.add('strength-fair'); text.textContent = 'Trung bình'; }
    else if (score <= 3) { bar.classList.add('strength-good'); text.textContent = 'Tốt'; }
    else { bar.classList.add('strength-strong'); text.textContent = 'Mạnh'; }
}

// ===== AUTO-DISMISS ALERTS =====
document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.alert-dismissible').forEach(function (alert) {
        setTimeout(function () {
            var bsAlert = bootstrap.Alert.getOrCreateInstance(alert);
            bsAlert.close();
        }, 5000);
    });
});
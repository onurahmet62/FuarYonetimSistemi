// js/pages/login.js

document.addEventListener('DOMContentLoaded', () => {
    const loginForm = document.getElementById('loginForm');

    if (localStorage.getItem('token') && window.location.pathname.toLowerCase().includes('login.html')) {
        window.location.href = 'home.html';
        return;
    }

    if (loginForm) {
        loginForm.addEventListener('submit', async (event) => {
            event.preventDefault();
            const emailInput = document.getElementById('loginEmail');
            const passwordInput = document.getElementById('loginPassword');

            const email = emailInput.value;
            const password = passwordInput.value;

            try {
                const response = await fetch(`${API_BASE_URL}/Auth/Login`, {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({ email, password })
                });

                if (!response.ok) {
                    const errorData = await response.json().catch(() => ({}));
                    const errorMessage = errorData.message || 'Giriş başarısız. Lütfen bilgilerinizi kontrol edin.';
                    Swal.fire('Giriş Başarısız', errorMessage, 'error');
                    return;
                }

                const data = await response.json();

                // DÜZELTME: Sadece token varlığını kontrol et.
                if (data.token) {
                    localStorage.setItem('token', data.token);
                    localStorage.setItem('userName', data.userName || email.split('@')[0]);

                    // userRole kontrolünü esnek hale getir.
                    if (data.userRole) {
                        localStorage.setItem('userRole', data.userRole);
                    } else {
                        // Eğer API'den rol gelmezse, geliştirme için geçici bir varsayılan ata ve uyar.
                        console.warn('API /Auth/Login yanıtında "userRole" alanı bulunamadı. Yetkilendirme için varsayılan olarak "Admin" rolü atanıyor. Lütfen backend\'i güncelleyin.');
                        localStorage.setItem('userRole', 'Admin');
                    }

                    Swal.fire({
                        icon: 'success',
                        title: 'Giriş Başarılı!',
                        text: 'Yönlendiriliyorsunuz...',
                        timer: 1500,
                        showConfirmButton: false
                    }).then(() => {
                        window.location.href = 'home.html';
                    });

                } else {
                    Swal.fire('Giriş Hatası', 'API yanıtında "token" verisi bulunamadı.', 'warning');
                }
            } catch (error) {
                console.error('Login sırasında hata:', error);
                Swal.fire('Giriş Hatası', 'Bir hata oluştu. Lütfen tekrar deneyin.', 'error');
            }
        });
    }
});

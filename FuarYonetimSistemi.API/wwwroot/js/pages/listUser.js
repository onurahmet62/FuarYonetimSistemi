// js/pages/listUser.js (Geçici Düzeltme - İstemci Taraflı Filtreleme)
// NOT: Bu dosya, /User/filter endpoint'i backend'de hazır olana kadar
// tüm kullanıcıları çekip tarayıcıda filtreleme yöntemini kullanır.

// --- SAYFA DURUMU (PAGE STATE) ---
const userListPageState = {
    currentPage: 1,
    totalPages: 1,
    totalRecords: 0,
    pageSize: 20,
    filteredData: [] // Filtrelenmiş veriyi tutmak için
};

// --- ANA FONKSİYONLAR ---

/**
 * Kullanıcı listesi sayfasını başlatan ana fonksiyon.
 */
window.loadUsersPage = async function () {
    console.log("Kullanıcı sayfası İSTEMCİ TARAFLI filtreleme ile başlatılıyor (Geçici Düzeltme).");
    setupEventListeners();
    await loadUsers(); // Sayfa ilk yüklendiğinde kullanıcıları yükle
};

/**
 * Tüm kullanıcıları AppState'ten alır, filtreler ve görüntüler.
 */
async function loadUsers() {
    try {
        // core.js'teki fetchAllUsers fonksiyonu tüm kullanıcıları GET ile çeker.
        await window.fetchAllUsers(true);

        // Filtreleri uygula
        applyFilters();

        // Sayfalama bilgilerini ayarla
        userListPageState.totalRecords = userListPageState.filteredData.length;
        userListPageState.pageSize = parseInt(document.getElementById('pageSizeUser').value, 10);
        userListPageState.totalPages = Math.ceil(userListPageState.totalRecords / userListPageState.pageSize) || 1;
        userListPageState.currentPage = Math.min(userListPageState.currentPage, userListPageState.totalPages) || 1;

        renderUsersTable();
        updatePaginationInfo();

    } catch (error) {
        console.error("Kullanıcılar yüklenirken hata oluştu:", error);
        document.getElementById('userTableBody').innerHTML = `<tr><td colspan="5" class="text-center text-danger">Kullanıcılar yüklenirken bir hata oluştu.</td></tr>`;
    }
}

/**
 * Filtre kriterlerini AppState.users üzerinden uygular.
 */
function applyFilters() {
    const filterName = document.getElementById('filterUserName')?.value.toLowerCase() || '';
    const filterEmail = document.getElementById('filterUserEmail')?.value.toLowerCase() || '';
    const filterRole = document.getElementById('filterUserRole')?.value || '';

    userListPageState.filteredData = window.AppState.users.filter(user => {
        const matchesName = filterName ? user.fullName?.toLowerCase().includes(filterName) : true;
        const matchesEmail = filterEmail ? user.email?.toLowerCase().includes(filterEmail) : true;
        const matchesRole = (filterRole && filterRole !== "") ? user.role.toString() === filterRole : true;

        return matchesName && matchesEmail && matchesRole && !user.isDeleted;
    });
}

/**
 * Filtrelenmiş ve sayfalanmış kullanıcı verilerini HTML tablosunda görüntüler.
 */
function renderUsersTable() {
    const tableBody = document.getElementById('userTableBody');
    tableBody.innerHTML = '';

    const startIndex = (userListPageState.currentPage - 1) * userListPageState.pageSize;
    const endIndex = startIndex + userListPageState.pageSize;
    const usersToDisplay = userListPageState.filteredData.slice(startIndex, endIndex);

    if (usersToDisplay.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="5" class="text-center p-4">Gösterilecek kullanıcı bulunamadı.</td></tr>';
        return;
    }

    usersToDisplay.forEach(user => {
        const row = tableBody.insertRow();
        const userStatusText = user.isActive ? '<span class="badge bg-success">Aktif</span>' : '<span class="badge bg-danger">Pasif</span>';
        const userRoleText = window.getUserRoleName(user.role);

        row.innerHTML = `
            <td>${window.escapeHtml(user.fullName)}</td>
            <td>${window.escapeHtml(user.email)}</td>
            <td>${window.escapeHtml(userRoleText)}</td>
            <td>${userStatusText}</td>
            <td>
                <button class="btn btn-sm btn-warning edit-user-btn me-1" data-id="${user.id}" title="Düzenle"><i class="bi bi-pencil"></i></button>
                <button class="btn btn-info btn-sm reset-password-btn me-1" data-id="${user.id}" title="Şifre Sıfırla"><i class="bi bi-key"></i></button>
                <button class="btn btn-danger btn-sm delete-user-btn" data-id="${user.id}" title="Sil"><i class="bi bi-trash"></i></button>
            </td>
        `;
    });
}

/**
 * Sayfalama kullanıcı arayüzünü günceller.
 */
function updatePaginationInfo() {
    window.updateGenericPaginationUI({
        currentPage: userListPageState.currentPage,
        totalPages: userListPageState.totalPages,
        totalRecords: userListPageState.totalRecords,
        pageInfoId: 'currentPageInfoUser',
        prevBtnId: 'prevPageUser',
        nextBtnId: 'nextPageUser',
        recordsInfoId: 'recordCountUser'
    });
}

/**
 * Sayfadaki tüm olay dinleyicilerini ayarlar.
 */
function setupEventListeners() {
    const filterForm = document.getElementById('filterFormUser');
    filterForm?.addEventListener('submit', (e) => { e.preventDefault(); loadUsers(); });
    document.getElementById('clearUserFilterBtn')?.addEventListener('click', () => { filterForm.reset(); loadUsers(); });

    document.getElementById('pageSizeUser')?.addEventListener('change', () => loadUsers());
    document.getElementById('prevPageUser')?.addEventListener('click', () => { if (userListPageState.currentPage > 1) { userListPageState.currentPage--; renderUsersTable(); updatePaginationInfo(); } });
    document.getElementById('nextPageUser')?.addEventListener('click', () => { if (userListPageState.currentPage < userListPageState.totalPages) { userListPageState.currentPage++; renderUsersTable(); updatePaginationInfo(); } });

    document.getElementById('addUserForm')?.addEventListener('submit', handleAddUser);
    document.getElementById('editUserForm')?.addEventListener('submit', handleEditUser);
    document.getElementById('exportUserExcelButton')?.addEventListener('click', exportUsersToExcel);

    document.getElementById('userTableBody')?.addEventListener('click', (e) => {
        const editBtn = e.target.closest('.edit-user-btn');
        if (editBtn) setupEditUserForm(editBtn.dataset.id);
        const deleteBtn = e.target.closest('.delete-user-btn');
        if (deleteBtn) confirmDeleteUser(deleteBtn.dataset.id);
        const resetPassBtn = e.target.closest('.reset-password-btn');
        if (resetPassBtn) resetUserPassword(resetPassBtn.dataset.id);
    });
}

async function setupEditUserForm(userId) {
    const user = window.AppState.users.find(u => u.id === userId);
    if (!user) {
        Swal.fire('Hata', 'Kullanıcı bulunamadı.', 'error');
        return;
    }
    const form = document.getElementById('editUserForm');
    form.reset();
    document.getElementById('editUserId').value = user.id;
    document.getElementById('editUserName').value = user.fullName || '';
    document.getElementById('editUserEmail').value = user.email || '';
    document.getElementById('editUserRole').value = user.role;
    document.getElementById('editUserStatus').checked = user.isActive;
    new bootstrap.Modal(document.getElementById('editUserModal')).show();
}

async function handleAddUser(event) {
    event.preventDefault();
    const form = event.target;
    const userData = {
        fullName: form.querySelector('#newUserName').value,
        email: form.querySelector('#newUserEmail').value,
        password: form.querySelector('#newUserPassword').value,
        role: parseInt(form.querySelector('#newUserRole').value, 10),
        isActive: form.querySelector('#newUserStatus').checked,
    };
    try {
        await window.fetchWithToken('/User', { method: 'POST', body: JSON.stringify(userData) });
        Swal.fire('Başarılı!', 'Kullanıcı başarıyla eklendi.', 'success');
        bootstrap.Modal.getInstance(form.closest('.modal')).hide();
        await loadUsers();
    } catch (error) {
        console.error("Kullanıcı eklenirken hata:", error);
    }
}

async function handleEditUser(event) {
    event.preventDefault();
    const form = event.target;
    const userId = form.querySelector('#editUserId').value;
    const userData = {
        id: userId,
        fullName: form.querySelector('#editUserName').value,
        email: form.querySelector('#editUserEmail').value,
        role: parseInt(form.querySelector('#editUserRole').value, 10),
        isActive: form.querySelector('#editUserStatus').checked,
    };
    try {
        await window.fetchWithToken(`/User/${userId}`, { method: 'PUT', body: JSON.stringify(userData) });
        Swal.fire('Başarılı!', 'Kullanıcı başarıyla güncellendi.', 'success');
        bootstrap.Modal.getInstance(form.closest('.modal')).hide();
        await loadUsers();
    } catch (error) {
        console.error("Kullanıcı güncellenirken hata:", error);
    }
}

function confirmDeleteUser(id) {
    Swal.fire({
        title: 'Emin misiniz?', text: "Bu kullanıcıyı silmek geri alınamaz!", icon: 'warning', showCancelButton: true, confirmButtonColor: '#d33', confirmButtonText: 'Evet, Sil!', cancelButtonText: 'İptal'
    }).then(async (result) => {
        if (result.isConfirmed) {
            try {
                await window.fetchWithToken(`/User/${id}`, { method: 'DELETE' });
                Swal.fire('Silindi!', 'Kullanıcı başarıyla silindi.', 'success');
                await loadUsers();
            } catch (error) {
                console.error("Kullanıcı silinirken hata:", error);
            }
        }
    });
}

async function resetUserPassword(userId) {
    try {
        const response = await window.fetchWithToken(`/User/${userId}/reset-password`, { method: 'POST', body: JSON.stringify({}) });
        const data = await response.json();
        if (data && data.newPassword) {
            document.getElementById('newPasswordDisplay').textContent = data.newPassword;
            new bootstrap.Modal(document.getElementById('resetPasswordModal')).show();
            Swal.fire('Başarılı!', 'Yeni şifre oluşturuldu. Lütfen not alınız.', 'success');
        } else {
            throw new Error('API yanıtından yeni şifre alınamadı.');
        }
    } catch (error) {
        console.error("Şifre sıfırlanırken hata:", error);
    }
}

function exportUsersToExcel() {
    const dataToExport = userListPageState.filteredData;
    if (!dataToExport || dataToExport.length === 0) {
        Swal.fire('Bilgi', 'Excel\'e aktarılacak kullanıcı bulunamadı.', 'info');
        return;
    }
    const exportData = dataToExport.map(user => ({
        "Kullanıcı Adı": user.fullName,
        "E-posta": user.email,
        "Rol": window.getUserRoleName(user.role),
        "Durum": user.isActive ? 'Aktif' : 'Pasif'
    }));
    window.exportToExcel(exportData, 'KullaniciListesi', 'Kullanıcılar');
}

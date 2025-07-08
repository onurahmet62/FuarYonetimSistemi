// js/pages/listTargets.js

const targetListPageState = {
    currentPage: 1,
    totalPages: 1,
    totalRecords: 0,
    pageSize: 20
};

window.loadTargetsPage = async function () {
    console.log("Hedef Yönetimi sayfası yükleniyor...");
    setupEventListeners();

    await Promise.all([
        window.loadUsersForDropdown('filterTargetUser', 'Tüm Personeller', '', true),
        window.loadFairsForDropdown('filterTargetFair', 'Tüm Fuarlar', '', true),
        window.loadUsersForDropdown('targetUserId', 'Personel Seçiniz...', '', false),
        window.loadFairsForDropdown('targetFairId', 'Fuar Seçiniz...', '', false)
    ]);

    await loadTargets(1);
};

async function loadTargets(pageNumber = 1) {
    targetListPageState.currentPage = pageNumber;
    targetListPageState.pageSize = parseInt(document.getElementById('pageSizeTarget').value, 10);

    const filters = {
        pageNumber: targetListPageState.currentPage,
        pageSize: targetListPageState.pageSize,
        userId: document.getElementById('filterTargetUser')?.value || null,
        fairId: document.getElementById('filterTargetFair')?.value || null,
        status: document.getElementById('filterTargetStatus')?.value || null,
    };

    try {
        const responseData = await window.fetchTargets(filters);

        targetListPageState.totalRecords = responseData.totalCount;
        targetListPageState.totalPages = responseData.totalPages || 1;

        renderTargetsTable(responseData.targets);
        updatePaginationInfo();
    } catch (error) {
        console.error("Hedefler yüklenirken hata:", error);
        document.getElementById('targetTableBody').innerHTML = `<tr><td colspan="8" class="text-center p-4 text-danger">Hedefler yüklenirken bir hata oluştu.</td></tr>`;
    }
}

function renderTargetsTable(targets) {
    const tableBody = document.getElementById('targetTableBody');
    tableBody.innerHTML = '';

    if (!targets || targets.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="8" class="text-center p-4">Gösterilecek hedef kaydı bulunamadı.</td></tr>';
        return;
    }

    targets.forEach(target => {
        const row = tableBody.insertRow();
        const userName = window.getUserNameById(target.userId) || 'Bilinmiyor';
        const fairName = window.getFairNameById(target.fairId) || 'Bilinmiyor';
        const achievedAmount = target.achievedAmount || 0;
        const targetAmount = target.targetAmount || 1; // 0'a bölünmeyi engelle
        const successRate = ((achievedAmount / targetAmount) * 100).toFixed(2);

        let statusBadge = '';
        switch (target.status) {
            case 'InProgress': statusBadge = '<span class="badge bg-primary">Devam Ediyor</span>'; break;
            case 'Completed': statusBadge = '<span class="badge bg-success">Tamamlandı</span>'; break;
            case 'Expired': statusBadge = '<span class="badge bg-danger">Süresi Doldu</span>'; break;
            default: statusBadge = `<span class="badge bg-secondary">${target.status}</span>`; break;
        }

        row.innerHTML = `
            <td>${window.escapeHtml(userName)}</td>
            <td>${window.escapeHtml(fairName)}</td>
            <td>${window.formatCurrency(target.targetAmount)}</td>
            <td>${window.formatCurrency(achievedAmount)}</td>
            <td>
                <div class="progress" style="height: 20px;">
                    <div class="progress-bar" role="progressbar" style="width: ${successRate}%;" aria-valuenow="${successRate}" aria-valuemin="0" aria-valuemax="100">${successRate}%</div>
                </div>
            </td>
            <td>${statusBadge}</td>
            <td>${window.formatDate(target.deadline)}</td>
            <td>
                <button class="btn btn-sm btn-warning edit-target-btn" data-id="${target.id}"><i class="bi bi-pencil"></i></button>
                <button class="btn btn-sm btn-danger delete-target-btn" data-id="${target.id}"><i class="bi bi-trash"></i></button>
            </td>
        `;
    });
}

function updatePaginationInfo() {
    window.updateGenericPaginationUI({
        currentPage: targetListPageState.currentPage,
        totalPages: targetListPageState.totalPages,
        totalRecords: targetListPageState.totalRecords,
        pageInfoId: 'currentPageInfoTarget',
        prevBtnId: 'prevPageTarget',
        nextBtnId: 'nextPageTarget',
        recordsInfoId: 'recordCountTarget' // Bu ID HTML'e eklenebilir.
    });
}

function setupEventListeners() {
    const filterForm = document.getElementById('filterTargetForm');
    filterForm?.addEventListener('submit', (e) => { e.preventDefault(); loadTargets(1); });
    document.getElementById('clearTargetFilterBtn')?.addEventListener('click', () => { filterForm.reset(); loadTargets(1); });

    document.getElementById('pageSizeTarget')?.addEventListener('change', () => loadTargets(1));
    document.getElementById('prevPageTarget')?.addEventListener('click', () => { if (targetListPageState.currentPage > 1) loadTargets(targetListPageState.currentPage - 1); });
    document.getElementById('nextPageTarget')?.addEventListener('click', () => { if (targetListPageState.currentPage < targetListPageState.totalPages) loadTargets(targetListPageState.currentPage + 1); });

    document.getElementById('assignTargetButton')?.addEventListener('click', () => {
        document.getElementById('assignTargetForm').reset();
        document.getElementById('targetId').value = '';
        document.getElementById('assignTargetModalLabel').textContent = 'Yeni Hedef Ata';
        new bootstrap.Modal(document.getElementById('assignTargetModal')).show();
    });

    document.getElementById('assignTargetForm')?.addEventListener('submit', handleAddOrEditTarget);

    document.getElementById('targetTableBody')?.addEventListener('click', (e) => {
        const editBtn = e.target.closest('.edit-target-btn');
        if (editBtn) setupEditTargetForm(editBtn.dataset.id);
        const deleteBtn = e.target.closest('.delete-target-btn');
        if (deleteBtn) confirmDeleteTarget(deleteBtn.dataset.id);
    });
}

async function setupEditTargetForm(targetId) {
    try {
        // Tek bir hedefi çekmek için /api/Targets/{id} endpoint'i varsayılıyor.
        const response = await window.fetchWithToken(`/Targets/${targetId}`);
        const target = await response.json();

        document.getElementById('assignTargetForm').reset();
        document.getElementById('targetId').value = target.id;
        document.getElementById('assignTargetModalLabel').textContent = 'Hedefi Düzenle';

        document.getElementById('targetUserId').value = target.userId;
        document.getElementById('targetFairId').value = target.fairId;
        document.getElementById('targetAmount').value = target.targetAmount;
        document.getElementById('targetDeadline').value = target.deadline ? new Date(target.deadline).toISOString().split('T')[0] : '';
        document.getElementById('targetStatus').value = target.status;

        new bootstrap.Modal(document.getElementById('assignTargetModal')).show();
    } catch (error) {
        Swal.fire('Hata', 'Hedef bilgileri yüklenemedi.', 'error');
    }
}

async function handleAddOrEditTarget(event) {
    event.preventDefault();
    const form = event.target;
    const targetId = form.querySelector('#targetId').value;

    const data = {
        userId: form.querySelector('#targetUserId').value,
        fairId: form.querySelector('#targetFairId').value,
        targetAmount: parseFloat(form.querySelector('#targetAmount').value),
        deadline: form.querySelector('#targetDeadline').value,
        status: form.querySelector('#targetStatus').value,
    };

    const url = targetId ? `/Targets/${targetId}` : '/Targets';
    const method = targetId ? 'PUT' : 'POST';

    try {
        await window.fetchWithToken(url, { method: method, body: JSON.stringify(data) });
        Swal.fire('Başarılı!', `Hedef başarıyla ${targetId ? 'güncellendi' : 'atandı'}.`, 'success');
        bootstrap.Modal.getInstance(form.closest('.modal')).hide();
        await loadTargets(targetId ? targetListPageState.currentPage : 1);
    } catch (error) {
        console.error(`Hedef ${targetId ? 'güncellenirken' : 'atanırken'} hata:`, error);
    }
}

function confirmDeleteTarget(id) {
    Swal.fire({
        title: 'Emin misiniz?', text: "Bu hedefi silmek geri alınamaz!", icon: 'warning', showCancelButton: true, confirmButtonColor: '#d33', confirmButtonText: 'Evet, Sil!', cancelButtonText: 'İptal'
    }).then(async (result) => {
        if (result.isConfirmed) {
            try {
                await window.fetchWithToken(`/Targets/${id}`, { method: 'DELETE' });
                Swal.fire('Silindi!', 'Hedef başarıyla silindi.', 'success');
                await loadTargets(targetListPageState.currentPage);
            } catch (error) {
                console.error("Hedef silinirken hata:", error);
            }
        }
    });
}

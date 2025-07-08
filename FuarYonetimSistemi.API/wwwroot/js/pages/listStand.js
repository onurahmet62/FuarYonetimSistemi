// js/pages/listStand.js (Doğrulama ve API uyumluluğu için son düzeltme)

// --- SAYFA DURUMU (PAGE STATE) ---
const standListPageState = {
    currentPage: 1,
    totalPages: 1,
    totalRecords: 0,
    pageSize: 20
};

// --- ANA FONKSİYONLAR ---

/**
 * Stand listesi sayfasını başlatan ana fonksiyon.
 */
window.loadStandsPage = async function () {
    console.log("Stand sayfası sunucu taraflı filtreleme ile başlatılıyor...");
    setupEventListeners();

    await Promise.all([
        window.loadFairsForDropdown('filterStandFair', 'Tüm Fuarlar', '', true),
        window.loadParticipantsForDropdown('filterStandParticipant', 'Tüm Katılımcılar', '', true),
        window.loadFairsForDropdown('addStandFairId', 'Fuar Seçiniz...', '', false),
        window.loadParticipantsForDropdown('addStandParticipantId', 'Katılımcı Seçiniz...', '', false),
        window.loadFairsForDropdown('editStandFairId', 'Fuar Seçiniz...', '', false),
        window.loadParticipantsForDropdown('editStandParticipantId', 'Katılımcı Seçiniz...', '', false)
    ]);

    await loadStands(1);
};

/**
 * Filtrelere göre standları API'den çeker ve görüntüler.
 */
async function loadStands(pageNumber = 1) {
    standListPageState.currentPage = pageNumber;
    standListPageState.pageSize = parseInt(document.getElementById('pageSizeStand').value, 10);

    const filters = {
        pageNumber: standListPageState.currentPage,
        pageSize: standListPageState.pageSize,
        fairId: document.getElementById('filterStandFair')?.value === 'all' ? null : document.getElementById('filterStandFair')?.value,
        participantId: document.getElementById('filterStandParticipant')?.value === 'all' ? null : document.getElementById('filterStandParticipant')?.value,
        standNo: document.getElementById('filterStandNo')?.value || null
    };

    try {
        const responseData = await window.fetchStands(filters);
        standListPageState.totalRecords = responseData.totalCount;
        standListPageState.totalPages = responseData.totalPages || 1;
        renderStandsTable(responseData.stands);
        updatePaginationInfo();
    } catch (error) {
        console.error("Standlar yüklenirken hata:", error);
        document.getElementById('standTableBody').innerHTML = '<tr><td colspan="7" class="text-center p-4 text-danger">Standlar yüklenirken bir hata oluştu.</td></tr>';
        standListPageState.totalRecords = 0;
        standListPageState.totalPages = 1;
        updatePaginationInfo();
    }
}

/**
 * Verilen stand verilerini HTML tablosunda görüntüler.
 */
function renderStandsTable(stands) {
    const tableBody = document.getElementById('standTableBody');
    tableBody.innerHTML = '';

    if (!stands || stands.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="7" class="text-center p-4">Gösterilecek stand bulunamadı.</td></tr>';
        return;
    }

    stands.forEach(stand => {
        const row = tableBody.insertRow();
        const fairName = window.getFairNameById(stand.fairId) || '-';
        const participantName = window.getParticipantNameById(stand.participantId) || '-';
        const balance = stand.balance || 0;
        const balanceClass = balance > 0 ? 'text-danger' : 'text-success';
        row.innerHTML = `
            <td>${window.escapeHtml(fairName)}</td>
            <td>${window.escapeHtml(participantName)}</td>
            <td>${window.escapeHtml(stand.name)}</td>
            <td>${stand.contractArea || '0'} m²</td>
            <td>${window.formatCurrency(stand.totalAmountWithVAT || 0, stand.currency)}</td>
            <td class="fw-bold ${balanceClass}">${window.formatCurrency(balance, stand.currency)}</td>
            <td>
                <a href="standDetails.html?id=${stand.id}" class="btn btn-sm btn-primary" title="Detaylar"><i class="bi bi-eye"></i></a>
                <button class="btn btn-sm btn-warning edit-stand-btn" data-id="${stand.id}" title="Düzenle"><i class="bi bi-pencil"></i></button>
                <button class="btn btn-sm btn-danger delete-stand-btn" data-id="${stand.id}" title="Sil"><i class="bi bi-trash"></i></button>
            </td>
        `;
    });
}


/**
 * Sayfalama kullanıcı arayüzünü günceller.
 */
function updatePaginationInfo() {
    window.updateGenericPaginationUI({
        currentPage: standListPageState.currentPage,
        totalPages: standListPageState.totalPages,
        totalRecords: standListPageState.totalRecords,
        pageInfoId: 'currentPageStand',
        prevBtnId: 'prevPageStand',
        nextBtnId: 'nextPageStand',
        recordsInfoId: 'recordCountStand'
    });
}

/**
 * Sayfadaki tüm olay dinleyicilerini ayarlar.
 */
function setupEventListeners() {
    const filterForm = document.getElementById('filterFormStand');
    filterForm?.addEventListener('submit', (e) => { e.preventDefault(); loadStands(1); });
    document.getElementById('clearStandFilterBtn')?.addEventListener('click', () => { filterForm.reset(); loadStands(1); });

    document.getElementById('pageSizeStand')?.addEventListener('change', () => loadStands(1));
    document.getElementById('prevPageStand')?.addEventListener('click', () => { if (standListPageState.currentPage > 1) loadStands(standListPageState.currentPage - 1); });
    document.getElementById('nextPageStand')?.addEventListener('click', () => { if (standListPageState.currentPage < standListPageState.totalPages) loadStands(standListPageState.currentPage + 1); });

    document.getElementById('addStandForm')?.addEventListener('submit', handleAddOrEditStand);
    document.getElementById('editStandForm')?.addEventListener('submit', handleAddOrEditStand);
    document.getElementById('addStandButton')?.addEventListener('click', () => document.getElementById('addStandForm').reset());
    document.getElementById('exportStandExcelButton')?.addEventListener('click', exportStandsToExcel);

    document.getElementById('standTableBody')?.addEventListener('click', (e) => {
        const editButton = e.target.closest('.edit-stand-btn');
        if (editButton) setupEditStandForm(editButton.dataset.id);
        const deleteButton = e.target.closest('.delete-stand-btn');
        if (deleteButton) confirmDeleteStand(deleteButton.dataset.id);
    });
}

async function setupEditStandForm(standId) {
    try {
        const stand = await window.fetchWithToken(`/Stands/${standId}`);
        const form = document.getElementById('editStandForm');
        form.reset();

        // Modal başlığını ve ID'yi ayarla
        document.getElementById('editStandModalLabel').textContent = `Standı Düzenle: ${window.escapeHtml(stand.name)}`;
        document.getElementById('editStandId').value = stand.id;

        // Dropdown değerlerini ayarla
        document.getElementById('editStandFairId').value = stand.fairId || '';
        document.getElementById('editStandParticipantId').value = stand.participantId || '';

        // Diğer form alanlarını doldur
        document.getElementById('editStandNumber').value = stand.name || '';
        document.getElementById('editStandFairHall').value = stand.fairHall || '';
        document.getElementById('editStandContractArea').value = stand.contractArea || 0;
        document.getElementById('editStandUnitPrice').value = stand.unitPrice || 0;
        document.getElementById('editStandCurrency').value = stand.currency || 'TRY';
        document.getElementById('editStandSalesRepresentative').value = stand.salesRepresentative || '';
        document.getElementById('editStandNote').value = stand.note || '';

        new bootstrap.Modal(document.getElementById('editStandModal')).show();
    } catch (error) {
        console.error("Stand düzenleme formu yüklenirken hata:", error);
        Swal.fire('Hata', 'Stand bilgileri yüklenemedi.', 'error');
    }
}

async function handleAddOrEditStand(event) {
    event.preventDefault();
    const form = event.target;
    const standId = form.querySelector('input[type=hidden]').value;
    const formPrefix = form.id.startsWith('edit') ? 'edit' : 'add';

    const fairIdValue = document.getElementById(`${formPrefix}StandFairId`).value;
    const participantIdValue = document.getElementById(`${formPrefix}StandParticipantId`).value;

    // --- BAŞLANGIÇ: Sunucuya göndermeden önce doğrulama ---
    if (!fairIdValue) {
        Swal.fire('Eksik Bilgi', 'Lütfen bir fuar seçiniz.', 'warning');
        return;
    }
    if (!participantIdValue) {
        Swal.fire('Eksik Bilgi', 'Lütfen bir katılımcı seçiniz.', 'warning');
        return;
    }
    // --- BİTİŞ: Sunucuya göndermeden önce doğrulama ---

    const standData = {
        id: standId || null,
        fairId: fairIdValue,
        participantId: participantIdValue,
        name: document.getElementById(`${formPrefix}StandNumber`).value,
        fairHall: document.getElementById(`${formPrefix}StandFairHall`).value,
        contractArea: parseFloat(document.getElementById(`${formPrefix}StandContractArea`).value) || 0,
        unitPrice: parseFloat(document.getElementById(`${formPrefix}StandUnitPrice`).value) || 0,
        currency: document.getElementById(`${formPrefix}StandCurrency`).value,
        salesRepresentative: document.getElementById(`${formPrefix}StandSalesRepresentative`).value,
        note: document.getElementById(`${formPrefix}StandNote`).value,
    };

    const url = standId ? `/Stands/${standId}` : '/Stands';
    const method = standId ? 'PUT' : 'POST';

    try {
        await window.fetchWithToken(url, { method: method, body: JSON.stringify(standData) });
        Swal.fire('Başarılı!', `Stand başarıyla ${standId ? 'güncellendi' : 'eklendi'}.`, 'success');
        bootstrap.Modal.getInstance(form.closest('.modal')).hide();
        await loadStands(standId ? standListPageState.currentPage : 1);
    } catch (error) {
        console.error("Stand kaydedilirken hata:", error);
        Swal.fire('İşlem Başarısız', `Bir hata oluştu: ${error.message}`, 'error');
    }
}


function confirmDeleteStand(id) {
    Swal.fire({
        title: 'Emin misiniz?',
        text: "Bu standı silmek geri alınamaz!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        confirmButtonText: 'Evet, Sil!',
        cancelButtonText: 'İptal'
    }).then(async (result) => {
        if (result.isConfirmed) {
            try {
                await window.fetchWithToken(`/Stands/${id}`, { method: 'DELETE' });
                Swal.fire('Silindi!', 'Stand başarıyla silindi.', 'success');
                await loadStands(standListPageState.currentPage);
            } catch (error) {
                console.error("Stand silinirken hata:", error);
            }
        }
    });
}

async function exportStandsToExcel() {
    const filters = {
        pageNumber: 1,
        pageSize: 10000, // Tüm filtrelenmiş veriyi çekmek için büyük bir sayı
        fairId: document.getElementById('filterStandFair')?.value === 'all' ? null : document.getElementById('filterStandFair')?.value,
        participantId: document.getElementById('filterStandParticipant')?.value === 'all' ? null : document.getElementById('filterStandParticipant')?.value,
        standNo: document.getElementById('filterStandNo')?.value || null
    };

    try {
        const responseData = await window.fetchStands(filters);
        const allStands = responseData.stands;

        if (!allStands || allStands.length === 0) {
            Swal.fire('Bilgi', 'Excel\'e aktarılacak stand bulunamadı.', 'info');
            return;
        }

        const exportData = allStands.map(s => ({
            "Fuar Adı": window.getFairNameById(s.fairId),
            "Katılımcı Firma": window.getParticipantNameById(s.participantId),
            "Stand No": s.name,
            "Alan (m²)": s.contractArea,
            "Toplam Tutar (KDV Dahil)": s.totalAmountWithVAT,
            "Bakiye": s.balance,
            "Para Birimi": s.currency,
            "Satış Temsilcisi": s.salesRepresentative
        }));
        window.exportToExcel(exportData, 'StandListesi', 'Standlar');
    } catch (error) {
        console.error("Excel'e aktarılırken hata:", error);
    }
}

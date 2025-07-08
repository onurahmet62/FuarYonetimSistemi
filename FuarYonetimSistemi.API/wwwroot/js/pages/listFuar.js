// js/pages/listFuar.js (Yeniden Düzenlenmiş ve Silme Yöntemi Güncellenmiş)

// --- SAYFA DURUMU (PAGE STATE) ---
const fairListPageState = {
    currentPage: 1,
    totalPages: 1,
    totalRecords: 0,
    pageSize: 10,
    filteredData: [] // Sunucudan gelen filtrelenmiş veriyi tutmak için
};

// --- ANA FONKSİYONLAR ---

/**
 * Fuar listesi sayfasını başlatan ana fonksiyon.
 */
window.loadFairsPage = async function () {
    setupEventListeners();
    await loadFairs(1); // Sayfa ilk yüklendiğinde fuarları yükle.
};

/**
 * API'den fuarları filtreleyerek çeker ve tabloda görüntüler.
 * Bu fonksiyon sunucu taraflı filtreleme ve sayfalama kullanır.
 * @param {number} pageNumber - Yüklenecek sayfa numarası.
 */
async function loadFairs(pageNumber) {
    fairListPageState.currentPage = pageNumber;
    const pageSizeSelect = document.getElementById('pageSizeFair');
    fairListPageState.pageSize = parseInt(pageSizeSelect.value, 10);

    const filterPayload = {
        pageNumber: fairListPageState.currentPage,
        pageSize: fairListPageState.pageSize,
        name: document.getElementById('filterFairName').value || null,
        location: document.getElementById('filterFairLocation').value || null,
        categoryName: document.getElementById('filterFairCategory').value || null,
        startDate: document.getElementById('filterFairStartDate').value || null,
    };

    try {
        const data = await window.fetchWithToken('/Fair/filter', {
            method: 'POST',
            body: JSON.stringify(filterPayload)
        });

        fairListPageState.filteredData = window.processApiResponse(data.fairs || data.items || data);
        fairListPageState.totalRecords = data.totalCount || 0;
        fairListPageState.totalPages = Math.ceil(fairListPageState.totalRecords / fairListPageState.pageSize) || 1;

        renderFairsTable(fairListPageState.filteredData);
        updatePaginationInfo();

    } catch (error) {
        console.error("Fuar listesi yüklenirken hata oluştu:", error);
        document.getElementById('fairListTableBody').innerHTML = '<tr><td colspan="11" class="text-center text-danger">Fuarlar yüklenirken bir hata oluştu.</td></tr>';
        fairListPageState.totalRecords = 0;
        fairListPageState.totalPages = 1;
        updatePaginationInfo();
    }
}

/**
 * Fuar verilerini HTML tablosunda görüntüler.
 */
function renderFairsTable(fairs) {
    const tableBody = document.getElementById('fairListTableBody');
    tableBody.innerHTML = '';

    if (!fairs || fairs.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="11" class="text-center p-4">Gösterilecek fuar bulunamadı.</td></tr>';
        return;
    }

    fairs.forEach(fair => {
        const row = tableBody.insertRow();
        row.innerHTML = `
            <td>${window.escapeHtml(fair.name)}</td>
            <td>${window.escapeHtml(fair.location)}</td>
            <td>${window.escapeHtml(fair.country)}</td>
            <td>${fair.year || '-'}</td>
            <td>${window.escapeHtml(fair.organizer)}</td>
            <td>${window.formatDate(fair.startDate)}</td>
            <td>${window.formatDate(fair.endDate)}</td>
            <td>${window.escapeHtml(fair.categoryName)}</td>
            <td><a href="${fair.website || '#'}" target="_blank" class="text-decoration-none">${fair.website ? 'Ziyaret Et' : '-'}</a></td>
            <td>${fair.totalParticipantCount || 0}</td>
            <td>
                <button class="btn btn-sm btn-warning edit-fair-btn" data-id="${fair.id}" title="Düzenle"><i class="bi bi-pencil"></i></button>
                <button class="btn btn-sm btn-danger delete-fair-btn" data-id="${fair.id}" title="Sil"><i class="bi bi-trash"></i></button>
                <a href="fairdetails.html?id=${fair.id}"  class="btn btn-sm btn-primary" title="Detaylar"><i class="bi bi-eye"></i></a>
            </td>
        `;
    });
}

/**
 * Sayfalama kullanıcı arayüzünü günceller.
 */
function updatePaginationInfo() {
    window.updateGenericPaginationUI({
        currentPage: fairListPageState.currentPage,
        totalPages: fairListPageState.totalPages,
        totalRecords: fairListPageState.totalRecords,
        pageInfoId: 'currentPageFairDisplay',
        prevBtnId: 'fairPreviousPage',
        nextBtnId: 'fairNextPage',
        recordsInfoId: 'totalFairsCount'
    });

    const startItem = fairListPageState.totalRecords > 0 ? (fairListPageState.currentPage - 1) * fairListPageState.pageSize + 1 : 0;
    const endItem = Math.min(fairListPageState.currentPage * fairListPageState.pageSize, fairListPageState.totalRecords);
    const rangeDisplay = document.getElementById('currentFairsRange');
    if (rangeDisplay) {
        rangeDisplay.textContent = `${startItem} - ${endItem}`;
    }
}

/**
 * Sayfadaki tüm olay dinleyicilerini ayarlar.
 */
function setupEventListeners() {
    document.getElementById('fairForm')?.addEventListener('submit', handleFairFormSubmit);

    document.getElementById('filterFormFair')?.addEventListener('submit', (e) => {
        e.preventDefault();
        loadFairs(1);
    });
    document.getElementById('clearFilterFairBtn')?.addEventListener('click', () => {
        document.getElementById('filterFormFair').reset();
        loadFairs(1);
    });

    document.getElementById('pageSizeFair')?.addEventListener('change', () => loadFairs(1));

    document.getElementById('fairPreviousPage')?.addEventListener('click', () => {
        if (fairListPageState.currentPage > 1) {
            loadFairs(fairListPageState.currentPage - 1);
        }
    });
    document.getElementById('fairNextPage')?.addEventListener('click', () => {
        if (fairListPageState.currentPage < fairListPageState.totalPages) {
            loadFairs(fairListPageState.currentPage + 1);
        }
    });

    document.getElementById('addFairButton')?.addEventListener('click', setupNewFairForm);
    document.getElementById('exportFairsExcelBtn')?.addEventListener('click', exportFairsToExcel);

    document.getElementById('fairListTableBody')?.addEventListener('click', (e) => {
        const editButton = e.target.closest('.edit-fair-btn');
        if (editButton) {
            loadFairForEdit(editButton.dataset.id);
            return;
        }

        const deleteButton = e.target.closest('.delete-fair-btn');
        if (deleteButton) {
            const fairId = deleteButton.dataset.id;
            const fairName = deleteButton.closest('tr').cells[0].textContent;
            confirmDeleteFair(fairId, fairName);
        }
    });
}

/**
 * Fuar silme işlemini onaylar ve gerçekleştirir.
 * @param {string} id - Silinecek fuarın ID'si.
 * @param {string} fairName - Onay mesajı için fuarın adı.
 */
function confirmDeleteFair(id, fairName = 'Bilinmeyen Fuar') {
    Swal.fire({
        title: 'Emin misiniz?',
        text: `"${window.escapeHtml(fairName)}" adlı fuarı silmek istediğinizden emin misiniz? Bu işlem geri alınamaz!`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        confirmButtonText: 'Evet, Sil!',
        cancelButtonText: 'İptal'
    }).then(async (result) => {
        if (result.isConfirmed) {
            try {
                await window.fetchWithToken(`/Fair/${id}`, {
                    method: 'DELETE'
                });

                Swal.fire('Silindi!', 'Fuar başarıyla silindi.', 'success');

                const tableBody = document.getElementById('fairListTableBody');
                if (tableBody.rows.length === 1 && fairListPageState.currentPage > 1) {
                    await loadFairs(fairListPageState.currentPage - 1);
                } else {
                    await loadFairs(fairListPageState.currentPage);
                }
            } catch (error) {
                console.error("Fuar silinirken hata:", error);
            }
        }
    });
}

/**
 * Yeni fuar ekleme formunu hazırlar ve modalı açar.
 */
function setupNewFairForm() {
    document.getElementById('fairForm').reset();
    document.getElementById('addEditFairModalLabel').textContent = 'Yeni Fuar Ekle';
    document.getElementById('fairId').value = '';
    const addEditModal = new bootstrap.Modal(document.getElementById('addEditFairModal'));
    addEditModal.show();
}

/**
 * Fuar ekleme/düzenleme formunu işler.
 */
async function handleFairFormSubmit(event) {
    event.preventDefault();

    const fairId = document.getElementById('fairId').value;
    const fairData = {
        name: document.getElementById('fairName').value,
        location: document.getElementById('fairLocation').value,
        country: document.getElementById('fairCountry').value,
        year: parseInt(document.getElementById('fairYear').value, 10),
        startDate: document.getElementById('fairStartDate').value,
        endDate: document.getElementById('fairEndDate').value,
        organizer: document.getElementById('fairOrganizer').value,
        categoryName: document.getElementById('fairCategory').value,
        newCategoryName: document.getElementById('fairNewCategoryName').value,
        fairType: document.getElementById('fairType').value,
        website: document.getElementById('fairWebsite').value,
        email: document.getElementById('fairEmail').value,
        totalParticipantCount: parseInt(document.getElementById('fairTotalParticipantCount').value, 10) || 0,
        foreignParticipantCount: parseInt(document.getElementById('fairForeignParticipantCount').value, 10) || 0,
        totalVisitorCount: parseInt(document.getElementById('fairTotalVisitorCount').value, 10) || 0,
        foreignVisitorCount: parseInt(document.getElementById('fairForeignVisitorCount').value, 10) || 0,
        totalStandArea: parseFloat(document.getElementById('fairTotalStandArea').value) || 0,
        participatingCountries: document.getElementById('fairParticipatingCountries').value,
        budget: parseFloat(document.getElementById('fairBudget').value) || 0,
        revenueTarget: parseFloat(document.getElementById('fairRevenueTarget').value) || 0,
        expenseTarget: parseFloat(document.getElementById('fairExpenseTarget').value) || 0,
        netProfitTarget: parseFloat(document.getElementById('fairNetProfitTarget').value) || 0,
        actualRevenue: parseFloat(document.getElementById('fairActualRevenue').value) || 0,
        actualExpense: parseFloat(document.getElementById('fairActualExpense').value) || 0,
        actualNetProfit: parseFloat(document.getElementById('fairActualNetProfit').value) || 0
    };

    try {
        if (fairId) {
            await window.fetchWithToken(`/Fair/${fairId}`, {
                method: 'PUT',
                body: JSON.stringify({ id: fairId, ...fairData })
            });
            Swal.fire('Başarılı!', 'Fuar başarıyla güncellendi.', 'success');
        } else {
            await window.fetchWithToken('/Fair', {
                method: 'POST',
                body: JSON.stringify(fairData)
            });
            Swal.fire('Başarılı!', 'Yeni fuar başarıyla eklendi.', 'success');
        }

        bootstrap.Modal.getInstance(document.getElementById('addEditFairModal')).hide();
        await loadFairs(fairId ? fairListPageState.currentPage : 1);

    } catch (error) {
        console.error("Fuar kaydedilirken hata:", error);
    }
}

/**
 * Düzenlenecek fuarın verilerini yükler ve modalı doldurur.
 */
async function loadFairForEdit(fairId) {
    const formatDateForInput = (dateString) => {
        if (!dateString) return '';
        try {
            return new Date(dateString).toISOString().split('T')[0];
        } catch (e) {
            return '';
        }
    };

    try {
        const fair = fairListPageState.filteredData.find(f => f.id === fairId);
        if (!fair) {
            Swal.fire('Hata', 'Fuar detayı bulunamadı. Lütfen sayfayı yenileyin.', 'error');
            return;
        }

        document.getElementById('addEditFairModalLabel').textContent = 'Fuar Düzenle';
        document.getElementById('fairId').value = fair.id;
        document.getElementById('fairName').value = fair.name || '';
        document.getElementById('fairLocation').value = fair.location || '';
        document.getElementById('fairCountry').value = fair.country || '';
        document.getElementById('fairYear').value = fair.year || '';
        document.getElementById('fairStartDate').value = formatDateForInput(fair.startDate);
        document.getElementById('fairEndDate').value = formatDateForInput(fair.endDate);
        document.getElementById('fairOrganizer').value = fair.organizer || '';
        document.getElementById('fairCategory').value = fair.categoryName || '';
        document.getElementById('fairNewCategoryName').value = '';
        document.getElementById('fairType').value = fair.fairType || '';
        document.getElementById('fairWebsite').value = fair.website || '';
        document.getElementById('fairEmail').value = fair.email || '';
        document.getElementById('fairTotalParticipantCount').value = fair.totalParticipantCount || 0;
        document.getElementById('fairForeignParticipantCount').value = fair.foreignParticipantCount || 0;
        document.getElementById('fairTotalVisitorCount').value = fair.totalVisitorCount || 0;
        document.getElementById('fairForeignVisitorCount').value = fair.foreignVisitorCount || 0;
        document.getElementById('fairTotalStandArea').value = fair.totalStandArea || 0;
        document.getElementById('fairParticipatingCountries').value = fair.participatingCountries || '';
        document.getElementById('fairBudget').value = fair.budget || 0;
        document.getElementById('fairRevenueTarget').value = fair.revenueTarget || 0;
        document.getElementById('fairExpenseTarget').value = fair.expenseTarget || 0;
        document.getElementById('fairNetProfitTarget').value = fair.netProfitTarget || 0;
        document.getElementById('fairActualRevenue').value = fair.actualRevenue || 0;
        document.getElementById('fairActualExpense').value = fair.actualExpense || 0;
        document.getElementById('fairActualNetProfit').value = fair.actualNetProfit || 0;

        const editModal = new bootstrap.Modal(document.getElementById('addEditFairModal'));
        editModal.show();

    } catch (error) {
        console.error("Fuar düzenleme için yüklenirken hata:", error);
    }
}

/**
 * Fuar listesini Excel'e aktarır. Filtrelenmiş tüm sonuçları çeker.
 */
async function exportFairsToExcel() {
    try {
        const filterPayload = {
            pageNumber: 1,
            pageSize: 10000,
            name: document.getElementById('filterFairName').value || null,
            location: document.getElementById('filterFairLocation').value || null,
            categoryName: document.getElementById('filterFairCategory').value || null,
            startDate: document.getElementById('filterFairStartDate').value || null,
        };

        const data = await window.fetchWithToken('/Fair/filter', {
            method: 'POST',
            body: JSON.stringify(filterPayload)
        });
        const allFairs = window.processApiResponse(data.fairs || data.items || data);

        if (!allFairs || allFairs.length === 0) {
            Swal.fire('Bilgi', 'Excel\'e aktarılacak fuar bulunamadı.', 'info');
            return;
        }

        const exportData = allFairs.map(fair => ({
            "Fuar Adı": fair.name,
            "Konum": fair.location,
            "Ülke": fair.country,
            "Yıl": fair.year,
            "Başlangıç Tarihi": window.formatDate(fair.startDate),
            "Bitiş Tarihi": window.formatDate(fair.endDate),
            "Organizatör": fair.organizer,
            "Kategori": fair.categoryName,
        }));

        window.exportToExcel(exportData, 'FuarListesi', 'Fuarlar');

    } catch (error) {
        console.error("Excel'e aktarılırken hata:", error);
    }
}

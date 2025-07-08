// js/pages/listOfficeExpenses.js (Düzeltilmiş ve İyileştirilmiş)

// --- SAYFA DURUMU (PAGE STATE) ---
const officeExpenseListPageState = {
    currentPage: 1,
    totalPages: 1,
    totalRecords: 0,
    pageSize: 20
};

// --- ANA FONKSİYONLAR ---

/**
 * Ofis Giderleri sayfasını başlatan ana fonksiyon.
 */
window.loadOfficeExpensesPage = async function () {
    setupEventListeners();
    // Dropdown'ları doldur
    await window.loadOfficeExpenseTypesForDropdown('filterOfficeExpenseTypeSelect', 'Tüm Gider Tipleri', '', true);
    await window.loadOfficeExpenseTypesForDropdown('addOfficeExpenseTypeId', 'Gider Tipi Seçiniz...', '', false);
    await window.loadOfficeExpenseTypesForDropdown('editOfficeExpenseTypeId', 'Gider Tipi Seçiniz...', '', false);

    // Sayfa ilk yüklendiğinde tüm giderleri yükle
    await loadOfficeExpenses(1);
};

/**
 * Filtrelere göre ofis giderlerini API'den çeker ve görüntüler.
 * Sunucu taraflı filtreleme ve sayfalama kullanır.
 * @param {number} pageNumber - Yüklenecek sayfa numarası.
 */
async function loadOfficeExpenses(pageNumber = 1) {
    officeExpenseListPageState.currentPage = pageNumber;
    officeExpenseListPageState.pageSize = parseInt(document.getElementById('pageSizeOfficeExpense').value, 10);

    const filters = {
        pageNumber: officeExpenseListPageState.currentPage,
        pageSize: officeExpenseListPageState.pageSize,
        officeExpenseTypeId: document.getElementById('filterOfficeExpenseTypeSelect')?.value || null,
        description: document.getElementById('filterDescription')?.value || null,
        accountCode: document.getElementById('filterAccountCode')?.value || null,
        minAmount: document.getElementById('filterMinAmount')?.value || null,
        maxAmount: document.getElementById('filterMaxAmount')?.value || null,
        startDate: document.getElementById('filterStartDate')?.value || null,
        endDate: document.getElementById('filterEndDate')?.value || null,
    };

    try {
        const responseData = await window.fetchAllOfficeExpenses(true, filters);

        officeExpenseListPageState.totalRecords = responseData.totalCount;
        officeExpenseListPageState.totalPages = responseData.totalPages;

        renderOfficeExpensesTable(responseData.items);
        updatePaginationInfo();

    } catch (error) {
        console.error("Ofis giderleri yüklenirken hata:", error);
        document.getElementById('officeExpensesTableBody').innerHTML = `<tr><td colspan="6" class="text-center text-danger">Giderler yüklenirken bir hata oluştu.</td></tr>`;
        officeExpenseListPageState.totalRecords = 0;
        officeExpenseListPageState.totalPages = 1;
        updatePaginationInfo();
    }
}

/**
 * Verilen gider verilerini HTML tablosunda görüntüler.
 */
function renderOfficeExpensesTable(expenses) {
    const tableBody = document.getElementById('officeExpensesTableBody');
    tableBody.innerHTML = '';

    if (!expenses || expenses.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="6" class="text-center p-4">Gösterilecek ofis gideri bulunamadı.</td></tr>';
        return;
    }

    expenses.forEach(expense => {
        const row = tableBody.insertRow();
        const expenseTypeName = window.getOfficeExpenseTypeNameById(expense.officeExpenseTypeId) || '-';

        row.innerHTML = `
            <td>${window.escapeHtml(expenseTypeName)}</td>
            <td>${window.formatDate(expense.date)}</td>
            <td>${window.formatCurrency(expense.amount, expense.currency)}</td>
            <td>${window.escapeHtml(expense.description)}</td>
            <td>${window.escapeHtml(expense.accountCode)}</td>
            <td>
                <button class="btn btn-sm btn-warning edit-officeexpense-btn" data-id="${expense.id}" title="Düzenle"><i class="bi bi-pencil"></i></button>
                <button class="btn btn-sm btn-danger delete-officeexpense-btn" data-id="${expense.id}" title="Sil"><i class="bi bi-trash"></i></button>
            </td>
        `;
    });
}

/**
 * Sayfalama kullanıcı arayüzünü günceller.
 */
function updatePaginationInfo() {
    window.updateGenericPaginationUI({
        currentPage: officeExpenseListPageState.currentPage,
        totalPages: officeExpenseListPageState.totalPages,
        totalRecords: officeExpenseListPageState.totalRecords,
        pageInfoId: 'currentPageInfoOfficeExpense',
        prevBtnId: 'prevPageOfficeExpense',
        nextBtnId: 'nextPageOfficeExpense',
        recordsInfoId: 'recordCountOfficeExpense'
    });
}


// --- OLAY DİNLEYİCİLERİ VE İŞLEYİCİLER ---

function setupEventListeners() {
    const filterForm = document.getElementById('filterOfficeExpenseForm');

    // "Ara" butonu için
    filterForm?.addEventListener('submit', (e) => {
        e.preventDefault();
        loadOfficeExpenses(1);
    });

    // "Temizle" butonu için
    filterForm?.querySelector('#clearFilterButton')?.addEventListener('click', () => {
        filterForm.reset();
        loadOfficeExpenses(1);
    });

    // Anında filtreleme için olay dinleyicileri
    document.getElementById('filterOfficeExpenseTypeSelect')?.addEventListener('change', () => loadOfficeExpenses(1));
    document.getElementById('filterStartDate')?.addEventListener('change', () => loadOfficeExpenses(1));
    document.getElementById('filterEndDate')?.addEventListener('change', () => loadOfficeExpenses(1));

    // Sayfalama olayları
    document.getElementById('pageSizeOfficeExpense')?.addEventListener('change', () => loadOfficeExpenses(1));
    document.getElementById('prevPageOfficeExpense')?.addEventListener('click', () => {
        if (officeExpenseListPageState.currentPage > 1) loadOfficeExpenses(officeExpenseListPageState.currentPage - 1);
    });
    document.getElementById('nextPageOfficeExpense')?.addEventListener('click', () => {
        if (officeExpenseListPageState.currentPage < officeExpenseListPageState.totalPages) loadOfficeExpenses(officeExpenseListPageState.currentPage + 1);
    });

    // CRUD ve diğer butonlar
    document.getElementById('addOfficeExpenseForm')?.addEventListener('submit', handleAddOfficeExpense);
    document.getElementById('editOfficeExpenseForm')?.addEventListener('submit', handleEditOfficeExpense);
    document.getElementById('addOfficeExpenseTypeForm')?.addEventListener('submit', handleAddExpenseType);
    document.getElementById('exportOfficeExpensesExcel')?.addEventListener('click', exportOfficeExpensesToExcel);

    // Tablo üzerindeki butonlar için olay delegasyonu
    document.getElementById('officeExpensesTableBody')?.addEventListener('click', (e) => {
        const editBtn = e.target.closest('.edit-officeexpense-btn');
        if (editBtn) setupEditOfficeExpenseForm(editBtn.dataset.id);
        const deleteBtn = e.target.closest('.delete-officeexpense-btn');
        if (deleteBtn) confirmDeleteOfficeExpense(deleteBtn.dataset.id);
    });
}

// ... (handleAdd, handleEdit, confirmDelete gibi diğer tüm fonksiyonlar öncekiyle aynı kalabilir)
// ... Diğer fonksiyonlar (setupEdit, handleAdd, handleEdit, confirmDelete, handleAddExpenseType, exportToExcel) burada yer almalı
// Önceki versiyondan kopyalayarak devam edebiliriz.

async function setupEditOfficeExpenseForm(expenseId) {
    try {
        const response = await window.fetchWithToken(`/OfficeExpenses/${expenseId}`);
        const expense = await response.json();
        const form = document.getElementById('editOfficeExpenseForm');
        form.reset();
        document.getElementById('editOfficeExpenseId').value = expense.id;
        form.querySelector('#editOfficeExpenseTypeId').value = expense.officeExpenseTypeId || '';
        form.querySelector('#editAmount').value = expense.amount || '';
        form.querySelector('#editCurrency').value = expense.currency || 'TRY';
        form.querySelector('#editDate').value = expense.date ? expense.date.split('T')[0] : '';
        form.querySelector('#editDescription').value = expense.description || '';
        form.querySelector('#editAccountCode').value = expense.accountCode || '';
        const editModal = new bootstrap.Modal(document.getElementById('editOfficeExpenseModal'));
        editModal.show();
    } catch (error) {
        console.error("Ofis gideri düzenleme için yüklenirken hata:", error);
    }
}

async function handleAddOfficeExpense(event) {
    event.preventDefault();
    const form = event.target;
    const expenseData = {
        officeExpenseTypeId: form.querySelector('#addOfficeExpenseTypeId').value,
        date: form.querySelector('#addDate').value,
        amount: parseFloat(form.querySelector('#addAmount').value),
        currency: form.querySelector('#addCurrency').value,
        description: form.querySelector('#addDescription').value,
        accountCode: form.querySelector('#addAccountCode').value,
    };
    if (!expenseData.officeExpenseTypeId || !expenseData.date || isNaN(expenseData.amount)) {
        Swal.fire('Uyarı', 'Gider Tipi, Tarih ve Tutar alanları zorunludur.', 'warning');
        return;
    }
    try {
        await window.fetchWithToken('/OfficeExpenses', { method: 'POST', body: JSON.stringify(expenseData) });
        Swal.fire('Başarılı!', 'Ofis gideri başarıyla eklendi.', 'success');
        bootstrap.Modal.getInstance(form.closest('.modal')).hide();
        await loadOfficeExpenses(1);
    } catch (error) {
        console.error("Ofis gideri eklenirken hata:", error);
    }
}

async function handleEditOfficeExpense(event) {
    event.preventDefault();
    const form = event.target;
    const expenseId = form.querySelector('#editOfficeExpenseId').value;
    const expenseData = {
        id: expenseId,
        officeExpenseTypeId: form.querySelector('#editOfficeExpenseTypeId').value,
        date: form.querySelector('#editDate').value,
        amount: parseFloat(form.querySelector('#editAmount').value),
        currency: form.querySelector('#editCurrency').value,
        description: form.querySelector('#editDescription').value,
        accountCode: form.querySelector('#editAccountCode').value,
    };
    if (!expenseData.officeExpenseTypeId || !expenseData.date || isNaN(expenseData.amount)) {
        Swal.fire('Uyarı', 'Gider Tipi, Tarih ve Tutar alanları zorunludur.', 'warning');
        return;
    }
    try {
        await window.fetchWithToken(`/OfficeExpenses/${expenseId}`, { method: 'PUT', body: JSON.stringify(expenseData) });
        Swal.fire('Başarılı!', 'Ofis gideri başarıyla güncellendi.', 'success');
        bootstrap.Modal.getInstance(form.closest('.modal')).hide();
        await loadOfficeExpenses(officeExpenseListPageState.currentPage);
    } catch (error) {
        console.error("Ofis gideri güncellenirken hata:", error);
    }
}

function confirmDeleteOfficeExpense(id) {
    Swal.fire({
        title: 'Emin misiniz?',
        text: "Bu gideri silmek geri alınamaz!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        confirmButtonText: 'Evet, Sil!',
        cancelButtonText: 'İptal'
    }).then(async (result) => {
        if (result.isConfirmed) {
            try {
                await window.fetchWithToken(`/OfficeExpenses/${id}`, { method: 'DELETE' });
                Swal.fire('Silindi!', 'Gider başarıyla silindi.', 'success');
                await loadOfficeExpenses(officeExpenseListPageState.currentPage);
            } catch (error) {
                console.error("Gider silinirken hata:", error);
            }
        }
    });
}

async function handleAddExpenseType(event) {
    event.preventDefault();
    const typeName = document.getElementById('addOfficeExpenseTypeName')?.value.trim();
    if (!typeName) return;
    try {
        await window.fetchWithToken("/OfficeExpenses/types", { method: 'POST', body: JSON.stringify({ name: typeName }) });
        Swal.fire('Başarılı!', 'Yeni gider kalemi eklendi.', 'success');
        bootstrap.Modal.getInstance(document.getElementById('addOfficeExpenseTypeModal')).hide();
        await window.loadOfficeExpenseTypesForDropdown('filterOfficeExpenseTypeSelect', 'Tüm Gider Tipleri', '', true);
        await window.loadOfficeExpenseTypesForDropdown('addOfficeExpenseTypeId', 'Gider Tipi Seçiniz...', '', false);
        await window.loadOfficeExpenseTypesForDropdown('editOfficeExpenseTypeId', 'Gider Tipi Seçiniz...', '', false);
    } catch (error) {
        console.error("Gider kalemi eklenirken hata:", error);
    }
}

async function exportOfficeExpensesToExcel() {
    const filters = {
        pageNumber: 1,
        pageSize: 10000,
        officeExpenseTypeId: document.getElementById('filterOfficeExpenseTypeSelect')?.value || null,
        description: document.getElementById('filterDescription')?.value || null,
        accountCode: document.getElementById('filterAccountCode')?.value || null,
        minAmount: document.getElementById('filterMinAmount')?.value || null,
        maxAmount: document.getElementById('filterMaxAmount')?.value || null,
        startDate: document.getElementById('filterStartDate')?.value || null,
        endDate: document.getElementById('filterEndDate')?.value || null,
    };
    try {
        const responseData = await window.fetchAllOfficeExpenses(true, filters);
        const allFilteredExpenses = responseData.items;
        if (!allFilteredExpenses || allFilteredExpenses.length === 0) {
            Swal.fire('Bilgi', 'Excel\'e aktarılacak ofis gideri bulunamadı.', 'info');
            return;
        }
        const exportData = allFilteredExpenses.map(expense => ({
            "Gider Tipi": window.getOfficeExpenseTypeNameById(expense.officeExpenseTypeId),
            "Tarih": window.formatDate(expense.date),
            "Tutar": window.formatCurrency(expense.amount, expense.currency),
            "Açıklama": expense.description || '-',
            "Hesap Kodu": expense.accountCode || '-'
        }));
        window.exportToExcel(exportData, 'OfisGiderleriListesi', 'Ofis Giderleri');
    } catch (error) {
        console.error("Ofis giderleri Excel'e aktarılırken hata:", error);
    }
}

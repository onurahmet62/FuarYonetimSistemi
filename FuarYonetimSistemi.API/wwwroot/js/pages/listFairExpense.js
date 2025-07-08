// js/pages/listFairExpense.js (Yeniden Düzenlenmiş)

// --- SAYFA DURUMU (PAGE STATE) ---
const fairExpenseListPageState = {
    currentPage: 1,
    totalPages: 1,
    totalRecords: 0,
    pageSize: 20,
    allDataForSelectedFair: [], // Seçili fuara ait tüm giderleri tutar
    currentExpenseIdToEdit: null
};

// --- ANA FONKSİYONLAR ---

/**
 * Fuar Giderleri sayfasını başlatan ana fonksiyon.
 */
window.loadFairExpensesPage = async function () {
    setupEventListeners();
    // Dropdown'ları doldur
    await window.loadFairsForDropdown('filterFairExpenseFair', 'Fuar Seçiniz...', '', false);
    await window.loadFairExpenseTypesForDropdown('filterFairExpenseTypeId', 'Tüm Gider Türleri', '', true);
    await window.loadFairsForDropdown('addFairExpenseFairId', 'Fuar Seçiniz...', '', false);
    await window.loadFairExpenseTypesForDropdown('addFairExpenseTypeId', 'Gider Türü Seçiniz...', '', false);
    await window.loadFairsForDropdown('editFairExpenseFairId', 'Fuar Seçiniz...', '', false);
    await window.loadFairExpenseTypesForDropdown('editFairExpenseTypeId', 'Gider Türü Seçiniz...', '', false);

    // Başlangıçta tabloyu boş olarak render et
    renderFairExpensesTable([]);
    updatePaginationInfo();
};

/**
 * Seçilen filtreye göre fuar giderlerini API'den çeker ve görüntüler.
 */
async function loadFairExpenses() {
    const fairId = document.getElementById('filterFairExpenseFair')?.value;
    const tableBody = document.getElementById('fairExpenseTableBody');

    if (!fairId) {
        fairExpenseListPageState.allDataForSelectedFair = [];
        tableBody.innerHTML = '<tr><td colspan="9" class="text-center p-4">Lütfen bir fuar seçerek giderleri görüntüleyin.</td></tr>';
        fairExpenseListPageState.totalRecords = 0;
        updatePaginationInfo();
        return;
    }

    try {
        // core.js içindeki özel fetchFairExpenses fonksiyonunu kullanalım
        await window.fetchAllFairExpenses(true, { fairId: fairId });

        // AppState'ten veriyi al
        const expenseData = window.AppState.fairExpenses;

        // Diğer filtreleri uygula (client-side)
        const filterExpenseTypeId = document.getElementById('filterFairExpenseTypeId')?.value;
        const filterAccountCode = document.getElementById('filterFairExpenseAccountCode')?.value.toLowerCase();

        const filteredData = expenseData.filter(expense => {
            const matchesType = (filterExpenseTypeId && filterExpenseTypeId !== 'all') ? expense.expenseTypeId === filterExpenseTypeId : true;
            const matchesAccountCode = filterAccountCode ? expense.accountCode?.toLowerCase().includes(filterAccountCode) : true;
            return matchesType && matchesAccountCode;
        });

        fairExpenseListPageState.allDataForSelectedFair = filteredData;
        applyPaginationAndRender();

    } catch (error) {
        console.error("Fuar giderleri yüklenirken hata:", error);
        tableBody.innerHTML = `<tr><td colspan="9" class="text-center text-danger">Hata: ${error.message}</td></tr>`;
    }
}

/**
 * Sayfalama hesaplamalarını yapar ve tabloyu render eder.
 */
function applyPaginationAndRender() {
    const state = fairExpenseListPageState;
    state.totalRecords = state.allDataForSelectedFair.length;
    state.pageSize = parseInt(document.getElementById('pageSizeFairExpense').value, 10);
    state.totalPages = Math.ceil(state.totalRecords / state.pageSize) || 1;
    state.currentPage = Math.min(state.currentPage, state.totalPages) || 1;

    const startIndex = (state.currentPage - 1) * state.pageSize;
    const endIndex = startIndex + state.pageSize;
    const expensesToDisplay = state.allDataForSelectedFair.slice(startIndex, endIndex);

    renderFairExpensesTable(expensesToDisplay);
    updatePaginationInfo();
}


/**
 * Verilen gider verilerini HTML tablosunda görüntüler.
 */
function renderFairExpensesTable(expenses) {
    const tableBody = document.getElementById('fairExpenseTableBody');
    tableBody.innerHTML = '';

    if (expenses.length === 0) {
        if (!document.getElementById('filterFairExpenseFair')?.value) {
            tableBody.innerHTML = '<tr><td colspan="9" class="text-center p-4">Lütfen bir fuar seçerek giderleri görüntüleyin.</td></tr>';
        } else {
            tableBody.innerHTML = '<tr><td colspan="9" class="text-center p-4">Filtreye uygun gider bulunamadı.</td></tr>';
        }
        return;
    }

    expenses.forEach(expense => {
        const row = tableBody.insertRow();
        const fairName = window.getFairNameById(expense.fairId) || '-';
        const expenseTypeName = window.getFairExpenseTypeNameById(expense.expenseTypeId) || '-';

        row.innerHTML = `
            <td>${window.escapeHtml(fairName)}</td>
            <td>${window.escapeHtml(expenseTypeName)}</td>
            <td>${window.escapeHtml(expense.accountCode)}</td>
            <td>${window.formatCurrency(expense.annualTarget)}</td>
            <td>${window.formatCurrency(expense.annualActual)}</td>
            <td>${window.formatCurrency(expense.currentTarget)}</td>
            <td>${window.formatCurrency(expense.currentActual)}</td>
            <td>${window.formatCurrency(expense.realizedExpense)}</td>
            <td>
                <button class="btn btn-sm btn-warning edit-fairexpense-btn" data-id="${expense.id}" title="Düzenle"><i class="bi bi-pencil"></i></button>
                <button class="btn btn-sm btn-danger delete-fairexpense-btn" data-id="${expense.id}" title="Sil"><i class="bi bi-trash"></i></button>
            </td>
        `;
    });
}

/**
 * Sayfalama kullanıcı arayüzünü günceller.
 */
function updatePaginationInfo() {
    window.updateGenericPaginationUI({
        currentPage: fairExpenseListPageState.currentPage,
        totalPages: fairExpenseListPageState.totalPages,
        totalRecords: fairExpenseListPageState.totalRecords,
        pageInfoId: 'currentPageFairExpense',
        prevBtnId: 'prevPageFairExpense',
        nextBtnId: 'nextPageFairExpense',
        recordsInfoId: 'recordCountFairExpense'
    });
}


// --- OLAY DİNLEYİCİLERİ VE İŞLEYİCİLER ---

function setupEventListeners() {
    const filterForm = document.getElementById('filterFormFairExpense');
    filterForm?.addEventListener('submit', (e) => {
        e.preventDefault();
        fairExpenseListPageState.currentPage = 1;
        loadFairExpenses();
    });

    document.getElementById('filterFairExpenseFair')?.addEventListener('change', () => {
        fairExpenseListPageState.currentPage = 1;
        loadFairExpenses();
    });

    document.getElementById('pageSizeFairExpense')?.addEventListener('change', () => {
        fairExpenseListPageState.currentPage = 1;
        applyPaginationAndRender();
    });

    document.getElementById('prevPageFairExpense')?.addEventListener('click', () => {
        if (fairExpenseListPageState.currentPage > 1) {
            fairExpenseListPageState.currentPage--;
            applyPaginationAndRender();
        }
    });
    document.getElementById('nextPageFairExpense')?.addEventListener('click', () => {
        if (fairExpenseListPageState.currentPage < fairExpenseListPageState.totalPages) {
            fairExpenseListPageState.currentPage++;
            applyPaginationAndRender();
        }
    });

    document.getElementById('addFairExpenseForm')?.addEventListener('submit', handleAddOrEditFairExpense);
    document.getElementById('editFairExpenseForm')?.addEventListener('submit', handleAddOrEditFairExpense);
    document.getElementById('addExpenseTypeForm')?.addEventListener('submit', handleAddExpenseType);

    document.getElementById('fairExpenseTableBody')?.addEventListener('click', (e) => {
        const editBtn = e.target.closest('.edit-fairexpense-btn');
        if (editBtn) setupEditFairExpenseForm(editBtn.dataset.id);

        const deleteBtn = e.target.closest('.delete-fairexpense-btn');
        if (deleteBtn) confirmDeleteFairExpense(deleteBtn.dataset.id);
    });
}

async function setupEditFairExpenseForm(expenseId) {
    const expense = fairExpenseListPageState.allDataForSelectedFair.find(e => e.id === expenseId);
    if (!expense) {
        Swal.fire('Hata', 'Gider detayı bulunamadı.', 'error');
        return;
    }

    const form = document.getElementById('editFairExpenseForm');
    form.reset();
    document.getElementById('editFairExpenseId').value = expense.id;
    form.querySelector('#editFairExpenseFairId').value = expense.fairId || '';
    form.querySelector('#editFairExpenseTypeId').value = expense.expenseTypeId || '';
    form.querySelector('#editFairExpenseAccountCode').value = expense.accountCode || '';
    form.querySelector('#editFairExpenseAnnualTarget').value = expense.annualTarget || 0;
    // ... diğer form alanları
    form.querySelector('#editFairExpenseRealizedExpense').value = expense.realizedExpense || 0;

    const editModal = new bootstrap.Modal(document.getElementById('editFairExpenseModal'));
    editModal.show();
}

async function handleAddOrEditFairExpense(event) {
    event.preventDefault();
    const form = event.target;
    const expenseId = form.querySelector('input[type=hidden]').value;

    const expenseData = {
        fairId: form.querySelector('[id*="FairId"]').value,
        expenseTypeId: form.querySelector('[id*="TypeId"]').value,
        accountCode: form.querySelector('[id*="AccountCode"]').value,
        annualTarget: parseFloat(form.querySelector('[id*="AnnualTarget"]').value),
        realizedExpense: parseFloat(form.querySelector('[id*="RealizedExpense"]').value)
        // ... diğer alanlar
    };

    try {
        if (expenseId) {
            await window.fetchWithToken(`/FairExpenses/${expenseId}`, {
                method: 'PUT',
                body: JSON.stringify({ id: expenseId, ...expenseData })
            });
            Swal.fire('Başarılı!', 'Fuar gideri güncellendi.', 'success');
        } else {
            await window.fetchWithToken('/FairExpenses', {
                method: 'POST',
                body: JSON.stringify(expenseData)
            });
            Swal.fire('Başarılı!', 'Fuar gideri eklendi.', 'success');
        }

        bootstrap.Modal.getInstance(form.closest('.modal')).hide();
        await loadFairExpenses(); // Listeyi yenile
    } catch (error) {
        console.error("Fuar gideri kaydedilirken hata:", error);
    }
}

function confirmDeleteFairExpense(id) {
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
                await window.fetchWithToken(`/FairExpenses/${id}`, { method: 'DELETE' });
                Swal.fire('Silindi!', 'Gider başarıyla silindi.', 'success');
                await loadFairExpenses();
            } catch (error) {
                console.error("Gider silinirken hata:", error);
            }
        }
    });
}

async function handleAddExpenseType(event) {
    event.preventDefault();
    const typeName = document.getElementById('addExpenseTypeName')?.value.trim();
    if (!typeName) return;

    try {
        await window.fetchWithToken("/FairExpenses/expense-type", {
            method: 'POST',
            body: JSON.stringify({ name: typeName })
        });
        Swal.fire('Başarılı!', 'Yeni gider türü eklendi.', 'success');
        bootstrap.Modal.getInstance(document.getElementById('addExpenseTypeModal')).hide();
        // Dropdown'ları yeniden yükle
        await window.loadFairExpenseTypesForDropdown('filterFairExpenseTypeId', 'Tüm Gider Türleri', '', true);
        await window.loadFairExpenseTypesForDropdown('addFairExpenseTypeId', 'Gider Türü Seçiniz...', '', false);
        await window.loadFairExpenseTypesForDropdown('editFairExpenseTypeId', 'Gider Türü Seçiniz...', '', false);
    } catch (error) {
        console.error("Gider türü eklenirken hata:", error);
    }
}

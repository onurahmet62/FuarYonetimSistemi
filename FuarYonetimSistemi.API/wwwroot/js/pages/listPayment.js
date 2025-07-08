// js/pages/listPayment.js (Nihai Düzeltilmiş Versiyon)

const paymentListPageState = {
    currentPage: 1,
    totalPages: 1,
    totalRecords: 0,
    pageSize: 20,
    // Sunucudan gelen filtrelenmiş veriyi saklamak için
    filteredPayments: []
};

window.loadPaymentsPage = async function () {
    console.log("loadPaymentsPage() başladı.");
    setupEventListeners();
    // Dropdown'ları ve ilişkisel verileri yükle
    await Promise.all([
        window.loadFairsForDropdown('filterPaymentFair', 'Tüm Fuarlar', '', true),
        window.loadStandsForDropdown('addPaymentStandId', 'Stand Seçiniz...', '', false),
        window.loadStandsForDropdown('editPaymentStandId', 'Stand Seçiniz...', '', false),
        window.fetchAllParticipants(true), // Katılımcı isimleri için gerekli
    ]);
    await loadPayments(1);
};

async function loadPayments(pageNumber = 1) {
    paymentListPageState.currentPage = pageNumber;
    paymentListPageState.pageSize = parseInt(document.getElementById('pageSizePayment').value, 10);

    const filters = {
        pageNumber: paymentListPageState.currentPage,
        pageSize: paymentListPageState.pageSize,
        fairId: document.getElementById('filterPaymentFair')?.value,
        paymentMethod: document.getElementById('filterPaymentMethod')?.value,
        paymentDate: document.getElementById('filterPaymentDate')?.value,
        receivedBy: document.getElementById('filterReceivedBy')?.value,
    };

    try {
        // core.js'teki fetchAllPayments fonksiyonu artık sunucudan filtrelenmiş listeyi getiriyor.
        const responseData = await window.fetchAllPayments(true, filters);

        paymentListPageState.filteredPayments = responseData.items; // Gelen veriyi sakla
        paymentListPageState.totalRecords = responseData.totalCount;
        paymentListPageState.totalPages = responseData.totalPages;

        renderPaymentsTable(); // Render fonksiyonu artık state'deki veriyi kullanacak
        updatePaginationInfo();

    } catch (error) {
        console.error("Ödemeler yüklenirken hata:", error);
        document.getElementById('paymentTableBody').innerHTML = '<tr><td colspan="9" class="text-center p-4 text-danger">Ödemeler yüklenirken bir hata oluştu.</td></tr>';
    }
}

function renderPaymentsTable() {
    const tableBody = document.getElementById('paymentTableBody');
    tableBody.innerHTML = '';

    const paymentsToDisplay = paymentListPageState.filteredPayments;

    if (!paymentsToDisplay || paymentsToDisplay.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="9" class="text-center p-4">Gösterilecek ödeme bulunamadı.</td></tr>';
        return;
    }

    paymentsToDisplay.forEach(payment => {
        // --- ÖNEMLİ DÜZELTME: İlişkisel verileri AppState'ten bulma ---
        const stand = window.getStandById(payment.standId);
        const participantName = stand ? window.getParticipantNameById(stand.participantId) : 'Bilinmiyor';
        const fairName = stand ? window.getFairNameById(stand.fairId) : 'Bilinmiyor';
        const standNo = stand ? (stand.standNo || stand.name || '-') : '-';

        const row = tableBody.insertRow();
        // API'den gelen 'id' alanını kullan
        const paymentId = payment.id;

        row.innerHTML = `
            <td>${window.escapeHtml(participantName)}</td>
            <td>${window.escapeHtml(fairName)}</td>
            <td>${window.escapeHtml(standNo)}</td>
            <td>${window.formatDate(payment.paymentDate)}</td>
            <td>${window.formatCurrency(payment.amount, payment.currency)}</td>
            <td>${window.escapeHtml(payment.paymentMethod)}</td>
            <td>${window.escapeHtml(payment.paymentDescription)}</td>
            <td>${window.escapeHtml(payment.receivedBy)}</td>
            <td>
                <button class="btn btn-sm btn-warning edit-payment-btn" data-id="${paymentId}" title="Düzenle"><i class="bi bi-pencil"></i></button>
                <button class="btn btn-sm btn-danger delete-payment-btn" data-id="${paymentId}" title="Sil"><i class="bi bi-trash"></i></button>
            </td>
        `;
    });
}


function updatePaginationInfo() {
    window.updateGenericPaginationUI({
        currentPage: paymentListPageState.currentPage,
        totalPages: paymentListPageState.totalPages,
        totalRecords: paymentListPageState.totalRecords,
        pageInfoId: 'currentPageInfoPayment',
        prevBtnId: 'prevPagePayment',
        nextBtnId: 'nextPagePayment',
        recordsInfoId: 'recordCountPayment'
    });
}

function setupEventListeners() {
    const filterForm = document.getElementById('filterFormPayment');
    filterForm?.addEventListener('submit', (e) => { e.preventDefault(); loadPayments(1); });
    filterForm?.querySelector('.btn-secondary')?.addEventListener('click', () => { filterForm.reset(); loadPayments(1); });

    document.getElementById('pageSizePayment')?.addEventListener('change', () => loadPayments(1));
    document.getElementById('prevPagePayment')?.addEventListener('click', () => { if (paymentListPageState.currentPage > 1) loadPayments(paymentListPageState.currentPage - 1); });
    document.getElementById('nextPagePayment')?.addEventListener('click', () => { if (paymentListPageState.currentPage < paymentListPageState.totalPages) loadPayments(paymentListPageState.currentPage + 1); });

    document.getElementById('addPaymentForm')?.addEventListener('submit', handleAddOrEditPayment);
    document.getElementById('editPaymentForm')?.addEventListener('submit', handleAddOrEditPayment);
    document.getElementById('addPaymentButton')?.addEventListener('click', () => document.getElementById('addPaymentForm').reset());

    document.getElementById('paymentTableBody')?.addEventListener('click', (e) => {
        const editButton = e.target.closest('.edit-payment-btn');
        if (editButton) setupEditPaymentForm(editButton.dataset.id);
        const deleteButton = e.target.closest('.delete-payment-btn');
        if (deleteButton) confirmDeletePayment(deleteButton.dataset.id);
    });
}

async function setupEditPaymentForm(paymentId) {
    try {
        // Detay endpoint'ini kullanmak yerine, AppState veya ana listeden bulmak daha verimli olabilir.
        // Ama API'den en güncel veriyi çekmek daha güvenli.
        const response = await window.fetchWithToken(`/Payments/${paymentId}`); // Basit get isteği
        const payment = await response.json();

        const form = document.getElementById('editPaymentForm');
        form.reset();
        form.querySelector('#editPaymentId').value = payment.id;
        await window.loadStandsForDropdown('editPaymentStandId', 'Stand Seçiniz...', payment.standId, false);
        form.querySelector('#editPaymentAmount').value = payment.amount;
        form.querySelector('#editPaymentCurrency').value = payment.currency || 'TRY';
        form.querySelector('#editPaymentDate').value = payment.paymentDate ? new Date(payment.paymentDate).toISOString().split('T')[0] : '';
        form.querySelector('#editPaymentMethod').value = payment.paymentMethod;
        form.querySelector('#editReceivedBy').value = payment.receivedBy;
        form.querySelector('#editPaymentDescription').value = payment.paymentDescription;

        new bootstrap.Modal(document.getElementById('editPaymentModal')).show();
    } catch (error) {
        console.error("Ödeme detayları yüklenirken hata:", error);
    }
}

async function handleAddOrEditPayment(event) {
    event.preventDefault();
    const form = event.target;
    const paymentId = form.querySelector('input[type=hidden]').value;

    const paymentData = {
        standId: form.querySelector('[id*="PaymentStandId"]').value,
        paymentDate: form.querySelector('[id*="PaymentDate"]').value,
        amount: parseFloat(form.querySelector('[id*="PaymentAmount"]').value),
        currency: form.querySelector('[id*="PaymentCurrency"]').value,
        paymentMethod: form.querySelector('[id*="PaymentMethod"]').value,
        receivedBy: form.querySelector('[id*="ReceivedBy"]').value,
        paymentDescription: form.querySelector('[id*="PaymentDescription"]').value,
    };

    try {
        if (paymentId) {
            await window.fetchWithToken(`/Payments/${paymentId}`, { method: 'PUT', body: JSON.stringify({ id: paymentId, ...paymentData }) });
            Swal.fire('Başarılı!', 'Ödeme güncellendi.', 'success');
        } else {
            await window.fetchWithToken('/Payments', { method: 'POST', body: JSON.stringify(paymentData) });
            Swal.fire('Başarılı!', 'Ödeme eklendi.', 'success');
        }
        bootstrap.Modal.getInstance(form.closest('.modal')).hide();
        await loadPayments(paymentId ? paymentListPageState.currentPage : 1);
    } catch (error) {
        console.error("Ödeme kaydedilirken hata:", error);
    }
}

function confirmDeletePayment(id) {
    Swal.fire({
        title: 'Emin misiniz?', text: "Bu ödemeyi silmek geri alınamaz!", icon: 'warning', showCancelButton: true, confirmButtonColor: '#d33', confirmButtonText: 'Evet, Sil!', cancelButtonText: 'İptal'
    }).then(async (result) => {
        if (result.isConfirmed) {
            try {
                await window.fetchWithToken(`/Payments/${id}`, { method: 'DELETE' });
                Swal.fire('Silindi!', 'Ödeme başarıyla silindi.', 'success');
                await loadPayments(paymentListPageState.currentPage);
            } catch (error) {
                console.error("Ödeme silinirken hata:", error);
            }
        }
    });
}

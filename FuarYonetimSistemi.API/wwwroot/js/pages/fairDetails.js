// dosya: js/pages/fairDetails.js

/**
 * Fuar detay sayfasını başlatan ana fonksiyon.
 */
window.loadFairDetailsPage = async function (fairId) {
    if (!fairId) {
        Swal.fire('Hata', 'Fuar ID bulunamadı.', 'error').then(() => window.location.href = 'listfuar.html');
        return;
    }

    try {
        // Gerekli tüm verileri paralel olarak çek.
        await Promise.all([
            window.fetchAllParticipants(true),
            window.fetchAllFairExpenseTypes(true),
            window.fetchAllPayments(true)
        ]);

        // Fuarın ana verilerini çekiyoruz.
        const [fair, standsResult, expensesResponse] = await Promise.all([
            window.fetchWithToken(`/Fair/${fairId}`),
            window.fetchWithToken('/Stands/filter', {
                method: 'POST',
                body: JSON.stringify({ fairId: fairId, pageSize: 1000 })
            }),
            window.fetchWithToken(`/FairExpenses/fair/${fairId}`)
        ]);

        const expenses = window.processApiResponse(expensesResponse);
        const stands = window.processApiResponse(standsResult.items || standsResult);

        // --- HESAPLAMALAR ---
        const standIdsForThisFair = new Set(stands.map(s => s.id));
        const paymentsForThisFair = window.AppState.payments.filter(p => standIdsForThisFair.has(p.standId));
        const calculatedRevenue = paymentsForThisFair.reduce((sum, payment) => sum + (payment.amount || 0), 0);
        const calculatedExpense = expenses.reduce((sum, expense) => sum + (expense.realizedExpense || 0), 0);
        const netProfit = calculatedRevenue - calculatedExpense;
        const totalArea = stands.reduce((sum, stand) => sum + (stand.contractArea || 0), 0);
        const totalPossibleArea = fair.totalArea || totalArea || 1; // 0'a bölünmeyi engelle
        const occupancyRate = totalArea / totalPossibleArea * 100;

        // AppState'i bu fuara özel verilerle güncelleyelim
        window.AppState.currentFair = fair;
        window.AppState.currentStands = stands;
        window.AppState.fairExpenses = expenses;
        window.AppState.fairPayments = paymentsForThisFair;

        populatePage(fair, stands, expenses, paymentsForThisFair, {
            revenue: calculatedRevenue,
            expense: calculatedExpense,
            netProfit: netProfit,
            occupancy: occupancyRate
        });

        setupEventListeners();

    } catch (error) {
        console.error("Fuar detayları yüklenirken kritik bir hata oluştu:", error);
        Swal.fire('Hata!', error.message || 'Fuar bilgileri yüklenirken bir sorun oluştu.', 'error');
    }
};

/**
 * Sayfadaki tüm UI elementlerini gelen ve hesaplanan verilerle doldurur.
 */
function populatePage(fair, stands, expenses, payments, calculatedStats) {
    document.getElementById('fairNameTitle').textContent = window.escapeHtml(fair.name);
    document.title = `Ardesk - ${window.escapeHtml(fair.name)}`;
    populateInfoCard(fair);
    populateStatCards(stands, calculatedStats);

    // Tabloları başlat
    initializeParticipantsTable(stands);
    initializeStandsTable(stands);
    initializeExpensesTable(expenses);
    initializePaymentsTable(payments);
}

/**
 * Sağdaki fuar bilgi kartını doldurur.
 */
function populateInfoCard(fair) {
    document.getElementById('infoOrganizer').textContent = window.escapeHtml(fair.organizer || '-');
    document.getElementById('infoCategory').textContent = window.escapeHtml(fair.categoryName || '-');
    document.getElementById('infoDates').textContent = `${window.formatDate(fair.startDate)} - ${window.formatDate(fair.endDate)}`;

    const websiteLink = document.getElementById('infoWebsite');
    if (fair.website) {
        websiteLink.href = fair.website.startsWith('http') ? fair.website : `https://${fair.website}`;
        websiteLink.textContent = fair.website;
        websiteLink.style.display = 'inline-block';
    } else {
        websiteLink.style.display = 'none';
        websiteLink.textContent = '-';
    }

    document.getElementById('infoRevenueTarget').textContent = window.formatCurrency(fair.revenueTarget);
    document.getElementById('infoExpenseTarget').textContent = window.formatCurrency(fair.expenseTarget);
    document.getElementById('infoNetProfitTarget').textContent = window.formatCurrency(fair.netProfitTarget);
}

/**
 * Üstteki istatistik kartlarını doldurur.
 */
function populateStatCards(stands, calculatedStats) {
    const uniqueParticipantIds = [...new Set(stands.map(s => s.participantId).filter(id => id))];

    document.getElementById('totalParticipants').textContent = uniqueParticipantIds.length;
    document.getElementById('totalStands').textContent = stands.length;
    document.getElementById('actualRevenue').textContent = window.formatCurrency(calculatedStats.revenue);
    document.getElementById('actualExpense').textContent = window.formatCurrency(calculatedStats.expense);
    document.getElementById('netProfit').textContent = window.formatCurrency(calculatedStats.netProfit);

    const occupancyRateEl = document.getElementById('occupancyRate');
    const occupancyBarEl = document.getElementById('occupancyProgressBar');
    const rate = Math.min(100, Math.max(0, calculatedStats.occupancy || 0));
    occupancyRateEl.textContent = `${rate.toFixed(1)}%`;
    occupancyBarEl.style.width = `${rate}%`;
    occupancyBarEl.setAttribute('aria-valuenow', rate);
}

function setupEventListeners() {
    const editFairForm = document.getElementById('editFairForm');
    if (editFairForm) {
        editFairForm.addEventListener('submit', handleUpdateFair);
    }

    const editModal = document.getElementById('editFairModal');
    if (editModal) {
        editModal.addEventListener('show.bs.modal', populateEditFairModal);
    }

    const revenueTargetInput = document.getElementById('editFairRevenueTarget');
    const expenseTargetInput = document.getElementById('editFairExpenseTarget');
    if (revenueTargetInput && expenseTargetInput) {
        revenueTargetInput.addEventListener('input', updateNetProfitTarget);
        expenseTargetInput.addEventListener('input', updateNetProfitTarget);
    }
}

function populateEditFairModal() {
    const fair = window.AppState.currentFair;
    if (!fair) return;

    document.getElementById('editFairId').value = fair.id;
    document.getElementById('editFairName').value = fair.name || '';
    document.getElementById('editFairOrganizer').value = fair.organizer || '';
    document.getElementById('editFairCategory').value = fair.categoryName || '';
    document.getElementById('editFairWebsite').value = fair.website || '';
    document.getElementById('editFairStartDate').value = fair.startDate ? new Date(fair.startDate).toISOString().split('T')[0] : '';
    document.getElementById('editFairEndDate').value = fair.endDate ? new Date(fair.endDate).toISOString().split('T')[0] : '';
    document.getElementById('editFairRevenueTarget').value = fair.revenueTarget || 0;
    document.getElementById('editFairExpenseTarget').value = fair.expenseTarget || 0;
    updateNetProfitTarget();
}

function updateNetProfitTarget() {
    const revenueTarget = parseFloat(document.getElementById('editFairRevenueTarget').value) || 0;
    const expenseTarget = parseFloat(document.getElementById('editFairExpenseTarget').value) || 0;
    document.getElementById('editFairNetProfitTarget').value = (revenueTarget - expenseTarget).toFixed(2);
}

async function handleUpdateFair(event) {
    event.preventDefault();
    const fairId = document.getElementById('editFairId').value;

    const fairData = {
        id: fairId,
        name: document.getElementById('editFairName').value,
        organizer: document.getElementById('editFairOrganizer').value,
        categoryName: document.getElementById('editFairCategory').value,
        website: document.getElementById('editFairWebsite').value,
        startDate: document.getElementById('editFairStartDate').value,
        endDate: document.getElementById('editFairEndDate').value,
        revenueTarget: parseFloat(document.getElementById('editFairRevenueTarget').value) || 0,
        expenseTarget: parseFloat(document.getElementById('editFairExpenseTarget').value) || 0,
        netProfitTarget: parseFloat(document.getElementById('editFairNetProfitTarget').value) || 0
    };

    try {
        // NİHAİ ÇÖZÜM: Sunucunun hata vermediği tek kombinasyon olan POST /Fair kullanılıyor.
        await window.fetchWithToken(`/Fair`, {
            method: 'POST',
            body: JSON.stringify(fairData)
        });
        Swal.fire('Başarılı!', 'Fuar bilgileri güncellendi.', 'success');
        bootstrap.Modal.getInstance(document.getElementById('editFairModal')).hide();
        window.loadFairDetailsPage(fairId); // Sayfayı yenile
    } catch (error) {
        Swal.fire('Hata!', `Fuar güncellenirken bir hata oluştu: ${error.message}`, 'error');
    }
}

function initializeParticipantsTable(stands) {
    const participantMap = new Map();
    stands.forEach(stand => {
        if (stand.participantId && !participantMap.has(stand.participantId)) {
            const participant = window.AppState.participants.find(p => p.id === stand.participantId);
            if (participant) participantMap.set(participant.id, participant);
        }
    });
    const participants = Array.from(participantMap.values());

    $('#dataTableParticipants').DataTable({
        data: participants,
        destroy: true,
        columns: [
            { title: 'Firma Adı', data: 'companyName', render: (data, type, row) => `<a href="participantDetails.html?id=${row.id}">${window.escapeHtml(data)}</a>` },
            { title: 'Yetkili', data: 'fullName', defaultContent: '-' },
            { title: 'E-posta', data: 'email', defaultContent: '-' },
            { title: 'Telefon', data: 'phone', defaultContent: '-' }
        ],
        language: { url: "https://cdn.datatables.net/plug-ins/1.13.6/i18n/tr.json" },
        order: [[0, 'asc']]
    });
}

function initializeStandsTable(stands) {
    $('#dataTableStands').DataTable({
        data: stands,
        destroy: true,
        columns: [
            { title: 'Stand No', data: 'name', render: (data, type, row) => window.escapeHtml(data || row.standNo || '-') },
            { title: 'Katılımcı', data: 'participantId', render: (data, type, row) => `<a href="participantDetails.html?id=${data}">${window.escapeHtml(window.getParticipantNameById(data))}</a>` },
            { title: 'Alan (m²)', data: 'contractArea', render: data => `${data || 0} m²` },
            { title: 'Sözleşme Tutarı', data: 'totalAmountWithVAT', render: data => window.formatCurrency(data || 0) },
            { title: 'Bakiye', data: 'balance', render: data => window.formatCurrency(data || 0) }
        ],
        language: { url: "https://cdn.datatables.net/plug-ins/1.13.6/i18n/tr.json" },
        order: [[0, 'asc']]
    });
}

function initializeExpensesTable(expenses) {
    $('#dataTableExpenses').DataTable({
        data: expenses,
        destroy: true,
        columns: [
            { title: 'Gider Tipi', data: 'expenseTypeId', render: data => window.escapeHtml(window.getFairExpenseTypeNameById(data)) },
            { title: 'Açıklama', data: 'description', defaultContent: '-' },
            { title: 'Tutar', data: 'realizedExpense', render: data => window.formatCurrency(data || 0) }
        ],
        language: { url: "https://cdn.datatables.net/plug-ins/1.13.6/i18n/tr.json" },
        order: [[2, 'desc']]
    });
}

function initializePaymentsTable(payments) {
    $('#dataTablePayments').DataTable({
        data: payments,
        destroy: true,
        columns: [
            {
                title: 'Katılımcı', data: 'standId', render: (data, type, row) => {
                    const stand = window.AppState.currentStands.find(s => s.id === data);
                    return stand ? `<a href="participantDetails.html?id=${stand.participantId}">${window.escapeHtml(window.getParticipantNameById(stand.participantId))}</a>` : '-';
                }
            },
            { title: 'Tarih', data: 'paymentDate', render: data => window.formatDate(data) },
            { title: 'Tutar', data: 'amount', render: (data, type, row) => window.formatCurrency(data || 0, row.currency) },
            { title: 'Ödeme Yöntemi', data: 'paymentMethod' },
            { title: 'Açıklama', data: 'paymentDescription', defaultContent: '-' }
        ],
        language: { url: "https://cdn.datatables.net/plug-ins/1.13.6/i18n/tr.json" },
        order: [[1, 'desc']]
    });
}

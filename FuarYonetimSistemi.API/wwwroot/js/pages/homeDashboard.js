// js/pages/homeDashboard.js (Filtreleme Özellikli ve Hata Düzeltmeli Nihai Sürüm)

/**
 * Anasayfa yüklendiğinde çalışacak ana fonksiyon.
 */
window.loadHomePage = async function () {
    console.log("Kapsamlı Dashboard sayfası yükleniyor...");
    setupDashboardEventListeners();

    // Gerekli temel verileri (fuar, stand, katılımcı listeleri) önceden yükle.
    // Bu, diğer fonksiyonların bu verilere hızlıca erişmesini sağlar.
    await Promise.all([
        window.fetchAllFairs(true),
        window.fetchAllStands(true),
        window.fetchAllParticipants(true)
    ]);

    // Filtre dropdown'ını doldur
    await window.loadFairsForDropdown('dashboardFairFilter', 'Tüm Fuarlar', 'all', false);

    // Sayfa ilk yüklendiğinde tüm verilerle dashboard'u oluştur
    await loadDashboardData();
};

/**
 * Dashboard için olay dinleyicilerini ayarlar.
 */
function setupDashboardEventListeners() {
    const filters = ['dashboardFairFilter', 'dashboardStartDate', 'dashboardEndDate'];
    filters.forEach(id => {
        const element = document.getElementById(id);
        // Değişiklik olduğunda verileri yeniden yükle
        element?.addEventListener('change', () => loadDashboardData());
    });

    document.getElementById('clearDashboardFilters')?.addEventListener('click', () => {
        document.getElementById('dashboardFilterForm').reset();
        loadDashboardData();
    });
}

/**
 * Seçilen filtrelere göre tüm dashboard verilerini çeker ve UI'ı günceller.
 * Hatalara karşı daha dayanıklı hale getirilmiş ve istemci taraflı filtreleme eklenmiştir.
 */
async function loadDashboardData() {
    console.log("Dashboard verileri filtrelere göre yeniden yükleniyor...");

    // Yükleme göstergesini aktif et (opsiyonel ama iyi bir kullanıcı deneyimi için önerilir)
    // Örnek: showLoadingIndicator();

    const filters = {
        fairId: document.getElementById('dashboardFairFilter')?.value,
        startDate: document.getElementById('dashboardStartDate')?.value || null,
        endDate: document.getElementById('dashboardEndDate')?.value || null,
    };

    // Gerekli tüm verileri API'den paralel olarak çek.
    const [paymentsRes, fairExpensesRes, officeExpensesRes] = await Promise.allSettled([
        window.fetchAllPayments({ pageSize: 10000 }), // Tüm ödemeleri çek
        window.fetchAllFairExpensesForDashboard(),   // Tüm fuar giderlerini çek
        window.fetchAllOfficeExpenses(true, { pageSize: 10000 }) // Tüm ofis giderlerini çek
    ]);

    // --- VERİLERİ İSTEMCİ TARAFINDA FİLTRELEME ---

    const allPayments = paymentsRes.status === 'fulfilled' ? (paymentsRes.value?.items || []) : [];
    const allFairExpenses = fairExpensesRes.status === 'fulfilled' ? fairExpensesRes.value : [];
    const allOfficeExpenses = officeExpensesRes.status === 'fulfilled' ? (officeExpensesRes.value?.items || []) : [];

    // Tarih filtresi uygula
    const paymentsInDateRange = allPayments.filter(p => isWithinDateRange(p.paymentDate, filters.startDate, filters.endDate));
    const fairExpensesInDateRange = allFairExpenses.filter(e => isWithinDateRange(e.date, filters.startDate, filters.endDate));
    const officeExpensesInDateRange = allOfficeExpenses.filter(e => isWithinDateRange(e.date, filters.startDate, filters.endDate));

    // Fuar filtresi uygula
    const fairId = (filters.fairId === 'all' || !filters.fairId) ? null : filters.fairId;

    const filteredParticipants = fairId
        ? window.AppState.participants.filter(p => p.fairId === fairId)
        : window.AppState.participants;

    const fairStandIds = fairId
        ? window.AppState.stands.filter(s => s.fairId === fairId).map(s => s.id)
        : null;

    const filteredPayments = fairId
        ? paymentsInDateRange.filter(p => fairStandIds && fairStandIds.includes(p.standId))
        : paymentsInDateRange;

    const filteredFairExpenses = fairId
        ? fairExpensesInDateRange.filter(e => e.fairId === fairId)
        : fairExpensesInDateRange;

    // Ofis giderleri fuardan bağımsızdır, sadece tarih filtresi uygulanır.
    const combinedExpenses = [...filteredFairExpenses, ...officeExpensesInDateRange];

    const dashboardData = {
        payments: filteredPayments,
        combinedExpenses: combinedExpenses,
        participants: filteredParticipants,
        fairs: window.AppState.fairs
    };

    updateFinalDashboardCards(dashboardData);
    renderFinalCharts(dashboardData);
    renderDynamicLists(dashboardData);

    console.log("Dashboard veri yükleme işlemi tamamlandı.");
    // Yükleme göstergesini gizle
    // Örnek: hideLoadingIndicator();
}

/**
 * Bir tarihin verilen aralıkta olup olmadığını kontrol eden yardımcı fonksiyon.
 */
function isWithinDateRange(itemDate, startDate, endDate) {
    if (!startDate && !endDate) return true;
    if (!itemDate) return false;
    const date = new Date(itemDate);
    date.setHours(0, 0, 0, 0); // Saat farklarından etkilenmemek için
    const start = startDate ? new Date(startDate) : null;
    const end = endDate ? new Date(endDate) : null;

    if (start) start.setHours(0, 0, 0, 0);
    if (end) end.setHours(0, 0, 0, 0);

    if (start && date < start) return false;
    if (end && date > end) return false;
    return true;
}


/**
 * KPI kartlarını günceller.
 */
function updateFinalDashboardCards(data) {
    const totalRevenue = (data.payments || []).reduce((sum, p) => sum + (p.amount || 0), 0);
    const totalExpenses = (data.combinedExpenses || []).reduce((sum, exp) => sum + (exp.amount || 0), 0);
    const netProfit = totalRevenue - totalExpenses;
    const participantCount = (data.participants || []).length;
    const revenuePerParticipant = participantCount > 0 ? totalRevenue / participantCount : 0;

    document.getElementById('totalRevenue').textContent = window.formatCurrency(totalRevenue);
    document.getElementById('totalExpenses').textContent = window.formatCurrency(totalExpenses);
    document.getElementById('netProfit').textContent = window.formatCurrency(netProfit);
    document.getElementById('revenuePerParticipant').textContent = window.formatCurrency(revenuePerParticipant);
}

/**
 * Sayfadaki tüm grafikleri çizer.
 */
function renderFinalCharts(data) {
    renderChart('monthlyTrendChart', 'line', getMonthlyTrendData(data));
    renderChart('fairProfitabilityChart', 'bar', getFairProfitabilityData(data));
    renderChart('paymentMethodsChart', 'doughnut', getPaymentMethodsData(data), { plugins: { legend: { position: 'top' } } });
}

/**
 * Dinamik listeleri (En çok ödeyenler, Son işlemler) oluşturur.
 */
function renderDynamicLists(data) {
    renderTopParticipantsList(data);
    renderRecentTransactionsTable(data);
}


// --- VERİ HAZIRLAMA FONKSİYONLARI ---

function getMonthlyTrendData(data) {
    const monthlyData = {};
    const locale = 'tr-TR';

    (data.payments || []).forEach(p => {
        const month = new Date(p.paymentDate).toLocaleString(locale, { month: 'short', year: 'numeric' });
        monthlyData[month] = monthlyData[month] || { revenue: 0, expense: 0 };
        monthlyData[month].revenue += p.amount;
    });

    (data.combinedExpenses || []).forEach(e => {
        const date = e.date;
        const month = new Date(date).toLocaleString(locale, { month: 'short', year: 'numeric' });
        monthlyData[month] = monthlyData[month] || { revenue: 0, expense: 0 };
        monthlyData[month].expense += (e.amount || 0);
    });

    const labels = Object.keys(monthlyData).sort((a, b) => new Date(a.replace(/(\w+)\s(\d+)/, '$1 1, $2')) - new Date(b.replace(/(\w+)\s(\d+)/, '$1 1, $2')));

    return {
        labels,
        datasets: [
            { label: 'Gelir', data: labels.map(l => monthlyData[l].revenue), borderColor: '#1cc88a', backgroundColor: '#1cc88a20', tension: 0.3, fill: true },
            { label: 'Gider', data: labels.map(l => monthlyData[l].expense), borderColor: '#e74a3b', backgroundColor: '#e74a3b20', tension: 0.3, fill: true }
        ]
    };
}

function getFairProfitabilityData(data) {
    const fairTotals = {};

    (data.payments || []).forEach(p => {
        const stand = window.getStandById(p.standId);
        if (!stand) return;
        const fairName = window.getFairNameById(stand.fairId) || 'Diğer';
        fairTotals[fairName] = fairTotals[fairName] || { revenue: 0, expense: 0 };
        fairTotals[fairName].revenue += p.amount || 0;
    });

    (data.combinedExpenses || []).filter(e => e.fairId).forEach(e => { // Sadece fuar giderlerini al
        const fairName = window.getFairNameById(e.fairId) || 'Diğer';
        fairTotals[fairName] = fairTotals[fairName] || { revenue: 0, expense: 0 };
        fairTotals[fairName].expense += e.amount || 0;
    });

    const labels = Object.keys(fairTotals);
    const profitData = labels.map(label => fairTotals[label].revenue - fairTotals[label].expense);

    return {
        labels,
        datasets: [{
            label: 'Net Kâr',
            data: profitData,
            backgroundColor: profitData.map(p => p >= 0 ? '#4e73df' : '#e74a3b'),
            borderRadius: 4
        }]
    };
}

function getPaymentMethodsData(data) {
    const methodTotals = (data.payments || []).reduce((acc, p) => {
        const method = p.paymentMethod || 'Belirtilmemiş';
        acc[method] = (acc[method] || 0) + 1;
        return acc;
    }, {});

    return {
        labels: Object.keys(methodTotals),
        datasets: [{
            data: Object.values(methodTotals),
            backgroundColor: ['#4e73df', '#1cc88a', '#36b9cc', '#f6c23e', '#e74a3b'],
            borderColor: '#ffffff',
            borderWidth: 2,
        }]
    };
}

// --- LİSTE OLUŞTURMA FONKSİYONLARI ---

function renderTopParticipantsList(data) {
    const container = document.getElementById('topParticipantsList');
    if (!container) return;

    const participantPayments = (data.payments || []).reduce((acc, p) => {
        const stand = window.getStandById(p.standId);
        if (!stand) return acc;
        const participantName = window.getParticipantNameById(stand.participantId);
        if (participantName !== '-') {
            acc[participantName] = (acc[participantName] || 0) + p.amount;
        }
        return acc;
    }, {});

    const sortedParticipants = Object.entries(participantPayments)
        .sort(([, a], [, b]) => b - a)
        .slice(0, 5);

    container.innerHTML = '';
    if (sortedParticipants.length === 0) {
        container.innerHTML = '<li class="list-group-item text-center p-3">Filtreye uygun veri bulunamadı.</li>';
        return;
    }

    sortedParticipants.forEach(([name, total], index) => {
        const icon = ['bi-trophy-fill text-warning', 'bi-award-fill text-secondary', 'bi-star-fill text-primary'][index] || 'bi-person-fill';
        container.innerHTML += `
            <li class="list-group-item d-flex justify-content-between align-items-center">
                <div>
                    <i class="bi ${icon} me-2"></i>
                    ${window.escapeHtml(name)}
                </div>
                <span class="badge bg-primary rounded-pill">${window.formatCurrency(total)}</span>
            </li>
        `;
    });
}


function renderRecentTransactionsTable(data) {
    const tableBody = document.getElementById('recentTransactionsTable')?.querySelector('tbody');
    if (!tableBody) return;

    const payments = (data.payments || []).map(p => {
        const stand = window.getStandById(p.standId);
        const participantName = stand ? window.getParticipantNameById(stand.participantId) : 'Bilinmeyen';
        return { date: p.paymentDate, desc: `Ödeme: ${participantName}`, amount: p.amount, type: 'gelir' };
    });

    const expenses = (data.combinedExpenses || []).map(e => {
        const desc = e.fairId
            ? `${window.getFairNameById(e.fairId)} Fuar Gideri`
            : e.description || 'Ofis Gideri';
        return { date: e.date, desc: `Gider: ${desc}`, amount: e.amount, type: 'gider' };
    });

    const allTransactions = [...payments, ...expenses]
        .sort((a, b) => new Date(b.date) - new Date(a.date))
        .slice(0, 7);

    tableBody.innerHTML = '';
    if (allTransactions.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="3" class="text-center p-3">Filtreye uygun işlem bulunmuyor.</td></tr>';
        return;
    }

    allTransactions.forEach(tx => {
        const isIncome = tx.type === 'gelir';
        tableBody.innerHTML += `
            <tr>
                <td class="small text-muted">${window.formatDate(tx.date)}</td>
                <td>${window.escapeHtml(tx.desc)}</td>
                <td class="text-end font-weight-bold text-${isIncome ? 'success' : 'danger'}">
                    ${isIncome ? '+' : '-'} ${window.formatCurrency(tx.amount)}
                </td>
            </tr>
        `;
    });
}

// --- YARDIMCI FONKSİYONLAR ---

function renderChart(canvasId, type, data, options = {}) {
    const ctx = document.getElementById(canvasId)?.getContext('2d');
    if (!ctx) { console.error(`Canvas with id '${canvasId}' not found.`); return; }

    // Chart.js'de aynı canvas üzerine yeni bir grafik çizilmeden önce eskisinin yok edilmesi gerekir.
    const existingChart = Chart.getChart(ctx);
    if (existingChart) {
        existingChart.destroy();
    }

    const defaultOptions = {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
            legend: {
                labels: {
                    font: {
                        family: "'Nunito', sans-serif"
                    }
                }
            }
        },
        interaction: {
            intersect: false,
            mode: 'index',
        },
    };
    new Chart(ctx, { type: type, data: data, options: { ...defaultOptions, ...options } });
}

// Dashboard için özel gider çekme fonksiyonu
async function fetchAllFairExpensesForDashboard() {
    // Bu fonksiyon zaten tüm fuarların giderlerini çekiyor, bu yüzden olduğu gibi kalabilir.
    if (!window.AppState.fairs || window.AppState.fairs.length === 0) return [];

    const expensePromises = window.AppState.fairs.map(fair =>
        window.fetchWithToken(`/FairExpenses/fair/${fair.id}`)
            .then(res => res.json())
            .then(data => (window.processApiResponse(data.items || data) || []).map(e => ({ ...e, type: 'Fuar' }))) // Gider tipini ekle
            .catch(() => [])
    );
    const results = await Promise.all(expensePromises);
    return results.flat();
}

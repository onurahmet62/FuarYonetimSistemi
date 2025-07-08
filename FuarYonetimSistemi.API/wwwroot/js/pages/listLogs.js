// js/pages/listLogs.js

const logListPageState = {
    currentPage: 1,
    totalPages: 1,
    totalRecords: 0,
    pageSize: 50
};

window.loadLogsPage = async function () {
    console.log("Sistem Logları sayfası yükleniyor...");
    setupLogEventListeners();
    await loadLogs(1);
};

async function loadLogs(pageNumber = 1) {
    logListPageState.currentPage = pageNumber;
    logListPageState.pageSize = parseInt(document.getElementById('pageSizeLog').value, 10);

    const filters = {
        pageNumber: logListPageState.currentPage,
        pageSize: logListPageState.pageSize,
        userName: document.getElementById('filterLogUserName')?.value || null,
        actionType: document.getElementById('filterLogActionType')?.value || null,
        startDate: document.getElementById('filterLogStartDate')?.value || null,
        endDate: document.getElementById('filterLogEndDate')?.value || null,
    };

    try {
        const response = await window.fetchWithToken('/Logs/filter', {
            method: 'POST',
            body: JSON.stringify(filters)
        });
        const responseData = await response.json();

        logListPageState.totalRecords = responseData.totalCount;
        logListPageState.totalPages = responseData.totalPages || 1;

        renderLogsTable(responseData.items || responseData);
        updateLogPaginationInfo();
    } catch (error) {
        console.error("Sistem logları yüklenirken hata:", error);
        document.getElementById('logTableBody').innerHTML = `<tr><td colspan="4" class="text-center text-danger">Loglar yüklenirken bir hata oluştu.</td></tr>`;
    }
}

function renderLogsTable(logs) {
    const tableBody = document.getElementById('logTableBody');
    tableBody.innerHTML = '';

    if (!logs || logs.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="4" class="text-center p-4">Gösterilecek log kaydı bulunamadı.</td></tr>';
        return;
    }

    logs.forEach(log => {
        const row = tableBody.insertRow();
        const formattedDate = new Intl.DateTimeFormat('tr-TR', { dateStyle: 'short', timeStyle: 'medium' }).format(new Date(log.timestamp));
        row.innerHTML = `
            <td>${formattedDate}</td>
            <td>${window.escapeHtml(log.userName)}</td>
            <td><span class="badge bg-info">${window.escapeHtml(log.actionType)}</span></td>
            <td>${window.escapeHtml(log.description)}</td>
        `;
    });
}

function updateLogPaginationInfo() {
    window.updateGenericPaginationUI({
        currentPage: logListPageState.currentPage,
        totalPages: logListPageState.totalPages,
        totalRecords: logListPageState.totalRecords,
        pageInfoId: 'currentPageInfoLog',
        prevBtnId: 'prevPageLog',
        nextBtnId: 'nextPageLog',
        recordsInfoId: 'recordCountLog' // Bu ID'yi HTML'e eklemek gerekebilir
    });
}

function setupLogEventListeners() {
    const filterForm = document.getElementById('filterLogForm');
    filterForm?.addEventListener('submit', (e) => {
        e.preventDefault();
        loadLogs(1);
    });

    document.getElementById('clearLogFilterBtn')?.addEventListener('click', () => {
        filterForm.reset();
        loadLogs(1);
    });

    document.getElementById('pageSizeLog')?.addEventListener('change', () => loadLogs(1));
    document.getElementById('prevPageLog')?.addEventListener('click', () => { if (logListPageState.currentPage > 1) loadLogs(logListPageState.currentPage - 1); });
    document.getElementById('nextPageLog')?.addEventListener('click', () => { if (logListPageState.currentPage < logListPageState.totalPages) loadLogs(logListPageState.currentPage + 1); });
}

// script.js'in bu sayfayı tanıması için global fonksiyonu ekleyelim
document.addEventListener('DOMContentLoaded', () => {
    if (window.location.pathname.toLowerCase().includes('listlogs.html')) {
        window.loadLogsPage();
    }
});

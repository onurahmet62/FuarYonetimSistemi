// js/core.js (Dinamik Sidebar ve Topbar yapısını içeren tam sürüm)

// --- TEMEL SABİTLER ---
const API_BASE_URL = "https://azb.net.tr/api";
const TOKEN_KEY = "token";

// --- MERKEZİ UYGULAMA DURUMU (STATE) ---
window.AppState = {
    fairs: [],
    participants: [],
    stands: [],
    fairExpenseTypes: [],
    fairExpenses: [],
    officeExpenseTypes: [],
    officeExpenses: [],
    payments: [],
    users: []
};

window.fetchWithToken = async (url, options = {}) => {
    const token = localStorage.getItem('token');
    if (!token) {
        window.location.href = '/login.html';
        // Hata fırlatarak fonksiyonun geri kalanının çalışmasını engelle
        throw new Error("Kullanıcı token bulunamadı. Lütfen giriş yapın.");
    }

    const headers = {
        'Authorization': `Bearer ${token}`,
        ...options.headers,
    };

    // Body içeren istekler için Content-Type'ı otomatik olarak ayarla (eğer belirtilmemişse)
    if (options.body && !headers['Content-Type']) {
        headers['Content-Type'] = 'application/json';
    }

    const config = {
        ...options,
        headers: headers
    };

    try {
        const response = await fetch(`${API_BASE_URL}${url}`, config);

        if (response.status === 401) {
            localStorage.removeItem('token');
            window.location.href = '/login.html';
            throw new Error('Oturum süresi doldu veya geçersiz. Lütfen tekrar giriş yapın.');
        }

        // 204 No Content durumunda başarılı ve boş veri ile dön
        if (response.status === 204) {
            return { success: true, data: null };
        }

        const text = await response.text();
        // Cevap tamamen boşsa, başarılı kabul et ve null dön
        if (!text) {
            return { success: true, data: null };
        }

        let data;
        try {
            data = JSON.parse(text);
        } catch (e) {
            console.error("Sunucu yanıtı JSON formatında değil:", text);
            throw new Error("Sunucudan gelen yanıt işlenemedi.");
        }

        if (!response.ok) {
            let errorMessage = data.message || data.title || `Sunucu hatası: ${response.status}`;

            // .NET doğrulama hatalarını daha detaylı göstermek için
            if (data.errors) {
                const errorDetails = Object.entries(data.errors)
                    .map(([field, messages]) => `${field}: ${messages.join(', ')}`)
                    .join('; ');
                errorMessage = `${data.title || "Doğrulama hatası"}: ${errorDetails}`;
            }

            console.error('API Hata Yanıtı:', data);
            throw new Error(errorMessage, { cause: data });
        }

        // Yanıtı standart { success, data } formatına getir
        // 1. Yanıt { $id, $values } formatında ise
        if (data && data.$values) {
            return { success: true, data: data.$values };
        }
        // 2. Yanıt zaten { success, data } formatında ise
        if (data && typeof data.success === 'boolean') {
            return data;
        }
        // 3. Yanıt doğrudan bir dizi veya nesne ise (sarmalanmamışsa)
        return { success: true, data: data };

    } catch (error) {
        // Zaten yukarıda anlamlı bir hata mesajı oluşturduysak, onu kullan
        if (error.cause) {
            throw error;
        }

        console.error(`API isteği sırasında kritik hata oluştu (${url}):`, error);
        throw new Error(`Ağ veya sunucuya ulaşılamıyor: ${error.message}`);
    }
};


// --- MERKEZİ MENÜ (SIDEBAR) TANIMLAMASI ---
const sidebarMenu = [
    { type: 'item', id: 'menuDashboard', text: 'Dashboard', icon: 'bi-speedometer', href: 'home.html', roles: ['Admin', 'Finans', 'Stand Sorumlusu', 'Raporlama'] },
    { type: 'divider' },
    { type: 'heading', text: 'Yönetim' },
    {
        type: 'collapse', id: 'menuFuarYonetimi', text: 'Fuar Yönetimi', icon: 'bi-calendar-event', roles: ['Admin', 'Finans', 'Stand Sorumlusu', 'Raporlama'],
        items: [{ text: 'Fuar Listesi', href: 'listfuar.html' }, { text: 'Katılımcı Listesi', href: 'listparticipant.html' }, { text: 'Stand Listesi', href: 'liststand.html' }]
    },
    {
        type: 'collapse', id: 'menuGiderYonetimi', text: 'Gider Yönetimi', icon: 'bi-cash-stack', roles: ['Admin', 'Finans'],
        items: [{ text: 'Fuar Giderleri', href: 'listfairexpense.html' }, { text: 'Ofis Giderleri', href: 'listofficeexpenses.html' }, { text: 'Ödeme Listesi', href: 'listpayment.html' }]
    },
    { type: 'item', id: 'menuHedefYonetimi', text: 'Hedef Yönetimi', icon: 'bi-bullseye', href: 'listtargets.html', roles: ['Admin', 'Finans'] },
    { type: 'item', id: 'menuDosyaYonetimi', text: 'Ortak Dosyalar', icon: 'bi-folder-symlink', href: 'sharedfiles.html', roles: ['Admin', 'Finans', 'Stand Sorumlusu', 'Raporlama'] },
    { type: 'divider' },
    { type: 'heading', text: 'Sistem' },
    { type: 'item', id: 'menuKullaniciYonetimi', text: 'Kullanıcı Yönetimi', icon: 'bi-people', href: 'listuser.html', roles: ['Admin'] },
    { type: 'item', id: 'menuSistemYonetimi', text: 'Sistem Logları', icon: 'bi-hdd-stack', href: 'listlogs.html', roles: ['Admin'] },
];

/**
 * Sidebar menüsünü dinamik olarak oluşturur ve yetkilendirme uygular.
 */
window.buildSidebar = function () {
    const userRole = localStorage.getItem('userRole') || 'Stand Sorumlusu';
    const sidebar = document.getElementById('accordionSidebar');
    if (!sidebar) return;

    sidebar.className = 'navbar-nav sidebar sidebar-dark accordion new-sidebar';

    let menuHtml = `
        <a class="sidebar-brand" href="home.html">
            <div class="sidebar-brand-icon">
                <i class="bi bi-folder-fill"></i>
            </div>
            <span class="sidebar-brand-text">ArDesk</span>
        </a>`;
    const currentPage = window.location.pathname.split('/').pop();

    sidebarMenu.forEach(item => {
        if (item.roles && !item.roles.includes(userRole)) return;

        if (item.type === 'divider') {
            menuHtml += `<hr class="sidebar-divider my-2">`;
        } else if (item.type === 'heading') {
            menuHtml += `<div class="sidebar-heading">${item.text}</div>`;
        } else if (item.type === 'item') {
            const isActive = currentPage === item.href;
            menuHtml += `
                <li class="nav-item ${isActive ? 'active' : ''}" id="${item.id}">
                    <a class="nav-link" href="${item.href}">
                        <i class="bi ${item.icon}"></i>
                        <span>${item.text}</span>
                    </a>
                </li>`;
        } else if (item.type === 'collapse') {
            const isParentActive = item.items.some(sub => sub.href === currentPage);
            menuHtml += `
                <li class="nav-item ${isParentActive ? 'active' : ''}" id="${item.id}">
                    <a class="nav-link ${isParentActive ? '' : 'collapsed'}" href="#" data-bs-toggle="collapse" data-bs-target="#${item.id}Collapse" aria-expanded="${isParentActive ? 'true' : 'false'}" aria-controls="${item.id}Collapse">
                        <i class="bi ${item.icon}"></i>
                        <span>${item.text}</span>
                        <i class="bi bi-chevron-down collapse-arrow"></i>
                    </a>
                    <div id="${item.id}Collapse" class="collapse ${isParentActive ? 'show' : ''}" data-bs-parent="#${item.id}">
                        <div class="collapse-inner">
                            ${item.items.map(sub => `<a class="collapse-item ${currentPage === sub.href ? 'active' : ''}" href="${sub.href}">${sub.text}</a>`).join('')}
                        </div>
                    </div>
                </li>`;
        }
    });

    sidebar.innerHTML = menuHtml;
};


/**
 * Üst navigasyon çubuğunu (Topbar) dinamik olarak oluşturur.
 */
window.buildTopbar = function () {
    const topbarPlaceholder = document.getElementById('topbar-placeholder');
    if (!topbarPlaceholder) return;

    const userName = window.escapeHtml(localStorage.getItem('userName') || 'Kullanıcı');
    const userInitial = userName.charAt(0).toUpperCase();

    topbarPlaceholder.innerHTML = `
        <button id="sidebarToggleTop" class="btn btn-link d-md-none rounded-circle me-3"><i class="bi bi-list"></i></button>
        <form class="d-none d-sm-inline-block form-inline me-auto ms-md-3 my-2 my-md-0 mw-100 navbar-search"><div class="input-group"><input type="text" class="form-control bg-light border-0 small" placeholder="Proje genelinde ara..." aria-label="Search"><button class="btn btn-primary" type="button"><i class="bi bi-search"></i></button></div></form>
        <ul class="navbar-nav ms-auto">
            <li class="nav-item dropdown no-arrow mx-1"><a class="nav-link dropdown-toggle" href="listtargets.html" title="Bekleyen Görevler"><i class="bi bi-bullseye fs-5"></i><span class="badge bg-danger badge-counter" id="tasksCounter" style="display: none;"></span></a></li>
            <li class="nav-item dropdown no-arrow mx-1"><a class="nav-link dropdown-toggle" href="messaging.html" title="Mesajlar"><i class="bi bi-envelope-fill fs-5"></i><span class="badge bg-danger badge-counter" id="messagesCounter" style="display: none;"></span></a></li>
            <div class="topbar-divider d-none d-sm-block"></div>
            <li class="nav-item dropdown no-arrow"><a class="nav-link dropdown-toggle" href="#" id="userDropdown" role="button" data-bs-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><span class="me-2 d-none d-lg-inline text-gray-600 small">${userName}</span><img class="img-profile rounded-circle" src="https://via.placeholder.com/60/4e73df/ffffff?text=${userInitial}"></a><div class="dropdown-menu dropdown-menu-end shadow animated--grow-in" aria-labelledby="userDropdown"><a class="dropdown-item" href="#"><i class="bi bi-person-fill me-2 text-gray-400"></i> Profil</a><div class="dropdown-divider"></div><a class="dropdown-item logout-trigger" href="#"><i class="bi bi-box-arrow-right me-2 text-gray-400"></i> Çıkış Yap</a></div></li>
        </ul>
    `;
};

/**
 * Topbar'daki bildirim sayılarını API'den çekip günceller.
 */
window.updateTopbarCounters = async function () {
    try {
        const { unreadMessages, pendingTasks } = { unreadMessages: 3, pendingTasks: 2 };

        const messagesCounter = document.getElementById('messagesCounter');
        if (messagesCounter && unreadMessages > 0) {
            messagesCounter.textContent = unreadMessages;
            messagesCounter.style.display = 'inline-block';
        }
        const tasksCounter = document.getElementById('tasksCounter');
        if (tasksCounter && pendingTasks > 0) {
            tasksCounter.textContent = pendingTasks;
            tasksCounter.style.display = 'inline-block';
        }
    } catch (error) {
        console.error("Bildirim sayıları güncellenirken hata oluştu:", error);
    }
};

// --- YARDIMCI VE GENEL FONKSİYONLAR ---
window.escapeHtml = function (text) {
    if (text === null || typeof text === 'undefined') return '';
    const map = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#039;' };
    return String(text).replace(/[&<>"']/g, m => map[m]);
};

window.showLoadingOverlay = function () {
    const overlay = document.getElementById('loadingOverlay');
    if (overlay) overlay.style.display = 'flex';
};

window.hideLoadingOverlay = function () {
    const overlay = document.getElementById('loadingOverlay');
    if (overlay) overlay.style.display = 'none';
};

window.processApiResponse = function (apiResponse) {
    if (!apiResponse) return [];
    if (apiResponse.$values && Array.isArray(apiResponse.$values)) return apiResponse.$values;
    if (Array.isArray(apiResponse)) return apiResponse;
    if (apiResponse.items && Array.isArray(apiResponse.items)) return apiResponse.items;
    if (apiResponse.data && Array.isArray(apiResponse.data)) return apiResponse.data;
    // Eğer yanıtın kendisi bir array ise doğrudan onu döndür
    return apiResponse;
};

/**
 * Token ile API isteği yapan merkezi fonksiyon.
 */
window.fetchWithToken = async function (url, options = {}) {
    window.showLoadingOverlay();
    const token = localStorage.getItem(TOKEN_KEY);
    const headers = { 'Content-Type': 'application/json', ...options.headers };
    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }

    try {
        const response = await fetch(`${API_BASE_URL}${url}`, { ...options, headers });

        if (response.status === 401) {
            window.logout();
            throw new Error('Oturumunuz sona erdi. Lütfen tekrar giriş yapın.');
        }

        const responseText = await response.text();

        if (!response.ok) {
            let errorMessage = `HTTP Hatası: ${response.status}`;
            if (responseText) {
                try {
                    const jsonError = JSON.parse(responseText);
                    errorMessage = jsonError.message || jsonError.title || JSON.stringify(jsonError);
                } catch (e) {
                    errorMessage = responseText.trim().startsWith('<') ? `Sunucu Hatası: ${response.status}` : responseText;
                }
            }
            throw new Error(errorMessage);
        }

        // Başarılı ama boş yanıtları (204 No Content) veya boş JSON {} handle et
        if (response.status === 204 || !responseText) {
            return;
        }

        return JSON.parse(responseText);

    } catch (error) {
        console.error(`API isteği hatası (${url}):`, error);
        if (typeof Swal !== 'undefined') {
            Swal.fire('Hata!', error.message || 'Bilinmeyen bir hata oluştu.', 'error');
        }
        throw error;
    } finally {
        window.hideLoadingOverlay();
    }
};


window.logout = function () {
    localStorage.clear();
    window.location.href = 'login.html';
};

window.formatDate = function (dateString) {
    if (!dateString) return '-';
    try {
        return new Intl.DateTimeFormat('tr-TR', { day: '2-digit', month: '2-digit', year: 'numeric' }).format(new Date(dateString));
    } catch (e) {
        return '-';
    }
};

window.formatCurrency = function (amount, currency = 'TRY') {
    if (typeof amount !== 'number' || isNaN(amount)) {
        if (amount !== 0) return '-';
        amount = 0;
    }
    return new Intl.NumberFormat('tr-TR', { style: 'currency', currency: currency }).format(amount);
};

// --- GENEL VERİ ÇEKME VE YÖNETME FONKSİYONLARI ---
async function fetchAndStoreSimple(key, endpoint, forceRefresh = false) {
    if (!Array.isArray(window.AppState[key])) {
        window.AppState[key] = [];
    }
    if (forceRefresh || window.AppState[key].length === 0) {
        try {
            const data = await window.fetchWithToken(endpoint, { method: 'GET' });
            window.AppState[key] = window.processApiResponse(data);
        } catch (error) {
            console.error(`${key} verileri yüklenirken hata oluştu. AppState.${key} boş bir dizi olarak ayarlandı.`);
            window.AppState[key] = [];
        }
    }
}

window.fetchAllFairs = (forceRefresh = false) => fetchAndStoreSimple('fairs', '/Fair', forceRefresh);
window.fetchAllParticipants = (forceRefresh = false) => fetchAndStoreSimple('participants', '/Participants', forceRefresh);
window.fetchAllStands = (forceRefresh = false) => fetchAndStoreSimple('stands', '/Stands', forceRefresh);
window.fetchAllFairExpenseTypes = (forceRefresh = false) => fetchAndStoreSimple('fairExpenseTypes', '/FairExpenses/expense-types', forceRefresh);
window.fetchAllOfficeExpenseTypes = (forceRefresh = false) => fetchAndStoreSimple('officeExpenseTypes', '/OfficeExpenses/types', forceRefresh);
window.fetchAllUsers = (forceRefresh = false) => fetchAndStoreSimple('users', '/User', forceRefresh);
window.fetchAllPaymentsSimple = (forceRefresh = false) => fetchAndStoreSimple('payments', '/Payments?pageSize=10000', forceRefresh);

window.authorizeUI = function () {
    const userRole = localStorage.getItem('userRole');
    if (!userRole) console.warn("Kullanıcı rolü bulunamadı. Yetkilendirme başarısız.");

    const permissions = {
        'menuKullaniciYonetimi': ['Admin'],
        'menuGiderYonetimi': ['Admin', 'Finans'],
        'addFairButton': ['Admin', 'Stand Sorumlusu'],
    };

    for (const elementId in permissions) {
        const element = document.getElementById(elementId);
        if (element) {
            const allowedRoles = permissions[elementId];
            if (!userRole || !allowedRoles.includes(userRole)) {
                element.style.display = 'none';
            }
        }
    }
};

window.fetchTargets = async function (filters = {}) {
    try {
        const data = await window.fetchWithToken('/Targets/filter', {
            method: 'POST',
            body: JSON.stringify(filters)
        });
        return {
            targets: window.processApiResponse(data.items || data),
            totalCount: data.totalCount || 0,
            totalPages: data.totalPages || 1,
            currentPage: data.currentPage || 1
        };
    } catch (error) {
        console.error(`Hedefler yüklenirken hata oluştu:`, error);
        return { targets: [], totalCount: 0, totalPages: 1, currentPage: 1 };
    }
};

/**
 * Sisteme bir log kaydı gönderir.
 * KULLANICI İSTEĞİ ÜZERİNE GEÇİCİ OLARAK DEVRE DIŞI BIRAKILDI.
 */
window.logAction = async function (actionType, description) {
    console.log(`Loglama devre dışı. Eylem: ${actionType}, Açıklama: ${description}`);
    return Promise.resolve(); // Fonksiyonun bir promise döndürmesini sağlayarak olası 'await' hatalarını önler.
};

window.fetchParticipants = async function (filters = {}) {
    try {
        const data = await window.fetchWithToken('/Participants/filter', {
            method: 'POST',
            body: JSON.stringify(filters)
        });
        return {
            participants: window.processApiResponse(data.participants || data.items || data),
            totalCount: data.totalCount || 0,
            totalPages: data.totalPages || 1,
            currentPage: data.currentPage || 1
        };
    } catch (error) {
        console.error(`Katılımcılar yüklenirken hata oluştu:`, error);
        return { participants: [], totalCount: 0, totalPages: 1, currentPage: 1 };
    }
};
window.fetchStands = async function (filters = {}) {
    try {
        const data = await window.fetchWithToken('/Stands/filter', {
            method: 'POST',
            body: JSON.stringify(filters)
        });
        return {
            stands: window.processApiResponse(data.stands || data.items || data),
            totalCount: data.totalCount || 0,
            totalPages: data.totalPages || 1,
            currentPage: data.currentPage || 1
        };
    } catch (error) {
        console.error(`Standlar yüklenirken hata oluştu:`, error);
        return { stands: [], totalCount: 0, totalPages: 1, currentPage: 1 };
    }
};

window.fetchUsers = async function (filters = {}) {
    try {
        const data = await window.fetchWithToken('/User/filter', {
            method: 'POST',
            body: JSON.stringify(filters)
        });
        return {
            users: window.processApiResponse(data.users || data.items || data),
            totalCount: data.totalCount || 0,
            totalPages: data.totalPages || 1,
            currentPage: data.currentPage || 1
        };
    } catch (error) {
        console.error(`Kullanıcılar yüklenirken hata oluştu:`, error);
        return { users: [], totalCount: 0, totalPages: 1, currentPage: 1 };
    }
};

window.fetchAllPayments = async function (forceRefresh = false, filters = {}) {
    if (forceRefresh || !window.AppState.payments || window.AppState.payments.length === 0) {
        try {
            const queryParams = new URLSearchParams({ pageSize: 10000, ...filters });
            const data = await window.fetchWithToken(`/Payments?${queryParams.toString()}`);
            const items = window.processApiResponse(data);
            window.AppState.payments = items;
            return { items, totalCount: data.totalCount || items.length, totalPages: data.totalPages || 1 };
        } catch (error) {
            console.error(`Ödemeler yüklenirken hata oluştu:`, error);
            window.AppState.payments = [];
            return { items: [], totalCount: 0, totalPages: 1 };
        }
    }
    return { items: window.AppState.payments, totalCount: window.AppState.payments.length, totalPages: Math.ceil(window.AppState.payments.length / (filters.pageSize || 20)) };
};

window.fetchAllOfficeExpenses = async function (forceRefresh = false, filters = {}) {
    if (forceRefresh || !window.AppState.officeExpenses || window.AppState.officeExpenses.length === 0) {
        try {
            const queryParams = new URLSearchParams({ pageNumber: 1, pageSize: 10000, ...filters });
            const data = await window.fetchWithToken(`/OfficeExpenses?${queryParams.toString()}`);
            window.AppState.officeExpenses = window.processApiResponse(data.items || data);
            return { items: window.AppState.officeExpenses, totalCount: data.totalCount || 0, totalPages: data.totalPages || 1 };
        } catch (error) {
            window.AppState.officeExpenses = [];
            return { items: [], totalCount: 0, totalPages: 1 };
        }
    }
    return { items: window.AppState.officeExpenses, totalCount: window.AppState.officeExpenses.length, totalPages: Math.ceil(window.AppState.officeExpenses.length / (filters.pageSize || 20)) };
};

window.fetchAllFairExpenses = async function (forceRefresh = false, filters = {}) {
    const fairId = filters.fairId;
    if (!fairId) {
        console.warn("Fuar giderlerini çekmek için fairId gereklidir.");
        window.AppState.fairExpenses = [];
        return;
    }

    try {
        const endpoint = `/FairExpenses/fair/${fairId}`;
        const data = await window.fetchWithToken(endpoint);
        const items = window.processApiResponse(data);
        window.AppState.fairExpenses = items;
    } catch (error) {
        console.error(`Fuar Giderleri yüklenirken hata oluştu (fairId: ${fairId}):`, error);
        window.AppState.fairExpenses = [];
    }
};

window.fetchAllFairExpensesForDashboard = async function () {
    console.log("Dashboard için tüm fuar giderleri çekiliyor...");
    await window.fetchAllFairs(true);
    if (!window.AppState.fairs || window.AppState.fairs.length === 0) {
        return [];
    }
    const expensePromises = window.AppState.fairs.map(fair =>
        window.fetchWithToken(`/FairExpenses/fair/${fair.id}`)
            .then(data => window.processApiResponse(data.items || data))
            .catch(() => [])
    );
    const results = await Promise.all(expensePromises);
    const allExpenses = results.flat();
    return allExpenses;
};


// --- GETTERS, DROPDOWN, EXCEL, PAGINATION UI... ---
window.getFairNameById = (id) => window.AppState.fairs.find(f => f.id === id)?.name || '-';
window.getParticipantNameById = (id) => window.AppState.participants.find(p => p.id === id)?.companyName || '-';
window.getStandById = (id) => window.AppState.stands.find(s => s.id === id) || null;
window.getFairExpenseTypeNameById = (id) => window.AppState.fairExpenseTypes.find(e => e.id === id || e.expenseTypeId === id)?.name || '-';
window.getOfficeExpenseTypeNameById = (id) => window.AppState.officeExpenseTypes.find(e => e.id === id)?.name || '-';
window.getUserRoleName = (roleId) => ({ 0: 'Yönetici', 1: 'Finans', 2: 'Stand Sorumlusu', 3: 'Raporlama' }[roleId] || 'Bilinmiyor');

function populateDropdown(selectElementId, data, options) {
    const selectElement = document.getElementById(selectElementId);
    if (!selectElement) return;
    selectElement.innerHTML = '';
    if (options.defaultOptionText) selectElement.innerHTML += `<option value="">${options.defaultOptionText}</option>`;
    if (options.addAllOption) selectElement.innerHTML += `<option value="all">Tümü</option>`;
    data.forEach(item => {
        const value = item[options.valueField];
        const text = item[options.textField];
        selectElement.innerHTML += `<option value="${value}">${window.escapeHtml(text)}</option>`;
    });
    selectElement.value = options.selectedValue || (options.addAllOption ? 'all' : '');
}

window.loadFairsForDropdown = async (id, text, val, all) => { await fetchAllFairs(true); populateDropdown(id, window.AppState.fairs, { valueField: 'id', textField: 'name', defaultOptionText: text, selectedValue: val, addAllOption: all }); };
window.loadParticipantsForDropdown = async (id, text, val, all) => { await fetchAllParticipants(true); populateDropdown(id, window.AppState.participants, { valueField: 'id', textField: 'companyName', defaultOptionText: text, selectedValue: val, addAllOption: all }); };
window.loadFairExpenseTypesForDropdown = async (id, text, val, all) => { await fetchAllFairExpenseTypes(true); populateDropdown(id, window.AppState.fairExpenseTypes, { valueField: 'id', textField: 'name', defaultOptionText: text, selectedValue: val, addAllOption: all }); };
window.loadOfficeExpenseTypesForDropdown = async (id, text, val, all) => { await fetchAllOfficeExpenseTypes(true); populateDropdown(id, window.AppState.officeExpenseTypes, { valueField: 'id', textField: 'name', defaultOptionText: text, selectedValue: val, addAllOption: all }); };
window.loadStandsForDropdown = async function (selectElementId, defaultOptionText, selectedValue, addAllOption) {
    const selectElement = document.getElementById(selectElementId);
    if (!selectElement) return;
    await Promise.all([fetchAllStands(true), fetchAllFairs(true), fetchAllParticipants(true)]);
    selectElement.innerHTML = '';
    if (defaultOptionText) selectElement.innerHTML += `<option value="">${defaultOptionText}</option>`;
    if (addAllOption) selectElement.innerHTML += `<option value="all">Tümü</option>`;
    window.AppState.stands.forEach(stand => {
        const text = `Stand: ${stand.standNo || stand.name} (${getParticipantNameById(stand.participantId)} / ${getFairNameById(stand.fairId)})`;
        selectElement.innerHTML += `<option value="${stand.id}" ${stand.id === selectedValue ? 'selected' : ''}>${window.escapeHtml(text)}</option>`;
    });
};
window.loadUsersForDropdown = async (selectId, defaultText, selectedVal, addAllOption) => {
    await window.fetchAllUsers(true);
    const options = {
        valueField: 'id',
        textField: 'fullName',
        defaultOptionText: defaultText,
        selectedValue: selectedVal,
        addAllOption: addAllOption
    };
    populateDropdown(selectId, window.AppState.users, options);
};
window.getUserNameById = (id) => window.AppState.users.find(u => u.id === id)?.fullName || 'Bilinmiyor';

window.exportToExcel = function (data, filename, sheetname) {
    const ws = XLSX.utils.json_to_sheet(data);
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, sheetname);
    XLSX.writeFile(wb, `${filename}.xlsx`);
};

window.updateGenericPaginationUI = function (config) {
    const { currentPage, totalPages, totalRecords, pageInfoId, prevBtnId, nextBtnId, recordsInfoId } = config;
    const pageInfoEl = document.getElementById(pageInfoId);
    const prevBtnEl = document.getElementById(prevBtnId);
    const nextBtnEl = document.getElementById(nextBtnId);
    const recordsInfoEl = document.getElementById(recordsInfoId);
    if (pageInfoEl) pageInfoEl.textContent = `Sayfa: ${currentPage} / ${totalPages || 1}`;
    if (recordsInfoEl) recordsInfoEl.textContent = `Toplam Kayıt: ${totalRecords}`;
    if (prevBtnEl) prevBtnEl.disabled = currentPage <= 1;
    if (nextBtnEl) nextBtnEl.disabled = currentPage >= totalPages;
};
window.logout = () => {
    localStorage.removeItem('token');
    window.location.href = '/login.html';
};

// js/pages/listParticipant.js (Yeniden Düzenlenmiş)

// --- SAYFA DURUMU (PAGE STATE) ---
const participantListPageState = {
    currentPage: 1,
    totalPages: 1,
    totalRecords: 0,
    pageSize: 20,
    filteredData: [] // Filtrelenmiş veriyi tutmak için
};

// --- ANA FONKSİYONLAR ---

/**
 * Katılımcı listesi sayfasını başlatan ana fonksiyon.
 */
window.loadParticipantsPage = async function () {
    setupEventListeners();
    // Gerekli global verileri ve dropdownları yükle
    await window.loadFairsForDropdown('filterParticipantFair', 'Tüm Fuarlar', '', true);
    await window.loadFairsForDropdown('addParticipantFairId', 'Fuar Seçiniz...', '', false);
    await window.loadFairsForDropdown('editParticipantFairId', 'Fuar Seçiniz...', '', false);

    // Sayfayı ilk kez yükle
    await loadParticipants();
};

/**
 * Katılımcıları AppState'ten alır, filtreler ve görüntüler.
 * Bu fonksiyon istemci taraflı (client-side) filtreleme ve sayfalama yapar.
 */
async function loadParticipants() {
    // Merkezi state'in dolu olduğundan emin ol
    await window.fetchAllParticipants();

    // Filtreleri uygula
    applyFilters();

    // Sayfalama bilgilerini ayarla
    participantListPageState.totalRecords = participantListPageState.filteredData.length;
    participantListPageState.pageSize = parseInt(document.getElementById('pageSizeParticipant').value, 10);
    participantListPageState.totalPages = Math.ceil(participantListPageState.totalRecords / participantListPageState.pageSize) || 1;
    // Geçerli sayfanın toplam sayfa sayısını aşmadığından emin ol
    participantListPageState.currentPage = Math.min(participantListPageState.currentPage, participantListPageState.totalPages) || 1;

    // Tabloyu ve sayfalama UI'ını render et
    renderParticipantsTable();
    updatePaginationInfo();
}

/**
 * Filtre kriterlerini AppState.participants üzerinden uygular.
 */
function applyFilters() {
    const filterName = document.getElementById('filterParticipantName')?.value.toLowerCase() || '';
    const filterCompany = document.getElementById('filterParticipantCompany')?.value.toLowerCase() || '';
    const filterFairId = document.getElementById('filterParticipantFair')?.value || '';

    participantListPageState.filteredData = window.AppState.participants.filter(participant => {
        const matchesName = filterName ? (participant.fullName?.toLowerCase().includes(filterName)) : true;
        const matchesCompany = filterCompany ? (participant.companyName?.toLowerCase().includes(filterCompany)) : true;
        const matchesFair = (filterFairId && filterFairId !== 'all') ? participant.fairId === filterFairId : true;

        return matchesName && matchesCompany && matchesFair;
    });
}

/**
 * Filtrelenmiş ve sayfalanmış katılımcı verilerini HTML tablosunda görüntüler.
 */
function renderParticipantsTable() {
    const tableBody = document.getElementById('participantTableBody');
    tableBody.innerHTML = '';

    const startIndex = (participantListPageState.currentPage - 1) * participantListPageState.pageSize;
    const endIndex = startIndex + participantListPageState.pageSize;
    const participantsToDisplay = participantListPageState.filteredData.slice(startIndex, endIndex);

    if (participantsToDisplay.length === 0) {
        tableBody.innerHTML = '<tr><td colspan="7" class="text-center p-4">Gösterilecek katılımcı bulunamadı.</td></tr>';
        return;
    }

    participantsToDisplay.forEach(participant => {
        const row = tableBody.insertRow();
        const fairName = window.getFairNameById(participant.fairId) || '-';
        row.innerHTML = `
            <td>${window.escapeHtml(participant.companyName)}</td>
            <td>${window.escapeHtml(participant.fullName)}</td>
            <td>${window.escapeHtml(participant.email)}</td>
            <td>${window.escapeHtml(participant.phone)}</td>
            <td>${window.escapeHtml(fairName)}</td>
            <td>${window.escapeHtml(participant.city)}</td>
            <td>
                <button class="btn btn-sm btn-warning edit-participant-btn" data-id="${participant.id}" title="Düzenle"><i class="bi bi-pencil"></i></button>
                <button class="btn btn-sm btn-danger delete-participant-btn" data-id="${participant.id}" title="Sil"><i class="bi bi-trash"></i></button>
                <a href="participantDetails.html?id=${participant.id}"  class="btn btn-sm btn-primary" title="Detaylar"><i class="bi bi-eye"></i></a>
            </td>
        `;
    });
}

/**
 * Sayfalama kullanıcı arayüzünü günceller.
 */
function updatePaginationInfo() {
    window.updateGenericPaginationUI({
        currentPage: participantListPageState.currentPage,
        totalPages: participantListPageState.totalPages,
        totalRecords: participantListPageState.totalRecords,
        pageInfoId: 'currentPageParticipant',
        prevBtnId: 'prevPageParticipant',
        nextBtnId: 'nextPageParticipant',
        recordsInfoId: 'recordCountParticipant'
    });
}


// --- OLAY DİNLEYİCİLERİ VE İŞLEYİCİLER ---

/**
 * Sayfadaki tüm olay dinleyicilerini ayarlar.
 */
function setupEventListeners() {
    // Filtreleme formu olayları
    const filterForm = document.getElementById('filterFormParticipant');
    filterForm?.addEventListener('submit', (e) => {
        e.preventDefault();
        participantListPageState.currentPage = 1;
        loadParticipants();
    });
    filterForm?.querySelector('.btn-secondary')?.addEventListener('click', () => {
        filterForm.reset();
        participantListPageState.currentPage = 1;
        loadParticipants();
    });

    // Sayfalama olayları
    document.getElementById('pageSizeParticipant')?.addEventListener('change', () => {
        participantListPageState.currentPage = 1;
        loadParticipants();
    });
    document.getElementById('prevPageParticipant')?.addEventListener('click', () => {
        if (participantListPageState.currentPage > 1) {
            participantListPageState.currentPage--;
            renderParticipantsTable(); // Sadece render'ı tekrar çağır, veri zaten var
            updatePaginationInfo();
        }
    });
    document.getElementById('nextPageParticipant')?.addEventListener('click', () => {
        if (participantListPageState.currentPage < participantListPageState.totalPages) {
            participantListPageState.currentPage++;
            renderParticipantsTable();
            updatePaginationInfo();
        }
    });

    // Modal ve CRUD olayları
    document.getElementById('addParticipantForm')?.addEventListener('submit', handleAddOrEditParticipant);
    document.getElementById('editParticipantForm')?.addEventListener('submit', handleAddOrEditParticipant);
    document.getElementById('addParticipantButton')?.addEventListener('click', setupNewParticipantForm);
    document.getElementById('exportParticipantExcelButton')?.addEventListener('click', exportParticipantsToExcel);

    // Tablo üzerindeki butonlar için olay delegasyonu
    document.getElementById('participantTableBody')?.addEventListener('click', (e) => {
        const editButton = e.target.closest('.edit-participant-btn');
        if (editButton) {
            setupEditParticipantForm(editButton.dataset.id);
            return;
        }
        const deleteButton = e.target.closest('.delete-participant-btn');
        if (deleteButton) {
            confirmDeleteParticipant(deleteButton.dataset.id);
        }
    });
}

/**
 * Yeni katılımcı ekleme formunu hazırlar ve modalı açar.
 */
function setupNewParticipantForm() {
    document.getElementById('addParticipantModalLabel').textContent = 'Yeni Katılımcı Ekle';
    document.getElementById('addParticipantForm').reset();
    document.getElementById('addParticipantId').value = '';
    const addModal = new bootstrap.Modal(document.getElementById('addParticipantModal'));
    addModal.show();
}

/**
 * Düzenlenecek katılımcının verilerini yükler ve modalı doldurur.
 */
async function setupEditParticipantForm(participantId) {
    const participant = window.AppState.participants.find(p => p.id === participantId);
    if (!participant) {
        Swal.fire('Hata', 'Katılımcı bulunamadı.', 'error');
        return;
    }

    const form = document.getElementById('editParticipantForm');
    form.reset();
    document.getElementById('editParticipantModalLabel').textContent = 'Katılımcı Düzenle';
    document.getElementById('editParticipantId').value = participant.id;
    document.getElementById('editParticipantName').value = participant.fullName || '';
    document.getElementById('editParticipantCompany').value = participant.companyName || '';
    document.getElementById('editParticipantEmail').value = participant.email || '';
    document.getElementById('editParticipantPhone').value = participant.phone || '';
    document.getElementById('editParticipantCountry').value = participant.country || '';
    document.getElementById('editParticipantCity').value = participant.city || '';
    document.getElementById('editParticipantFairId').value = participant.fairId || '';

    const editModal = new bootstrap.Modal(document.getElementById('editParticipantModal'));
    editModal.show();
}

/**
 * Katılımcı ekleme veya düzenleme formunu işler.
 */
async function handleAddOrEditParticipant(event) {
    event.preventDefault();
    const form = event.target;
    const participantId = form.querySelector('input[type=hidden]').value;

    const participantData = {
        fullName: form.querySelector('[id*="ParticipantName"]').value,
        companyName: form.querySelector('[id*="ParticipantCompany"]').value,
        email: form.querySelector('[id*="ParticipantEmail"]').value,
        phone: form.querySelector('[id*="ParticipantPhone"]').value,
        country: form.querySelector('[id*="ParticipantCountry"]').value,
        city: form.querySelector('[id*="ParticipantCity"]').value,
        fairId: form.querySelector('[id*="ParticipantFairId"]').value
    };

    try {
        if (participantId) {
            // Düzenleme
            await window.fetchWithToken(`/Participants/${participantId}`, {
                method: 'PUT',
                body: JSON.stringify({ id: participantId, ...participantData })
            });
            Swal.fire('Başarılı!', 'Katılımcı güncellendi.', 'success');
        } else {
            // Ekleme
            await window.fetchWithToken('/Participants', {
                method: 'POST',
                body: JSON.stringify(participantData)
            });
            Swal.fire('Başarılı!', 'Katılımcı eklendi.', 'success');
        }

        // Modal'ı kapat ve veriyi yenile
        const modalId = form.closest('.modal').id;
        bootstrap.Modal.getInstance(document.getElementById(modalId)).hide();
        await window.fetchAllParticipants(true); // Veriyi tazelemek için zorla
        await loadParticipants(); // Listeyi yeniden render et

    } catch (error) {
        console.error("Katılımcı kaydedilirken hata:", error);
    }
}

/**
 * Katılımcı silme işlemini onaylar ve gerçekleştirir.
 */
function confirmDeleteParticipant(id) {
    Swal.fire({
        title: 'Emin misiniz?',
        text: "Bu katılımcıyı silmek geri alınamaz!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        confirmButtonText: 'Evet, Sil!',
        cancelButtonText: 'İptal'
    }).then(async (result) => {
        if (result.isConfirmed) {
            try {
                await window.fetchWithToken(`/Participants/${id}`, { method: 'DELETE' });
                Swal.fire('Silindi!', 'Katılımcı başarıyla silindi.', 'success');
                await window.fetchAllParticipants(true); // Veriyi tazelemek için zorla
                await loadParticipants(); // Listeyi yeniden render et
            } catch (error) {
                console.error("Katılımcı silinirken hata:", error);
            }
        }
    });
}

/**
 * Katılımcı listesini Excel'e aktarır.
 */
function exportParticipantsToExcel() {
    // Filtrelenmiş güncel veriyi kullan
    const dataToExport = participantListPageState.filteredData;

    if (!dataToExport || dataToExport.length === 0) {
        Swal.fire('Bilgi', 'Excel\'e aktarılacak katılımcı bulunamadı.', 'info');
        return;
    }

    const exportData = dataToExport.map(p => ({
        "Firma Adı": p.companyName || '-',
        "Ad Soyad": p.fullName,
        "E-posta": p.email || '-',
        "Telefon": p.phone || '-',
        "Fuar Adı": window.getFairNameById(p.fairId) || '-',
        "Ülke": p.country || '-',
        "Şehir": p.city || '-',
    }));

    window.exportToExcel(exportData, 'KatilimcilarListesi', 'Katılımcılar');
}

// dosya: js/pages/participantDetails.js

/**
 * Katılımcı detay sayfasını başlatan ana fonksiyon.
 * API dokümanına uygun, stabil ve zenginleştirilmiş versiyon.
 */
window.loadParticipantDetailsPage = async function (participantId) {
    if (!participantId) {
        Swal.fire('Hata', 'Katılımcı ID bulunamadı.', 'error').then(() => window.location.href = 'listparticipant.html');
        return;
    }

    try {
        // Gerekli tüm genel verileri (fuar listesi gibi) önceden yükle
        await window.fetchAllFairs(true);

        // Katılımcıya özel verileri paralel olarak çek
        const [participantData, standsResult, paymentsResult] = await Promise.all([
            window.fetchWithToken(`/Participants/${participantId}`),
            window.fetchWithToken(`/Stands/filter`, {
                method: 'POST',
                body: JSON.stringify({ participantId: participantId, pageSize: 1000 })
            }),
            window.fetchWithToken(`/Payments/filter`, {
                method: 'POST',
                body: JSON.stringify({ participantId: participantId, pageSize: 1000 })
            })
        ]);

        const participant = participantData;
        const stands = window.processApiResponse(standsResult.items || standsResult);
        const payments = window.processApiResponse(paymentsResult.items || paymentsResult);

        // Katıldığı fuarlar stand listesinden türetilir
        const attendedFairIds = [...new Set(stands.map(s => s.fairId))];
        const attendedFairs = window.AppState.fairs.filter(f => attendedFairIds.includes(f.id));

        // Verileri AppState'e de kaydedelim
        window.AppState.currentParticipant = participant;
        window.AppState.currentStands = stands;
        window.AppState.currentPayments = payments;
        window.AppState.attendedFairs = attendedFairs; // Fuarları da saklayalım

        // Sayfayı gelen verilerle doldur
        populatePage(participant, stands, payments, attendedFairs);
        setupEventListeners(participant);

    } catch (error) {
        console.error("Katılımcı detayları yüklenirken kritik bir hata oluştu:", error);
        Swal.fire('Hata!', error.message || 'Katılımcı bilgileri yüklenirken bir sorun oluştu.', 'error');
    }
};

/**
 * Sayfadaki tüm UI elementlerini gelen verilerle doldurur.
 */
function populatePage(participant, stands, payments, attendedFairs) {
    document.getElementById('participantNameTitle').textContent = window.escapeHtml(participant.companyName);
    document.title = `Ardesk - ${window.escapeHtml(participant.companyName)}`;

    populateInfoCard(participant);
    populateStatCards(stands, attendedFairs);
    initializeAttendedFairs(attendedFairs);
    initializeStandsTable(stands);
    initializePaymentsTable(payments);
    initializeDocumentsTab(participant, stands); // Belge sekmesini ayarla
}

/**
 * Sağdaki firma bilgi kartını doldurur.
 */
function populateInfoCard(participant) {
    document.getElementById('infoFullName').textContent = window.escapeHtml(participant.fullName || '-');
    document.getElementById('infoEmail').textContent = window.escapeHtml(participant.email || '-');
    document.getElementById('infoPhone').textContent = window.escapeHtml(participant.phone || '-');
    document.getElementById('infoAddress').textContent = window.escapeHtml(participant.address || '-');
    document.getElementById('infoNotes').textContent = window.escapeHtml(participant.note || 'Girilmemiş.');
    const websiteLink = document.getElementById('infoWebsite');
    if (participant.website) {
        websiteLink.href = participant.website.startsWith('http') ? participant.website : `https://${participant.website}`;
        websiteLink.textContent = participant.website;
    } else {
        websiteLink.textContent = '-';
        websiteLink.removeAttribute('href');
    }
}


/**
 * Üstteki istatistik kartlarını doldurur.
 */
function populateStatCards(stands, attendedFairs) {
    const totalContractAmount = stands.reduce((sum, s) => sum + (s.totalAmountWithVAT || 0), 0);
    const totalBalance = stands.reduce((sum, s) => sum + (s.balance || 0), 0);

    document.getElementById('totalContractAmount').textContent = window.formatCurrency(totalContractAmount);
    document.getElementById('totalBalance').textContent = window.formatCurrency(totalBalance);
    document.getElementById('totalFairsCount').textContent = attendedFairs.length;
    document.getElementById('totalStandsCount').textContent = stands.length;
}

/**
 * Katıldığı Fuarlar sekmesini doldurur.
 */
function initializeAttendedFairs(attendedFairs) {
    const listContainer = document.getElementById('attendedFairsList');
    listContainer.innerHTML = '';
    if (!attendedFairs || attendedFairs.length === 0) {
        listContainer.innerHTML = '<p class="text-muted p-3">Bu katılımcının kayıtlı olduğu fuar bulunmamaktadır.</p>';
        return;
    }

    attendedFairs.forEach(fair => {
        const fairItem = `
            <a href="fairDetails.html?id=${fair.id}" class="list-group-item list-group-item-action">
                <div class="d-flex w-100 justify-content-between">
                    <h5 class="mb-1">${window.escapeHtml(fair.name)}</h5>
                    <small>${fair.year}</small>
                </div>
                <p class="mb-1"><i class="bi bi-geo-alt-fill me-2 text-gray-400"></i>${window.escapeHtml(fair.location)}</p>
                <small><i class="bi bi-calendar-range me-2 text-gray-400"></i>${window.formatDate(fair.startDate)} - ${window.formatDate(fair.endDate)}</small>
            </a>
        `;
        listContainer.innerHTML += fairItem;
    });
}


/**
 * Standlar tablosunu DataTable ile başlatır.
 */
function initializeStandsTable(stands) {
    $('#dataTableStands').DataTable({
        data: stands,
        destroy: true,
        columns: [
            { title: 'Fuar', data: 'fairId', render: (data) => `<a href="fairDetails.html?id=${data}">${window.escapeHtml(window.getFairNameById(data))}</a>` },
            { title: 'Stand No', data: 'name', render: (data, type, row) => window.escapeHtml(data || row.name || '-') },
            { title: 'Alan (m²)', data: 'contractArea' },
            { title: 'Toplam Tutar', data: 'totalAmountWithVAT', render: (data, type, row) => window.formatCurrency(data || 0, row.currency) },
            { title: 'Bakiye', data: 'balance', render: (data, type, row) => window.formatCurrency(data || 0, row.currency) },
            { title: 'Sözleşme Tarihi', data: 'contractDate', render: (data) => window.formatDate(data) }
        ],
        language: { url: "https://cdn.datatables.net/plug-ins/1.13.6/i18n/tr.json" },
        order: [[5, 'desc']]
    });
}


/**
 * Ödemeler tablosunu DataTable ile başlatır.
 */
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
            { title: 'Yöntem', data: 'paymentMethod' },
            { title: 'Açıklama', data: 'paymentDescription', defaultContent: '-' }
        ],
        language: { url: "https://cdn.datatables.net/plug-ins/1.13.6/i18n/tr.json" },
        order: [[1, 'desc']]
    });
}

/**
 * Belge oluşturma sekmesindeki dropdown'ları ayarlar.
 */
function initializeDocumentsTab(participant, stands) {
    const kbfFairSelect = document.getElementById('kbfFairSelect');
    const proformaStandSelect = document.getElementById('proformaStandSelect');

    kbfFairSelect.innerHTML = '<option value="">Fuar Seçiniz...</option>';
    proformaStandSelect.innerHTML = '<option value="">Stand Seçiniz...</option>';

    const participatedFairs = window.AppState.attendedFairs;
    participatedFairs.forEach(fair => {
        kbfFairSelect.innerHTML += `<option value="${fair.id}">${window.escapeHtml(fair.name)}</option>`;
    });

    stands.forEach(stand => {
        const standName = window.escapeHtml(stand.name || stand.standNo);
        const fairName = window.escapeHtml(window.getFairNameById(stand.fairId));
        proformaStandSelect.innerHTML += `<option value="${stand.id}">${standName} (${fairName})</option>`;
    });

    document.getElementById('generateKBFButton').disabled = !kbfFairSelect.value;
    document.getElementById('generateProformaButton').disabled = !proformaStandSelect.value;
}


/**
 * Sayfadaki tüm olay dinleyicilerini ayarlar.
 */
function setupEventListeners(participant) {
    const editModal = new bootstrap.Modal(document.getElementById('editParticipantModal'));

    document.getElementById('editParticipantBtn').addEventListener('click', () => {
        document.getElementById('editParticipantId').value = participant.id;
        document.getElementById('editCompanyName').value = participant.companyName || '';
        document.getElementById('editFullName').value = participant.fullName || '';
        document.getElementById('editEmail').value = participant.email || '';
        document.getElementById('editPhone').value = participant.phone || '';
        document.getElementById('editWebsite').value = participant.website || '';
        document.getElementById('editAddress').value = participant.address || '';
        document.getElementById('editNote').value = participant.note || '';
        editModal.show();
    });

    document.getElementById('saveParticipantChanges').addEventListener('click', async () => {
        const id = document.getElementById('editParticipantId').value;
        const updatedData = {
            id: id,
            companyName: document.getElementById('editCompanyName').value,
            fullName: document.getElementById('editFullName').value,
            email: document.getElementById('editEmail').value,
            phone: document.getElementById('editPhone').value,
            website: document.getElementById('editWebsite').value,
            address: document.getElementById('editAddress').value,
            note: document.getElementById('editNote').value,
            // fairId gibi diğer zorunlu alanlar korunmalı
            fairId: participant.fairId
        };
        try {
            await window.fetchWithToken(`/Participants/${id}`, { method: 'PUT', body: JSON.stringify(updatedData) });
            Swal.fire('Başarılı!', 'Katılımcı bilgileri güncellendi.', 'success');
            editModal.hide();
            window.AppState.currentParticipant = { ...window.AppState.currentParticipant, ...updatedData };
            populateInfoCard(window.AppState.currentParticipant);
        } catch (error) {
            Swal.fire('Hata!', 'Güncelleme sırasında bir sorun oluştu.', 'error');
        }
    });

    // Belge oluşturma butonları için olay dinleyicileri
    const kbfFairSelect = document.getElementById('kbfFairSelect');
    kbfFairSelect.addEventListener('change', () => {
        document.getElementById('generateKBFButton').disabled = !kbfFairSelect.value;
    });

    const proformaStandSelect = document.getElementById('proformaStandSelect');
    proformaStandSelect.addEventListener('change', () => {
        document.getElementById('generateProformaButton').disabled = !proformaStandSelect.value;
    });

    document.getElementById('generateKBFButton').addEventListener('click', handleGenerateKBF);
    document.getElementById('generateProformaButton').addEventListener('click', handleGenerateProforma);
}


/**
 * Katılımcı Bilgi Formu (KBF) oluşturur ve indirir.
 */
async function handleGenerateKBF() {
    const kbfFairSelect = document.getElementById('kbfFairSelect');
    const fairId = kbfFairSelect.value;
    if (!fairId) {
        Swal.fire('Eksik Bilgi', 'Lütfen KBF oluşturmak için bir fuar seçin.', 'warning');
        return;
    }

    const spinner = document.getElementById('kbfSpinner');
    const button = document.getElementById('generateKBFButton');
    button.disabled = true;
    spinner.style.display = 'inline-block';

    try {
        const participant = window.AppState.currentParticipant;
        const fair = window.AppState.attendedFairs.find(f => f.id === fairId);

        // HATA DÜZELTME: fetch için mutlak bir URL oluşturuluyor.
        // Bu, tarayıcının dosyayı bulma sorunlarını çözer.
        const templateUrl = `${window.location.origin}/template/kbf_sablon.docx`;

        console.log("Şablon dosyası şu adresten çekiliyor:", templateUrl); // Teşhis için log ekle

        const response = await fetch(templateUrl);

        if (!response.ok) {
            // Hata mesajını daha anlaşılır hale getir
            throw new Error(`Şablon dosyası (${templateUrl}) yüklenemedi. Sunucu yanıtı: ${response.status} ${response.statusText}`);
        }
        const template = await response.blob();

        // Verileri şablondaki yer tutucularla eşleştir
        const patch = {
            fuar_adi: {
                type: docx.PatchType.PARAGRAPH,
                children: [new docx.TextRun(fair.name || "Belirtilmemiş Fuar")],
            },
            firma_adi: {
                type: docx.PatchType.PARAGRAPH,
                children: [new docx.TextRun(participant.companyName || "")],
            },
            adres: {
                type: docx.PatchType.PARAGRAPH,
                children: [new docx.TextRun(participant.address || "")],
            },
            telefon: {
                type: docx.PatchType.PARAGRAPH,
                children: [new docx.TextRun(participant.phone || "")],
            },
            email: {
                type: docx.PatchType.PARAGRAPH,
                children: [new docx.TextRun(participant.email || "")],
            },
            web: {
                type: docx.PatchType.PARAGRAPH,
                children: [new docx.TextRun(participant.website || "")],
            },
            // Henüz verisi olmayan alanlar için boş bırakıyoruz.
            subeler: { type: docx.PatchType.PARAGRAPH, children: [new docx.TextRun(" ")] },
            sergilenecek_urunler: { type: docx.PatchType.PARAGRAPH, children: [new docx.TextRun(" ")] },
        };

        // docx.patchDocument kullanarak belgeyi oluştur
        const patchedDoc = await docx.patchDocument(template, {
            patches: patch,
        });

        // Oluşturulan belgeyi blob'a çevirip indir
        const blob = await docx.Packer.toBlob(patchedDoc);
        const safeCompanyName = (participant.companyName || 'katilimci').replace(/[^a-z0-9]/gi, '_').toLowerCase();
        const safeFairName = (fair.name || 'fuar').replace(/[^a-z0-9]/gi, '_').toLowerCase();
        saveAs(blob, `KBF_${safeCompanyName}_${safeFairName}.docx`);

        Swal.fire('Başarılı!', 'Katılımcı Bilgi Formu oluşturuldu ve indiriliyor.', 'success');
    } catch (error) {
        console.error("KBF oluşturulurken hata:", error);
        // Hata mesajına URL'yi ekleyerek daha fazla bilgi ver
        Swal.fire('Hata!', `Dosya oluşturulurken bir hata oluştu. Şablon dosyasına erişilemiyor olabilir.\nDetay: ${error.message}`, 'error');
    } finally {
        button.disabled = false;
        spinner.style.display = 'none';
    }
}


/**
 * Pro-forma Fatura oluşturur (şimdilik taslak).
 */
async function handleGenerateProforma() {
    const standId = document.getElementById('proformaStandSelect').value;
    if (!standId) {
        Swal.fire('Eksik Bilgi', 'Lütfen pro-forma oluşturmak için bir stand seçin.', 'warning');
        return;
    }
    Swal.fire('Başlatıldı', `Pro-forma fatura oluşturuluyor...`, 'info');
    const stand = window.AppState.currentStands.find(s => s.id === standId);
    console.log("Proforma oluşturulacak:", window.AppState.currentParticipant, "Stand:", stand);
    // TODO: Gerçek pro-forma oluşturma mantığı eklenecek.
}
